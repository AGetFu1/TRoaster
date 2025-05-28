using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRoaster.Entity
{
    [SqlSugar.SugarTable("TMS_BAKE_INFO")]
    public class BakeInfoEntity
    {
      [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
       public int Id { get; set; }
       public string LOT_ID { get; set; }
       public string DEVICE_ID { get; set; }
       public string PROGROM_NAME { get; set; }
        /// <summary>
        /// 烘烤开始时间
        /// </summary>
        [SqlSugar.SugarColumn(DefaultValue = null)]
        public DateTime? BAKE_BEGIN_TIME { get; set; }
        /// <summary>
        /// 烘烤结束时间
        /// </summary>
        [SqlSugar.SugarColumn(DefaultValue = null)]
        public DateTime? BAKE_END_TIME { get; set; }
        /// <summary>
        /// 恒温开始时间
        /// </summary>
        [SqlSugar.SugarColumn(DefaultValue = null)]
        public DateTime? CONST_TEMP_BEGIN_TIME { get; set; }
        /// <summary>
        /// 恒温结束时间
        /// </summary>
        [SqlSugar.SugarColumn(DefaultValue = null)]
        public DateTime? CONST_TEMP_END_TIME { get; set; }
        public string STEP_CODE { get; set; }
        public string STEP_NAME { get; set; }
         
       public string REMARK { get; set; }
       public string BANK_INFO_FLAG { get; set; }
    }
}
