using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Service
{
    internal class CancelTrackIn
    {
        public void cancelTrackIn(String deviceName) { 
           bool isSuccess = GlobleCache.lotinfoDic.Remove(deviceName);
            if (isSuccess)
            {
                Log4netHelper.Info(deviceName + "：清理批次完成");
            }
            else {
                Log4netHelper.Info(deviceName + "：批次未能清理成功");
            }
        }
    }
}
