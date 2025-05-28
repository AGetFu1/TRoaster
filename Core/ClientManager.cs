using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRoaster.Entity;

namespace TRoaster.Core
{
    public class ClientManager
    {
        private String _deviceName { get; }
        RoasterClient _client;
        DeviceModel deviceModel;
        public ClientManager() {
 
        }
        public bool IsConnected() {
            if (_client != null) {
                return _client.Connected;
            }
            return false;
        }
        public void Connect() {
            _client.Connect(deviceModel.IP,int.Parse(deviceModel.Port));
        }
        // 添加设备
        public bool AddDevice(string key, DeviceModel device)
        {
            if (!GlobleCache.deviceInfos.ContainsKey(key))
            {
                GlobleCache.deviceInfos.Add(key, device);
                return true;
            }
            return false;
        }

        // 查询设备
        public DeviceModel GetDevice(string key)
        {
            if (GlobleCache.deviceInfos.ContainsKey(key))
            {
                return GlobleCache.deviceInfos[key];
            }
            return null;
        }

        // 更新设备信息
        public bool UpdateDevice(string key, DeviceModel updatedDevice)
        {
            if (GlobleCache.deviceInfos.ContainsKey(key))
            {
                GlobleCache.deviceInfos[key] = updatedDevice;
                return true;
            }
            return false;
        }

        // 删除设备
        public bool RemoveDevice(string key)
        {
            return GlobleCache.deviceInfos.Remove(key);
        }
        public bool AddDeviceClient(string key, RoasterClient device)
        {
            if (!GlobleCache.deviceClients.ContainsKey(key))
            {
                GlobleCache.deviceClients.Add(key, device);
                return true;
            }
            return false;
        }

        // 查询设备
        public RoasterClient GetDeviceClient(string key)
        {
            if (GlobleCache.deviceClients.ContainsKey(key))
            {
                return GlobleCache.deviceClients[key];
            }
            return null;
        }
        public bool RemoveDeviceClient(string key)
        {
            return GlobleCache.deviceClients.Remove(key);
        }
        public static void AppendText(string text) {
            TextBox textBox = GlobleCache.textBox;
            textBox.Invoke(() => {
                textBox.AppendText(text+"\r\n");
            });
        }
    }
}
