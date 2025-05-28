 
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Util;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TRoaster.Core;
using TRoaster.Interface;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TRoaster.Schedule.Job
{
    internal class DeviceStatusJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                IScheduler scheduler = context.Scheduler;
                JobDataMap dataMap = context.JobDetail.JobDataMap; 
                String deviceName = dataMap.GetString("deviceName"); 
                GlobleCache.ConnectionDic[deviceName] = false; 
            });

        }
    }
}
