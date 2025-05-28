using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;
using TRoaster.Entity;
using TRoaster.Helper;
using TRoaster.Repository;
using TRoaster.Service;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Core.Plugin
{
    public class ConstTempStartPlugin : PluginBase, ITcpReceivedPlugin<TcpClient>
    {

        public async Task OnTcpReceived(TcpClient client, ReceivedDataEventArgs e)
        {
            ByteBlock byteBlock = e.ByteBlock;

            string recvmsg = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);
            string msg = recvmsg.Substring(0, recvmsg.IndexOf(';') + 1);

            bool isMatch = DataParser.IsMatch(msg, OperationCommands.ConstanTempStart);
            if (isMatch)
            {
                string deviceName = DataParser.GetDeviceName(msg);
                string tempData = DataParser.GetData(msg);
                //恒温开始
                Log4netHelper.Info("恒温开始");
                LotManager lotManager = new LotManager();
                //回复消息
                ReplyMessage(client, deviceName);
                Task.Run(() => {
                    SaveTempData(tempData, deviceName);
                     
                    //填充结束时间
                    lotManager.SetConstTempBeginDate(deviceName);
                    lotManager.IniWriteValue("Lot", "ConstTempBeginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    UpdateFinishedTime(deviceName);
                    lotManager.SetStage(deviceName, "TEMP2");
                    //拉取数据，开始校验
                    bool isPass1 = CheckRealTemp(deviceName, tempData);
                    if (!isPass1)
                    {
                        lotManager.SetResult(deviceName, "check fail!");
                        Log4netHelper.Info("温度比对不通过");
                    }
                    bool isPass2 = CheckBakeTime(deviceName);
                    if (!isPass2)
                    {
                        lotManager.SetResult(deviceName, "check fail!");
                        Log4netHelper.Info("时长比对不通过");
                    }
                    lotManager.IniWriteValue("Lot", "stage", "TEMP2");
                    
                    DeviceLabel deviceLabel = GlobleCache.deviceLableInfos[deviceName];
                    if (deviceLabel != null)
                    {
                        deviceLabel.Temperature = "温度：" + tempData + "℃";
                    }
                });
                e.Handled = true;
            }
            await e.InvokeNext();
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
        private void ReplyMessage(TcpClient client, string deviceName)
        {
            try
            {
                string sendMsg = deviceName + ",,;";
                client.Send(sendMsg);
                Log4netHelper.Info("常温开始消息回复：" + sendMsg);
            }
            catch (Exception ex)
            {
                Log4netHelper.Info("常温开始回复数据出错:" + ex.Message);
            }
        }
        public void UpdateFinishedTime(string deviceName)
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
                    bakeInfo.CONST_TEMP_BEGIN_TIME = DateTime.Now;
                });
                int i = bakeInfoRepository.UpdateConstTempBeginTime(bakeInfos,deviceName,lotInfoModel.programName, works);
                if (bakeInfos.Count != i)
                {
                    Log4netHelper.Info("更新烘箱数据出现不一致：总共" + bakeInfos.Count + ",实际更新数量：" + i);
                    return;
                }
                Log4netHelper.Info("烘箱常温开始数据更新成功");
            }
        }
        private bool CheckRealTemp(string deviceName, string tempDate)
        {
            LotManager lotManager = new LotManager();
            List<RecipeModel> recipeModels = lotManager.GetRecipeList(deviceName);
            String stage = lotManager.GetStage(deviceName);
            Log4netHelper.Info("烘烤阶段："+stage);
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
        private bool CheckBakeTime(string deviceName)
        {
            DateTime beginBake;
            Double Time1Tick=0;
            LotManager lotManager = new LotManager();
            List<RecipeModel> recipeModels = lotManager.GetRecipeList(deviceName);
            LotInfoModel lotInfo = lotManager.GetLotInfo(deviceName);
            if (lotInfo != null) {
                beginBake = lotInfo.BakeBeginTime;
                Time1Tick = DateTime.Now.Subtract(beginBake).TotalMinutes;
            }
            if (recipeModels != null && recipeModels.Count > 0)
            {
                foreach (var item in recipeModels)
                {
                    try
                    {
                        if (item.ItemName == "TIME1")
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
                        Log4netHelper.Info("比对升温时长出错:" + ex.Message);
                        return false;
                    }
                }
            }
            Log4netHelper.Info("recipe未维护，参数没找到");
            return false;
        }
    }
}
