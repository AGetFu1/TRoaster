using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRoaster.Entity
{
    public class LotInfoModel
    {
        public string DeviceName { get; set; }
        //public List<string> WorkCards { get; set; }
        public List<CardModel> Cards { get; set; }
        public String programName { get; set; }
        public DateTime BakeBeginTime { get; set; }
        public DateTime BakeEndTime { get; set; }
        public DateTime ConstTempBeginTime { get; set; }
        public DateTime ConstTempEndTime { get; set; }
        public String Result { get; set; }

        public String Stage { get; set; }
    }
}
