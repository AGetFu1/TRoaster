using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRoaster.Entity
{
    [SqlSugar.SugarTable("TMS_DEVICE_INFO")]
    internal class DeviceInfoEntity
    {
        public string IP { get; set; }
        public string PORT { get; set; }
        public string DEVICE_TYPE { get; set; }
        public string DEVICE_NAME { get; set; }
        public string DEVICE_MODEL { get; set; }
        public string SERVICE_ADDR { get; set; }
        public string InterFaceType { get; set; }
        public string RevEndodingStr { get; set; }
        public string SendEncodingStr { get; set; }
        public string EqProgramFTPServer { get; set; }
        public string EqProgramFTPID { get; set; }
        public string EqProgramFTPPwd { get; set; }
        public string EqProgramFTPPath { get; set; }
        public string EqProgramPostFix { get; set; }
        public bool Enable { get; set; }
        public bool IsSECS { get; set; }
    }
}
