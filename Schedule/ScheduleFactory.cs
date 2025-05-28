using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRoaster.Schedule
{
    internal class ScheduleFactory
    {
        private static IScheduler _Scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
        public static IScheduler GetScheduler()
        {
            return _Scheduler;
        }
 
    }
}
