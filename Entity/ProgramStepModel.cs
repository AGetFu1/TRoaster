using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRoaster.Entity
{
    public class ProgramStepModel
    {
        //{"roastcode":"10-004","stepname":"BAKE烘烤","stepcode":"15015","stepcategorycode":"15","stepcategory":"测试"}
        public string roastcode { get; set; }
        public string stepname { get; set; }   
        public string stepcode { get; set; }
        public string stepcategorycode { get; set; }
        public string stepcategory { get; set; }
    }
}
