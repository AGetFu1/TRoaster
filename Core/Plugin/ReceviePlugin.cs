using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;
using TRoaster.Schedule.Job;
using TRoaster.Schedule;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Core.Plugin
{
    public class ReceviePlugin : PluginBase, ITcpReceivedPlugin<TcpClient>
    {
        IScheduler scheduler;
        public async Task OnTcpReceived(TcpClient client, ReceivedDataEventArgs e)
        {
            ByteBlock byteBlock = e.ByteBlock;
            string mes = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);
            string deviceName = DataParser.GetDeviceName(mes);
            GlobleCache.ConnectionDic[deviceName] = true;
            Log4netHelper.Info("接受到消息："+mes);
            //移除上一个定时
            if (GlobleCache.jobkeys.ContainsKey(deviceName + "_DeviceStatus")) {
                JobKey jobKey = GlobleCache.jobkeys[deviceName + "_DeviceStatus"];
                TriggerKey triggerKey = GlobleCache.triggerkeys[deviceName + "_DeviceStatus"];
                //移除定时
                scheduler.PauseJob(jobKey);//停止触发器  
                scheduler.UnscheduleJob(triggerKey);//移除触发器  
                scheduler.DeleteJob(jobKey);//删除任务  
            } 
            
            //开启定时
            JobFactory jobFactory = new JobFactory();
            scheduler = ScheduleFactory.GetScheduler();
            await scheduler.Start();

            Dictionary<string, Object> messagedic1 = new Dictionary<string, Object>();
            //messagedic1["messageBox"] = textBox1;
            messagedic1["deviceName"] = deviceName;

            JobKey lotjobKey = new JobKey(deviceName + "DeviceStatusjob", deviceName + "DeviceStatusjob");
            TriggerKey lottriggerKey = new TriggerKey(deviceName + "DeviceStatustrriger", deviceName + "DeviceStatusgroup");
            
            GlobleCache.jobkeys.AddOrUpdate(deviceName + "_DeviceStatus", lotjobKey);
             
            GlobleCache.triggerkeys.AddOrUpdate(deviceName + "_DeviceStatus", lottriggerKey);
            IJobDetail statusjob = jobFactory.CreateJob<DeviceStatusJob>(lotjobKey.Name, lotjobKey.Group, messagedic1);
            ITrigger statustrriger = jobFactory.CreateTrigger(lottriggerKey.Name, lottriggerKey.Group, 90);
            scheduler.ScheduleJob(statusjob, statustrriger).Wait();

            await e.InvokeNext();
        }
    }
}
