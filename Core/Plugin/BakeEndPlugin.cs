using Dm.filter.log;
using log4net;

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;
using TRoaster.Core.Plugin.EventArgs;
using TRoaster.Entity;
using TRoaster.Helper;
using TRoaster.Interface;
using TRoaster.Repository;
using TRoaster.Service;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Core.Plugin
{
    //烘烤结束：9,OV-379,,;
    public class BakeEndPlugin : PluginBase, ITcpReceivedPlugin<TcpClient>
    {

        public async Task OnTcpReceived(TcpClient client, ReceivedDataEventArgs e)
        {
            ByteBlock byteBlock = e.ByteBlock;
            //结束时间
            DateTime endDateTime = DateTime.Now;
            string recvmsg = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);
            string msg = recvmsg.Substring(0, recvmsg.IndexOf(';') + 1);
            bool isMatch = DataParser.IsMatch(msg, OperationCommands.BakeEnd);
            if (isMatch)
            {
                string deviceName = DataParser.GetDeviceName(msg);
                string tempDate = DataParser.GetData(msg);
                LotManager lotManager = new LotManager();
                //回复
                ReplyMessage(client, deviceName);

                //清批
                Task.Run(() => {
                    String Reason = "";
                    //填充结束时间
                    lotManager.SetBakeEndDate(deviceName);
                    lotManager.IniWriteValue("Lot", "BakeEndTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    
                    SaveTempData(tempDate, deviceName);
                    //参数比对检验
                    
                    bool isPassByTime = CheckTime3(deviceName);
                    if (!isPassByTime)
                    {
                        lotManager.SetResult(deviceName, "时长比对不通过，check fail!");
                        Log4netHelper.Info("时长比对不通过");
                    }
                    bool isPassByParam = CheckRecipeParams(deviceName, tempDate);
                    if (!isPassByParam) {
                        Reason = "实时温度比对不通过，check fail!";
                        Log4netHelper.Info("温度比对不通过");
                        lotManager.SetResult(deviceName, Reason);
                        string res = "";
                        String tempRes = lotManager.IniReadValue("Lot", "Result");
                        if (String.IsNullOrEmpty(tempRes))
                        {
                            res = Reason;
                        }
                        else {
                            res = tempRes + ";" + Reason;
                        }
                        lotManager.IniWriteValue("Lot", "Result", res);
                       
                    }
                    //调用接口进行过站比对
                    LotInfoModel lotInfo = lotManager.GetLotInfo(deviceName);
                    String tmpBeginDate = lotInfo.ConstTempBeginTime.ToString("yyyy-MM-dd HH:mm:ss");
                    String BakeBeginDate = lotInfo.BakeBeginTime.ToString("yyyy-MM-dd HH:mm:ss");
                    String BakeEndDate = lotInfo.BakeEndTime.ToString("yyyy-MM-dd HH:mm:ss");
                    String tmpEndDate = lotInfo.ConstTempEndTime.ToString("yyyy-MM-dd HH:mm:ss");
                    String Result = lotManager.GetResult(deviceName);
                    
                    String IsPass = "N";
                    if (String.IsNullOrEmpty(Result))
                    {
                        IsPass = "Y";
                    }
                    string Res1 = "";
                    lotInfo.Cards.ForEach(f => {
                        Log4netHelper.Info("调用过站接口"); 
                        if (!string.IsNullOrEmpty(lotInfo.Result))
                        {
                            Res1 = string.Join(";", lotInfo.Result.Split(';').ToList().Distinct());
                        }
                        Log4netHelper.Info("接口参数：" + f.WordCard + "\t" + deviceName + "\t" + f.stepcode + "\t" + BakeBeginDate + "\t" + BakeEndDate + "\t" + tmpBeginDate + "\t" + IsPass + "\t" + Res1 + "\t" + tmpEndDate);
                        try
                        {
                            String error = new MESInterface().HoldLot(f.WordCard, deviceName, f.stepcode, BakeBeginDate, BakeEndDate, tmpBeginDate, IsPass, Res1, tmpEndDate);
                            Log4netHelper.Info("RoastCardSetMsg接口返回：" + error);
                        }
                        catch (Exception e)
                        {
                            Log4netHelper.Info("工单："+ f.WordCard+"接口调用失败,"+e.Message);
                        }
                        
                        Task.Delay(2000).Wait();
                    });
                    if (!string.IsNullOrEmpty(Res1))
                    {
                        UpdateReason(deviceName, Res1);
                    }
                    UpdateFinishedTime(deviceName);
                    //更新温度flag 
                    //UpdateTempFlag(deviceName);
                    //清批
                    ClearLot(deviceName);
                    DeviceLabel deviceLabel = GlobleCache.deviceLableInfos[deviceName];
                    if (deviceLabel != null)
                    {
                        deviceLabel.DeviceStatus = "idle";
                        deviceLabel.Temperature = "温度：";
                        deviceLabel.BatchInfo = "工单：";
                    }
                });
                e.Handled = true;
            }
            await e.InvokeNext();
        }

        private string FindStepName(LotInfoModel lotInfo, string card)
        {
            if (lotInfo != null) {
                return lotInfo.Cards.Find(f => f.WordCard == card).stepname;
            }
            return "";
        }

        private string FindStepCode(LotInfoModel lotInfo, string card)
        {
            if (lotInfo != null)
            {
                return lotInfo.Cards.Find(f => f.WordCard == card).stepcode;
            }
            return "";
        }

        private void UpdateTempFlag(string deviceName)
        {
            LotManager lotManager = new LotManager();
            LotInfoModel lotInfoModel = lotManager.GetLotInfo(deviceName);
            DateTime beginTime = lotInfoModel.BakeBeginTime;
            DateTime endTime = lotInfoModel.BakeEndTime;
            List<String> works = new List<string>();
            lotInfoModel.Cards.ForEach(f => {
                works.Add(f.WordCard);
            });
            List<RealTemperatureEntity> tempDatas;
            SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();
            tempDatas = sqlClient.Queryable<RealTemperatureEntity>().Where(a=>a.CREATE_DATE>=beginTime && a.CREATE_DATE<=endTime && a.TOASTER_TEMPERATURE_FLAG=="0" && works.Contains(a.WORK_CARD)).ToList();
            if (tempDatas != null) {
                BakeInfoRepository bakeInfoRepository = new BakeInfoRepository();

                tempDatas.ForEach(a => { 
                    a.TOASTER_TEMPERATURE_FLAG = "1";
                });
                int i = bakeInfoRepository.UpdateTempFlag(tempDatas, deviceName, lotInfoModel.programName);
                if (tempDatas.Count != i)
                {
                    Log4netHelper.Info("更新烘箱温度flag数据出现不一致：总共" + tempDatas.Count + ",实际更新数量：" + i);
                    return;
                }
            }
        }

        private static void SaveTempData(string tempDate, string deviceName)
        {
            LotManager lotManager = new LotManager();
            LotInfoModel lotInfoModel = lotManager.GetLotInfo(deviceName);
            SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();
            List<RealTemperatureEntity> tempDatas = new List<RealTemperatureEntity>();
            if (lotInfoModel != null && lotInfoModel.Cards != null && lotInfoModel.Cards.Count > 0)
            {
                lotInfoModel.Cards.ForEach(a => {
                    RealTemperatureEntity temperatureEntity = new RealTemperatureEntity();
                    temperatureEntity.DEVICE_NAME = deviceName;
                    temperatureEntity.CREATE_DATE = DateTime.Now;
                    temperatureEntity.TEMPERATURE = tempDate;
                    temperatureEntity.PROGRAM_NAME = lotInfoModel.programName;
                    temperatureEntity.WORK_CARD = a.WordCard;
                    temperatureEntity.TOASTER_TEMPERATURE_FLAG = a.Id.ToString();
                    tempDatas.Add(temperatureEntity);
                });
                int i = sqlClient.Insertable(tempDatas).ExecuteCommand();
                if (i > 0)
                {
                    Log4netHelper.Info("实时温度数据保存成功:");
                }
                else
                {
                    Log4netHelper.Info("实时温度数据保存失败:");
                }
            }

            
        }
        private bool CheckTime3(string deviceName)
        {
            
            DateTime beginConst;
            Double Time1Tick = 0;
            LotManager lotManager = new LotManager();
            List<RecipeModel> recipeModels = lotManager.GetRecipeList(deviceName);
            LotInfoModel lotInfo = lotManager.GetLotInfo(deviceName);
            if (lotInfo != null)
            {
                beginConst = lotInfo.ConstTempEndTime;
                Time1Tick = DateTime.Now.Subtract(beginConst).TotalMinutes;
            }
            if (recipeModels != null && recipeModels.Count > 0)
            {
                foreach (var item in recipeModels)
                {
                    try
                    {
                        if (item.ItemName == "TIME3")
                        {
                            String upVal = item.UpLimit;
                            String downVal = item.DownLimit;
                            if (Time1Tick <= Double.Parse(upVal) && Time1Tick >= Double.Parse(downVal))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log4netHelper.Info("比对常温时长出错:" + ex.Message);
                        return false;
                    }
                }
            }
            Log4netHelper.Info("recipe未维护，参数没找到");
            return false;
        }
        private void ReplyMessage(TcpClient client, string deviceName)
        {
            try
            {
                string sendMsg = deviceName + ",,;";
                client.Send(sendMsg);
                Log4netHelper.Info("烘烤结束消息回复：" + sendMsg);
            }
            catch (Exception ex)
            {
                Log4netHelper.Info("烘烤结束回复数据出错:" + ex.Message);
            }
        }
        private void ClearLot(string deviceName)
        {
            LotManager lotManager = new LotManager();
            lotManager.IniWriteValue("Lot", "DeviceName", "");
            lotManager.IniWriteValue("Lot", "WorkCards","");
            lotManager.IniWriteValue("Lot", "programName", "");
            lotManager.IniWriteValue("Lot", "BakeBeginTime", "");
            lotManager.IniWriteValue("Lot", "BakeEndTime", "");
            lotManager.IniWriteValue("Lot", "ConstTempBeginTime", "");
            lotManager.IniWriteValue("Lot", "ConstTempEndTime", "");
            lotManager.IniWriteValue("Lot", "Result", "");
            lotManager.IniWriteValue("Lot", "stepCode", "");
            lotManager.IniWriteValue("Lot", "stepName", "");
            lotManager.IniWriteValue("Lot", "stage", "");
            LotInfoModel lotInfoModel = lotManager.GetLotInfo(deviceName);
            if (lotInfoModel != null) { 
                lotManager.RemoveLotInfo(deviceName);
                 
                Log4netHelper.Info("批次清理完成");
            }
        }

        public bool CheckRecipeParams(string deviceName, string tempDate)
        {
            LotManager lotManager = new LotManager();
            List<RecipeModel> recipeModels = lotManager.GetRecipeList(deviceName);
            String stage = lotManager.GetStage(deviceName);
            Log4netHelper.Info("烘烤阶段为："+stage);
            if (recipeModels != null && recipeModels.Count > 0)
            {
                foreach (var item in recipeModels)
                {
                    try
                    {
                        if (item.ItemName == stage)
                        {
                            String upVal = item.UpLimit;
                            String downVal = item.DownLimit;
                            if (Double.Parse(tempDate) <= Double.Parse(upVal) && Double.Parse(tempDate) >= Double.Parse(downVal))
                            {
                                return true;
                            }
                            else { 
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log4netHelper.Info("比对实时温度出错:" + ex.Message);
                        return false;
                    }
                }
            }
            Log4netHelper.Info("recipe未维护，参数没找到");
            return false;
        }
        public void UpdateFinishedTime(string deviceName) {
            LotManager lotManager = new LotManager();
            LotInfoModel lotInfoModel = lotManager.GetLotInfo(deviceName);
            if (lotInfoModel != null)
            {
                List<String> works = new List<string>();
                lotInfoModel.Cards.ForEach(f => {
                    works.Add(f.WordCard);
                });
                BakeInfoRepository bakeInfoRepository = new BakeInfoRepository();
                List<BakeInfoEntity> bakeInfos = bakeInfoRepository.GetAllLotInfo(deviceName, lotInfoModel.programName, works);
                bakeInfos.ForEach(bakeInfo => {
                    bakeInfo.BAKE_END_TIME = DateTime.Now;
                });
                int i = bakeInfoRepository.UpdateFinishedDateTime(bakeInfos,deviceName,lotInfoModel.programName, works);
                if (bakeInfos.Count != i)
                {
                    Log4netHelper.Info("更新烘箱数据出现不一致：总共" + bakeInfos.Count + ",实际更新数量：" + i); 
                    return;
                }
                
                Log4netHelper.Info("烘箱数据更新成功");
            }
        }
        public void UpdateReason(string deviceName,string Reason)
        {
            LotManager lotManager = new LotManager();
            LotInfoModel lotInfoModel = lotManager.GetLotInfo(deviceName);
            if (lotInfoModel != null)
            {
                List<String> works = new List<string>();
                lotInfoModel.Cards.ForEach(f => {
                    works.Add(f.WordCard);
                });
                BakeInfoRepository bakeInfoRepository = new BakeInfoRepository();
                List<BakeInfoEntity> bakeInfos = bakeInfoRepository.GetAllLotInfo(deviceName, lotInfoModel.programName, works);
                bakeInfos.ForEach(bakeInfo => {
                    bakeInfo.REMARK = Reason;
                });
                int i = bakeInfoRepository.UpdateReason(bakeInfos, deviceName, lotInfoModel.programName, works);
                if (bakeInfos.Count != i)
                {
                    Log4netHelper.Info("更新烘箱数据出现不一致：总共" + bakeInfos.Count + ",实际更新数量：" + i);
                    return;
                }
                Log4netHelper.Info("烘箱数据更新成功");
            }
        }
    }
}
