using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRoaster.Schedule
{
    public class JobHolder
    {
        public object JobReturnValue { get; private set; }

        public void Execute(IJobExecutionContext context, IJob job)
        {
            JobReturnValue = job.Execute(context);
        }
    }
}
