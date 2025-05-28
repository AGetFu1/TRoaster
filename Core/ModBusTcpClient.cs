using EasyModbus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRoaster.Helper;

namespace TRoaster.Core
{
    public class ModBusTcpClient
    {
        ModbusClient modbusClient;
        String IPAddress;
        String DeviceName;
        int Port;
        public Action<String> receiveAction;
        public ModBusTcpClient(string iPAddress, int port, string deviceName)
        {
            modbusClient = new ModbusClient();
            modbusClient.ReceiveDataChanged += new ModbusClient.ReceiveDataChangedHandler(UpdateReceiveData);
            modbusClient.SendDataChanged += new ModbusClient.SendDataChangedHandler(UpdateSendData);
            modbusClient.ConnectedChanged += new ModbusClient.ConnectedChangedHandler(UpdateConnectedChanged);
            IPAddress = iPAddress;
            Port = port;
            DeviceName = deviceName;
        }

        private void UpdateConnectedChanged(object sender)
        {
            
        }

        private void UpdateSendData(object sender)
        {
            
        }

        private void UpdateReceiveData(object sender)
        {
            String recStr = Encoding.UTF8.GetString(modbusClient.receiveData);
            String receiveData = "Rx: " + BitConverter.ToString(modbusClient.receiveData).Replace("-", " ") + System.Environment.NewLine;
            receiveAction?.Invoke(receiveData);
        }
        public void connect() {
            try
            {
                modbusClient.IPAddress = IPAddress;
                modbusClient.Port = Port;
                modbusClient?.Connect();
            }
            catch (Exception ex)
            { 

            }
        }
        public bool IsConnect()
        {
            return (bool)modbusClient?.Connected;
    
        }
        public int IdleCheck()
        {
            int[] datas = null;
            if (!modbusClient.Connected)
            {
                connect();

            }
            datas = modbusClient.ReadHoldingRegisters(Constant.LINK, 1);

            int res = datas.First();
            return res;
        }
        public void disconnect()
        {
            modbusClient?.Disconnect();
        }
        public void enable() {
            if (!IsConnect()) {
                connect();
            }
            if (IsConnect()) {
                modbusClient.WriteSingleRegister(Constant.ENABLE, 1);
            }
        }
        public void disable()
        {
            if (!IsConnect())
            {
                connect();
            }
            modbusClient.WriteSingleRegister(Constant.DISABLE,1);
        }
        public void Open() {
            if (!IsConnect())
            {
                connect();
            }
            modbusClient.WriteSingleRegister(Constant.OPEN,1);
        }
        public void Close()
        {
            if (!IsConnect())
            {
                connect();
            }
            modbusClient.WriteSingleRegister(Constant.CLOSE,1);
        } 
        public int[] RealTemperature() {
            if (!IsConnect())
            {
                connect();
            }
            int[] datas = modbusClient.ReadHoldingRegisters(Constant.TEMPERATURE,1);
            //disconnect();
            return datas;
        }
        public int RealDeviceStatus() {
            int[] datas = null;
            if (!IsConnect())
            {
                connect();
            }
            if (!modbusClient.Connected)
            {
                connect();
                
            }
            datas = modbusClient.ReadHoldingRegisters(Constant.STATUS, 1);

            int res = datas.First();
            return res;
        }
        public bool OpenDoorFinish() {
            int res = 0;
            if (!IsConnect())
            {
                connect();
            }
            int[] datas = modbusClient.ReadHoldingRegisters(Constant.OPEN_DOOR_FINISH,1);
            
            res = datas.First();
            if (res == 1) { 
              return true;
            }
            else
            {
                return false;
            } 
        }
        public bool CloseDoorFinish() {
            int res = 0;
            if (!IsConnect())
            {
                connect();
            }
            int[] data = modbusClient.ReadHoldingRegisters(Constant.CLOSE_DOOR_FINISH,1);
           // disconnect();
            res = data.First();
            if (res == 1) { 
                return true ;
            }
            else
            {
                return false ;
            }
        }
        public int ActualTemperature()
        {
            int res = 0;
            if (!IsConnect())
            {
                connect();
            }
            int[] data = modbusClient.ReadHoldingRegisters(Constant.ACTUAL_TEMPERATURE, 1);
            //disconnect();
            res = data.First();
            return res;
        }
        public int ActualTime()
        {
            int res = 0;
            if (!IsConnect())
            {
                connect();
            }
            int[] data = modbusClient.ReadHoldingRegisters(Constant.ACTUAL_TIME, 1);
            //disconnect();
            res = data.First();
            return res;
        }
        public String FetchWorkCard()
        {
            String res = String.Empty;
            if (!IsConnect())
            {
                connect();
            }
            int[] datas = modbusClient.ReadHoldingRegisters(Constant.WORKCARD,20);
            //disconnect();
            
            res = ModbusClient.ConvertRegistersToString(datas,0,datas.Length);
            return res;
        }
        public void Dispose() {
            if (modbusClient != null)
            {
                modbusClient.Disconnect();
                modbusClient = null;
            }
        }

        internal void SetWorkCardStatus(int status)
        {
            if (!IsConnect())
            {
                connect();
            }
            modbusClient.WriteSingleRegister(Constant.WORKCARD_STATUS, status);
        }

        internal void SetIdleStatus(int status)
        {
            if (!IsConnect())
            {
                connect();
            }
            modbusClient.WriteSingleRegister(Constant.LINK, status);
        }

        ~ModBusTcpClient() => Dispose();
    }
}
