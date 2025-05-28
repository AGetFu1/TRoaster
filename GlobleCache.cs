using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRoaster.Core;
using TRoaster.Entity;

namespace TRoaster
{
    internal static class GlobleCache
    {
       public static Dictionary<String,DeviceModel> deviceInfos = new Dictionary<String,DeviceModel>();
       public static Dictionary<String, RoasterClient> deviceClients = new Dictionary<String, RoasterClient>();
       public static Dictionary<String,DeviceLabel> deviceLableInfos = new Dictionary<String, DeviceLabel>();
       public static Dictionary<String,JobKey> jobkeys = new Dictionary<String, JobKey>();
       public static Dictionary<String,TriggerKey> triggerkeys = new Dictionary<String, TriggerKey>();
       public static Dictionary<String, LotInfoModel> lotinfoDic = new Dictionary<string, LotInfoModel>();
       public static Dictionary<String, List<RecipeModel>> RecipeListDic = new Dictionary<string, List<RecipeModel>>();
       public static Dictionary<String, bool> ConnectionDic = new Dictionary<string, bool>();
       public static TextBox textBox;
       public static SplitContainer splitContainer;
    }
}
