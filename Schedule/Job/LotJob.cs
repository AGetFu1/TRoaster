
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRoaster.Core;
using TRoaster.Entity;
using TRoaster.Interface;
using TRoaster.Service;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Schedule.Job
{
    internal class LotJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
               
                IScheduler scheduler = context.Scheduler;
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                
                String deviceName = dataMap.GetString("deviceName");
                Log4netHelper.Info(deviceName + "：达到超时时间未完成扫描工作，将清空之前批次信息");
                LotManager lotManager = new LotManager();
                LotInfoModel lotInfo = lotManager.GetLotInfo(deviceName);
                if (lotInfo != null) {
                    CancelTrackIn cancelTrackIn = new CancelTrackIn();
                    cancelTrackIn.cancelTrackIn(deviceName);
                }
                Log4netHelper.Info(deviceName + "：已清空超时历史批次信息");
                JobKey jobKey = GlobleCache.jobkeys[deviceName + "_Lot"];
                TriggerKey triggerKey = GlobleCache.triggerkeys[deviceName + "_Lot"];
                //移除定时
                scheduler.PauseJob(jobKey);//停止触发器  
                scheduler.UnscheduleJob(triggerKey);//移除触发器  
                scheduler.DeleteJob(jobKey);//删除任务  
            });
        }
    }
}
