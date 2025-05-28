using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;
using TRoaster.Core.Plugin.EventArgs;
using TRoaster.Entity;
using TRoaster.Schedule.Job;
using TRoaster.Schedule;
using TRoaster.Service;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Core.Plugin
{
    public class WorkorderStartPlugin : PluginBase, ITcpReceivedPlugin<TcpClient>
    {

        public async Task OnTcpReceived(TcpClient client, ReceivedDataEventArgs e)
        {
            ByteBlock byteBlock = e.ByteBlock;
            IScheduler scheduler;
            string msg = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);

            bool isMatch = DataParser.IsMatch(msg, OperationCommands.StartWorkOrderEntry);
            if (isMatch)
            {
                string deviceName = DataParser.GetDeviceName(msg);
                //回复
                ReplyMessage(client, deviceName);
                //工单扫描开始，开批前进行校验
                Task.Run(async () => {
                    //清批
                    LotManager lotManager = new LotManager();
                    LotInfoModel lotInfo=lotManager.GetLotInfo(deviceName);
                    if (lotInfo != null) {
                        Log4netHelper.Info("存在历史批次信息未清理干净。");
                        lotManager.RemoveLotInfo(deviceName);
                        Log4netHelper.Info("已清理批次信息完成。");
                    }
                    //30分钟后不进行烘烤进行清批
                    //开启定时器
                    JobFactory jobFactory = new JobFactory();
                    scheduler = ScheduleFactory.GetScheduler();
                    await scheduler.Start();

                    Dictionary<string, Object> messagedic1 = new Dictionary<string, Object>();
                    //messagedic1["messageBox"] = textBox1;
                    messagedic1["deviceName"] = deviceName;

                    JobKey lotjobKey = new JobKey(deviceName + "Lotjob", deviceName + "Lotjob");
                    TriggerKey lottriggerKey = new TriggerKey(deviceName + "Lottrriger", deviceName + "Lotgroup");
                    GlobleCache.jobkeys.Add(deviceName + "_Lot", lotjobKey);
                    GlobleCache.triggerkeys.Add(deviceName + "_Lot", lottriggerKey);
                    IJobDetail statusjob = jobFactory.CreateJob<LotJob>(lotjobKey.Name, lotjobKey.Group, messagedic1);
                    ITrigger statustrriger = jobFactory.CreateTrigger(lottriggerKey.Name, lottriggerKey.Group, 60*60*2);
                    scheduler.ScheduleJob(statusjob, statustrriger).Wait(); 
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
                Log4netHelper.Info("开始录入工单消息回复：" + sendMsg);
            }
            catch (Exception ex)
            {
                Log4netHelper.Info("开始录入工单回复数据出错:" + ex.Message);
            }
        }
    }
}
