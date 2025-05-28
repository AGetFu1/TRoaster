using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TRoaster.Entity;
using TRoaster.Helper;

namespace TRoaster.Repository
{
    public class DeviceinfoRepository
    {

        internal List<DeviceInfoEntity> GetDeviceInfos() {
           SqlSugarClient client =  RTMDBConnection.GetDBConnection();
           List<DeviceInfoEntity>  deviceInfoEntities=  client.Queryable<DeviceInfoEntity>().Where(a => a.SERVICE_ADDR == GetIPAddr()).ToList();
            return deviceInfoEntities;
        }
        internal int DeleteDeviceInfo(String deviceName)
        {
            SqlSugarClient client = RTMDBConnection.GetDBConnection();
            int i = client.Deleteable<DeviceInfoEntity>().Where(a => a.DEVICE_NAME == deviceName).ExecuteCommand();
            return i;
        }
        private string GetIPAddr()
        {
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            string localAddress = null;

            foreach (IPAddress ipAddress in localIPs)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    localAddress = ipAddress.ToString();
                    break;
                }
            }
            return localAddress;
        }
    }
}
