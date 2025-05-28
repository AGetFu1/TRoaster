using log4net;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;
using TRoaster.Core.Plugin.EventArgs;
using TRoaster.Entity;
using TRoaster.Helper;
using TRoaster.Interface;
using TRoaster.Service;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Core.Plugin
{


    //实时温度：2,OV-379,temp,125.2;
    //烘烤工单：3,OV-379,workorder,WA020G30102-02;
    //开始录入工单：4,OV-379,workorder,;
    //录入工单完成：5,OV-379,workorder,;
    //删除当前工单：6,OV-379,workorder,WA020G30102-02;
    //删除全部工单：7,OV-379,workorder,;
    //烘烤开始：8,OV-379,,;
    //烘烤结束：9,OV-379,,;

    public class TempPlugin : PluginBase, ITcpReceivedPlugin<TcpClient>//, IRevicePlugin
    { 
        //public async Task Revice(object sender, MessageEventArgs e)
        //{

        //    string mes = e.Message;
        //    if (mes!= null){ 
        //        e.Handled = true;
                   
        //    }
        //    await e.InvokeNext();
        //}
        public async Task OnTcpReceived(TcpClient client, ReceivedDataEventArgs e)
        {
            ByteBlock byteBlock = e.ByteBlock;
            IRequestInfo requestInfo = e.RequestInfo;
            string mes = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);

            bool isMatch = DataParser.IsMatch(mes, OperationCommands.RealTimeTemperature);
            if (isMatch) {
                //获取数据
                string tempDate = DataParser.GetData(mes);
                string deviceName = DataParser.GetDeviceName(mes);
                Log4netHelper.Info("实时温度:" + tempDate);
                Task.Run(() =>
                {
                    //回复
                    ReplyMessage(client, deviceName);

                    //比对
                    bool isPass = CheckRealTemp(deviceName, tempDate);
                     
                    if (!isPass)
                    {
                        LotManager lotManager = new LotManager();
                        lotManager.SetResult(deviceName, "温度比对不通过,check fail!");
                        Log4netHelper.Info("温度比对不通过");
                    }
                    //保存数据
                    //存库
                    SaveTempData(tempDate, deviceName);
                    DeviceLabel deviceLabel = GlobleCache.deviceLableInfos[deviceName];
                    if (deviceLabel != null)
                    {
                        deviceLabel.Temperature = "温度："+tempDate+ "℃";
                    }
                });
                e.Handled = true;
            } 
            await e.InvokeNext();
        }

        private void ReplyMessage(TcpClient client, string deviceName)
        {
            try
            {
                string sendMsg = deviceName + ",,;";
                client.Send(sendMsg);
                Log4netHelper.Info("实时温度消息回复：" + sendMsg);
            }
            catch (Exception ex)
            {
                Log4netHelper.Info("实时温度回复数据出错:" + ex.Message);
            }
        }

        private bool CheckRealTemp(string deviceName,string tempDate)
        {
            LotManager lotManager = new LotManager();
            List<RecipeModel> recipeModels = lotManager.GetRecipeList(deviceName);
            String stage = lotManager.GetStage(deviceName);
            if (recipeModels != null && recipeModels.Count > 0) {
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

        private static void SaveTempData(string tempDate, string deviceName)
        {
            LotManager lotManager = new LotManager();
            LotInfoModel lotInfoModel = lotManager.GetLotInfo(deviceName);
            SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();
            List<RealTemperatureEntity> tempDatas = new List<RealTemperatureEntity>();
            if (lotInfoModel != null && lotInfoModel.Cards != null && lotInfoModel.Cards.Count > 0) {
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
    }
}
