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
    public class ConstTempEndPlugin : PluginBase, ITcpReceivedPlugin<TcpClient>
    {
        public async Task OnTcpReceived(TcpClient client, ReceivedDataEventArgs e)
        {
            ByteBlock byteBlock = e.ByteBlock;

            string recvmsg = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);
            string msg = recvmsg.Substring(0, recvmsg.IndexOf(';') + 1);
            bool isMatch = DataParser.IsMatch(msg, OperationCommands.ConstanTempEnd);
            if (isMatch)
            {
                string deviceName = DataParser.GetDeviceName(msg);
                string tempData = DataParser.GetData(msg);
                //回复消息
                ReplyMessage(client, deviceName);
                //恒温结束
                Log4netHelper.Info("恒温结束");
                LotManager lotManager = new LotManager();
                
                Task.Run(() => {
                    SaveTempData(tempData, deviceName);
                    //填充结束时间
                    lotManager.SetConstTempEndDate(deviceName);
                    lotManager.IniWriteValue("Lot", "ConstTempEndTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    UpdateFinishedTime(deviceName); 
                    //拉取数据，开始校验
                    bool isPass1 = CheckRealTemp(deviceName, tempData);
                    if (!isPass1)
                    {
                        lotManager.SetResult(deviceName, "温度比对不通过,check fail!");
                        Log4netHelper.Info("温度比对不通过");
                    }
                    bool isPass2 = CheckConstTime(deviceName);
                    if (!isPass2)
                    {
                        lotManager.SetResult(deviceName, "时长比对不通过,check fail!");
                        Log4netHelper.Info("时长比对不通过");
                    }
                    lotManager.SetStage(deviceName, "TEMP3");
                    lotManager.IniWriteValue("Lot", "stage", "TEMP3");
                    //设置结果
                    DeviceLabel deviceLabel = GlobleCache.deviceLableInfos[deviceName];
                    if (deviceLabel != null)
                    { 
                        deviceLabel.Temperature ="温度："+ tempData+ "℃";
                    }
                });
                e.Handled = true;
            }
            await e.InvokeNext();
        }
        private bool CheckRealTemp(string deviceName, string tempDate)
        {
            LotManager lotManager = new LotManager();
            List<RecipeModel> recipeModels = lotManager.GetRecipeList(deviceName);
            String stage = lotManager.GetStage(deviceName);
            if (recipeModels != null && recipeModels.Count > 0)
            {
                foreach (var item in recipeModels)
                {
                    try
                    {
                        if (item.ItemName == "TEMP2")
                        {
                            String upVal = item.UpLimit;
                            String downVal = item.DownLimit;
                            Log4netHelper.Info("TEMP2：" + "Time1Tick" + tempDate + " upVal" + upVal + " downVal" + downVal);
                            if (Double.Parse(tempDate) <= Double.Parse(upVal) && Double.Parse(tempDate) >= Double.Parse(downVal))
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
                        Log4netHelper.Info("比对实时温度出错:" + ex.Message);
                        return false;
                    }
                }
            }
            Log4netHelper.Info("recipe未维护，参数没找到");
            return false;
        }
        private bool CheckConstTime(string deviceName)
        {
            DateTime beginConst;
            Double Time1Tick = 0;
            LotManager lotManager = new LotManager();
            List<RecipeModel> recipeModels = lotManager.GetRecipeList(deviceName);
            LotInfoModel lotInfo = lotManager.GetLotInfo(deviceName);
            if (lotInfo != null)
            {
                beginConst = lotInfo.ConstTempBeginTime;
                Time1Tick = DateTime.Now.Subtract(beginConst).TotalMinutes;
            }
            if (recipeModels != null && recipeModels.Count > 0)
            {
                foreach (var item in recipeModels)
                {
                    try
                    {
                        if (item.ItemName == "TIME2")
                        {
                            String upVal = item.UpLimit;
                            String downVal = item.DownLimit;
                            Log4netHelper.Info("TIME2："+ "Time1Tick"+ Time1Tick+ " upVal"+ upVal+ " downVal"+ downVal);
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
                Log4netHelper.Info("常温结束消息回复：" + sendMsg);
            }
            catch (Exception ex)
            {
                Log4netHelper.Info("常温结束消息回复出错:" + ex.Message);
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
                    bakeInfo.CONST_TEMP_END_TIME = DateTime.Now;
                });
                int i = bakeInfoRepository.UpdateConstTempEndTime(bakeInfos, deviceName, lotInfoModel.programName, works);
                if (bakeInfos.Count != i)
                {
                    Log4netHelper.Info("更新烘箱数据出现不一致：总共" + bakeInfos.Count + ",实际更新数量：" + i);
                    return;
                }
                Log4netHelper.Info("烘箱常温结束数据更新成功");
            }
        }
    }
}
