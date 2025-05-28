using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;
using TRoaster.Core.Plugin.EventArgs;
using TRoaster.Schedule;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Core.Plugin
{
    public class WorkorderEndPlugin : PluginBase, ITcpReceivedPlugin<TcpClient>
    {

        public async Task OnTcpReceived(TcpClient client, ReceivedDataEventArgs e)
        {
            ByteBlock byteBlock = e.ByteBlock;
            IScheduler scheduler;
            string msg = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);

            bool isMatch = DataParser.IsMatch(msg, OperationCommands.WorkOrderEntryComplete);
            if (isMatch)
            {
                string deviceName = DataParser.GetDeviceName(msg);
                Log4netHelper.Info("工单扫描结束。");
                //回复
                ReplyMessage(client, deviceName);
                //工单扫描结束，进行开批
                Task.Run(() => {
                    Log4netHelper.Info("即将移除工单扫描定时。");
                    scheduler = ScheduleFactory.GetScheduler();
                    scheduler.Start();
                    JobKey jobKey = GlobleCache.jobkeys[deviceName + "_Lot"];
                    TriggerKey triggerKey = GlobleCache.triggerkeys[deviceName + "_Lot"];
                    //移除定时
                    scheduler.PauseJob(jobKey);//停止触发器  
                    scheduler.UnscheduleJob(triggerKey);//移除触发器  
                    scheduler.DeleteJob(jobKey);//删除任务  
                    Log4netHelper.Info("已完成移除工单扫描超时定时。");
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
                Log4netHelper.Info("录入工单结束消息回复：" + sendMsg);
            }
            catch (Exception ex)
            {
                Log4netHelper.Info("录入工单结束消息回复数据出错:" + ex.Message);
            }
        }
    }
}
