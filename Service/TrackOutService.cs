 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRoaster.Interface;

namespace TRoaster.Service
{
    internal class TrackOutService
    {

        public void trackout(String deviceName) {
            //recipe校验比对
            check();
            //调用接口进行出站
            HoldLot();
        }
        public bool check()
        {
            return false;
        }
        public void HoldLot() {
            //ErrorEntity error = new MESInterface().HoldLot("", "", "", "");
        }
    }
}
