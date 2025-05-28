using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRoaster.Core;
using TRoaster.Entity;
using TRoaster.Schedule;
using TRoaster.Schedule.Job;

namespace TRoaster.Service
{
    internal class TrackInService
    {
        public TrackInService() { }
        IScheduler scheduler;
        public void TrackIn(String deviceName,String workCard) {
            //开启定时器
            JobFactory jobFactory = new JobFactory();
            scheduler = ScheduleFactory.GetScheduler();
            scheduler.Start();
            DeviceModel deviceModel = new DeviceModel();
            Dictionary<string, Object> messagedic1 = new Dictionary<string, Object>();
            messagedic1["client"] = new ModBusTcpClient("", int.Parse(""), deviceName);
            //messagedic1["messageBox"] = textBox1;
            messagedic1["deviceName"] = deviceName;
            JobKey statusjobKey = new JobKey(deviceName + "Lotjob", deviceName + "Lotjob");
            TriggerKey statustriggerKey = new TriggerKey(deviceName + "Lottrriger", deviceName + "Lotgroup");
            GlobleCache.jobkeys.Add(deviceName + "_Lot", statusjobKey);
            GlobleCache.triggerkeys.Add(deviceName + "_Lot", statustriggerKey);
            IJobDetail statusjob = jobFactory.CreateJob<LotJob>(statustriggerKey.Name, statustriggerKey.Group, messagedic1);
            ITrigger statustrriger = jobFactory.CreateTrigger(statustriggerKey.Name, statustriggerKey.Group, 10);
            scheduler.ScheduleJob(statusjob, statustrriger).Wait();

            //收集工单信息
            LotInfoModel lotInfoModel =  GlobleCache.lotinfoDic[deviceName];
            //lotInfoModel.WorkCards.Add(workCard);
            //获取收集是否完成停止信号
            if (true)
            {
                JobKey jobKey = GlobleCache.jobkeys[deviceName + "_Lot"];
                TriggerKey triggerKey = GlobleCache.triggerkeys[deviceName + "_Lot"];
                JobFactory.removeJob(scheduler,jobKey, triggerKey);
            } 
        }
    }
}
