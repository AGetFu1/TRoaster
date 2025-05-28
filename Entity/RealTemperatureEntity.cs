using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRoaster.Entity
{
    [SqlSugar.SugarTable("TMS_TOASTER_TEMPERATURE")]
    internal class RealTemperatureEntity
    {
       public string DEVICE_NAME {get;set;}
       public String TEMPERATURE {get;set;}
       public DateTime CREATE_DATE {get;set;}
       public string PROGRAM_NAME { get; set; }
       public string WORK_CARD { get; set; }
       public string TOASTER_TEMPERATURE_FLAG { get; set; }
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
    }
}
