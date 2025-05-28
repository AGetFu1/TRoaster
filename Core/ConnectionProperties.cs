using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRoaster.Core
{
    public enum ModbusType
    {
        ModbusTCP = 0,
        ModbusRTU = 1
    }
    public class ConnectionProperties
    {

        ModbusType modbusType;

        [Browsable(true)]
        [Category("Modbus Type")]
        [Description("Modbus TCP or Modbus RTU")]
        [DisplayName("Modbus Type")]
        public ModbusType ModbusTypeProperty
        {
            get { return modbusType; }
            set { modbusType = value; }
        }

        string connectionName = "Connection #1";

        [Browsable(true)]
        [Category("Connection properties")]
        [Description("Unique Connection Name")]
        [DisplayName("Connection Name")]
        public string ConnectionName
        {
            get { return connectionName; }
            set { connectionName = value; }
        }

        string modbusTCPAddress = "127.0.0.1";

        [Browsable(true)]
        [Category("Connection properties")]
        [Description("IP-Address of Modbus-TCP Server")]
        [DisplayName("IP Address of Modbus-TCP Server")]
        public string ModbusTCPAddress
        {
            get { return modbusTCPAddress; }
            set { modbusTCPAddress = value; }
        }


        int port = 502;
        [Browsable(true)]
        [Category("Connection properties")]
        [Description("Port")]
        [DisplayName("Port")]
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        string comPort = "COM1";
        [Browsable(true)]
        [Category("Connection properties")]
        [Description("Serial COM-Port")]
        [DisplayName("Serial COM-Port")]
        public string ComPort
        {
            get { return comPort; }
            set { comPort = value; }
        }

        int slaveID = 1;
        [Browsable(true)]
        [Category("Connection properties")]
        [Description("Slave ID")]
        [DisplayName("Slave ID")]
        public int SlaveID
        {
            get { return slaveID; }
            set { slaveID = value; }
        }


        bool cyclicFlag = false;
        [Browsable(true)]
        [Category("Connection properties")]
        [Description("Enable cyclic data exchange")]
        [DisplayName("Enable cyclic data exchange")]
        public bool CyclicFlag
        {
            get { return cyclicFlag; }
            set { cyclicFlag = value; }
        }

        int cycleTime = 100;
        [Browsable(true)]
        [Category("Connection properties")]
        [Description("Cycle time for cyclic data exchange")]
        [DisplayName("Cycle time")]
        public int CycleTime
        {
            get { return cycleTime; }
            set { cycleTime = value; }
        }

        System.Collections.Generic.List<FunctionProperties> functionPropertiesList = new System.Collections.Generic.List<FunctionProperties>();
        [Browsable(false)]
        public System.Collections.Generic.List<FunctionProperties> FunctionPropertiesList
        {
            get { return functionPropertiesList; }
            set { functionPropertiesList = value; }
        }

        public EasyModbus.ModbusClient modbusClient;
        public System.Threading.Timer timer;
    }
}
