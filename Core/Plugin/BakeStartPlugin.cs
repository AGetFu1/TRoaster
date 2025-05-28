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
using TRoaster.Repository;
using TRoaster.Service;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Core.Plugin
{
    ////烘烤开始：8,OV-379,,;
    public class BakeStartPlugin : PluginBase, ITcpReceivedPlugin<TcpClient>
    {
        public async Task OnTcpReceived(TcpClient client, ReceivedDataEventArgs e)
        {
            ByteBlock byteBlock = e.ByteBlock;

            string recvmsg = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);
            string msg = recvmsg.Substring(0,recvmsg.IndexOf(';')+1);
            bool isMatch = DataParser.IsMatch(msg, OperationCommands.BakeStart);
            if (isMatch)
            {
                string deviceName = DataParser.GetDeviceName(msg);
                string tempData = DataParser.GetData(msg);
                //开始烘烤后，
                Task.Run(() =>
                {
                    //记录开始烘烤时间
                    Log4netHelper.Info("烘烤开始,烘箱开始启动。。。");

                    LotManager lotManager = new LotManager();
                    LotInfoModel lotInfoModel = lotManager.GetLotInfo(deviceName);
                    //根据proName获取recipe
                    Task.Run(() => {
                        if (lotInfoModel != null)
                        {
                            string prom = lotInfoModel.programName;
                            lotManager.SetStage(deviceName, "TEMP1");
                            lotManager.IniWriteValue("Lot", "stage", "TEMP1");
                        }
                        else {
                            Log4netHelper.Info("当前没有批次信息，不能获取程序名称");
                        }
                        lotManager.SetBakeBeginDate(deviceName);
                        lotManager.IniWriteValue("Lot", "BakeBeginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        SaveBakeInfo(deviceName, lotInfoModel);
                        //设置ID
                        SearchAndSetId(deviceName, lotInfoModel);
                        SaveTempData(tempData, deviceName);
                        //回复消息
                        ReplyMessage(client,deviceName);
                        bool isPass = CheckRealTemp(deviceName, tempData);
                        if (!isPass)
                        {
                            lotManager.SetResult(deviceName, "温度比对不通过，check fail!");
                            Log4netHelper.Info("温度比对不通过");
                        }
                        DeviceLabel deviceLabel = GlobleCache.deviceLableInfos[deviceName];
                        if (deviceLabel != null)
                        {
                            deviceLabel.DeviceStatus = "working";
                            deviceLabel.Temperature = "温度：" + tempData + "℃";
                            deviceLabel.BatchInfo = "程序：" + lotInfoModel.programName;

                            StringBuilder sb = new StringBuilder();
                            lotInfoModel.Cards.ForEach(x =>
                            {
                                sb.Append(x.WordCard).Append("\n");
                            });

                            if (deviceLabel.lblWorkCard.InvokeRequired)
                            {
                                deviceLabel.lblWorkCard.Invoke(new Action(() =>
                                {
                                    deviceLabel.toolTip1.SetToolTip(deviceLabel.lblWorkCard, sb.ToString());
                                }));
                            }
                            else
                            {
                                deviceLabel.toolTip1.SetToolTip(deviceLabel.lblWorkCard, sb.ToString());
                            }
                            deviceLabel.WorkCard = "工单：" + lotInfoModel.Cards.ElementAt(0);
                        }
                    }); 
                    
                });
                e.Handled = true;
            }
            await e.InvokeNext();
        }

        private void SearchAndSetId(string deviceName, LotInfoModel lotInfoModel)
        {
            List<CardModel> cards = new List<CardModel>();
            //Search Id from DB
            List<String> works = new List<string>();
            lotInfoModel.Cards.ForEach(f => {
                works.Add(f.WordCard);
            });
            BakeInfoRepository bakeInfoRepository = new BakeInfoRepository();
            List<BakeInfoEntity> bakeInfos = bakeInfoRepository.GetAllLotInfo(deviceName, lotInfoModel.programName, works);
            //Set Id to tempTB
            if (bakeInfos != null && bakeInfos.Count > 0) {
                bakeInfos.ForEach(x => {
                    CardModel cardModel = new CardModel();
                    cardModel.Id = x.Id;
                    cardModel.WordCard = x.LOT_ID;
                    cardModel.stepcode = x.STEP_CODE;
                    cardModel.stepname = x.STEP_NAME;
                    cards.Add(cardModel);
                });
                LotManager lotManager = new LotManager();
                lotManager.SetIds(deviceName, cards);
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
        private void ReplyMessage(TcpClient client, string deviceName)
        {
            try
            {
                string sendMsg = deviceName + ",,;";
                client.Send(sendMsg);
                Log4netHelper.Info("开始烘烤消息回复："+ sendMsg);
            }
            catch (Exception ex)
            {
                Log4netHelper.Info("开始烘烤回复消息出错:" + ex.Message);
            }
        }
        private static void SaveBakeInfo(string deviceName, LotInfoModel lotInfoModel)
        {
            List<BakeInfoEntity> bakeInfos = new List<BakeInfoEntity>();
            if (lotInfoModel != null && lotInfoModel.Cards != null && lotInfoModel.Cards.Count > 0)
            {
                List<CardModel> lots = lotInfoModel.Cards;
                foreach (var item in lots)
                {
                    BakeInfoEntity bakeInfoEntity = new BakeInfoEntity();
                    bakeInfoEntity.LOT_ID = item.WordCard;
                    bakeInfoEntity.DEVICE_ID = deviceName;
                    bakeInfoEntity.PROGROM_NAME = lotInfoModel.programName;
                    
                    bakeInfoEntity.BAKE_BEGIN_TIME = DateTime.Now;

                    bakeInfoEntity.STEP_CODE = item.stepcode;
                    bakeInfoEntity.STEP_NAME = item.stepname;
                    bakeInfoEntity.BANK_INFO_FLAG = "0";
                    bakeInfos.Add(bakeInfoEntity);
                }
            }
            BakeInfoRepository repository = new BakeInfoRepository();
            repository.InsertAllInfo(bakeInfos);
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
                        if (item.ItemName == stage)
                        {
                            String upVal = item.UpLimit;
                            String downVal = item.DownLimit;
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
            Log4netHelper.Info("recipe未维护，参数没找到" );
            return false;
        }
    }
}
