using log4net.Core;
using log4net.Repository.Hierarchy;
using Quartz;
using Quartz.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRoaster.Core;
using TRoaster.Schedule.Job;
using static Quartz.Logging.OperationName;

namespace TRoaster.Schedule
{
    internal class JobFactory
    {
        public IJobDetail CreateJob<T>(string name, string group, Dictionary<String,Object> messageDic) where T:IJob
        {
            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity(name, group)      
                .Build();
            if(messageDic!=null && messageDic.Count > 0)
            {
                foreach (var param in messageDic)
                {
                    job.JobDataMap.Put(param.Key, param.Value);
                }
            }
            return job;
        }

        public ITrigger CreateTrigger(string name, string group, int intervalInSeconds)
        {
            return TriggerBuilder.Create()
                .WithIdentity(name, group)
                .StartAt(DateBuilder.FutureDate(intervalInSeconds, IntervalUnit.Second))
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(intervalInSeconds)
                    .WithRepeatCount(1))
                .Build();
        }
        public static void removeJob(IScheduler scheduler, JobKey JKey, TriggerKey TKey)
        {
            try
            {
                scheduler.PauseJob(JKey);// 停止触发器  
                scheduler.UnscheduleJob(TKey);// 移除触发器  
                scheduler.DeleteJob(JKey);// 删除任务  
            }
            catch (Exception e)
            {
                
            }
        }
    }
}
