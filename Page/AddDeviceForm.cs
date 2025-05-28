using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TRoaster.Entity;
using TRoaster.Helper;

namespace TRoaster
{
    public partial class AddDeviceForm : Form
    {
        public string triggerSource;
        internal Action<DeviceInfoEntity> DeviceInfoAction;
        public String selectDevice;
        public AddDeviceForm()
        {
            InitializeComponent();
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
        private void button1_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(triggerSource) && triggerSource == "1")
            {
                DeviceInfoEntity deviceInfoEntity = new DeviceInfoEntity();
                if (!string.IsNullOrEmpty(devicetext.Text))
                {
                    deviceInfoEntity.DEVICE_NAME = devicetext.Text;
                }
                else
                {
                    MessageBox.Show("设备编号不能为空", "添加异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    deviceInfoEntity.IP = textBox1.Text;
                }
                else
                {
                    MessageBox.Show("IP不能为空", "添加异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!string.IsNullOrEmpty(numericUpDown1.Value.ToString()))
                {
                    deviceInfoEntity.PORT = numericUpDown1.Value.ToString();
                }
                else
                {
                    MessageBox.Show("Port不能为空", "添加异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                deviceInfoEntity.SERVICE_ADDR = GetIPAddr();
                SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();
                try
                {
                    int i = sqlClient.Insertable(deviceInfoEntity).ExecuteCommand();
                    if (i > 0)
                    {
                        DeviceInfoAction?.Invoke(deviceInfoEntity);
                        MessageBox.Show("添加成功");
                    }
                    else
                    {
                        MessageBox.Show("保存数据失败，请重新重试", "添加异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("数据重复添加:"+ex.Message, "添加异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (!string.IsNullOrEmpty(triggerSource) && triggerSource == "2")
            {
                SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();
                DeviceInfoEntity deviceInfoEntity = new DeviceInfoEntity();
                try
                {
                    if (string.IsNullOrEmpty(devicetext.Text))
                    {
                        MessageBox.Show("设备编号不能为空", "添加异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        deviceInfoEntity.DEVICE_NAME = devicetext.Text;
                    }

                    if (string.IsNullOrEmpty(textBox1.Text))
                    {
                        MessageBox.Show("IP不能为空", "添加异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        deviceInfoEntity.IP = textBox1.Text;
                    }

                    if (string.IsNullOrEmpty(numericUpDown1.Value.ToString()))
                    {
                        MessageBox.Show("Port不能为空", "添加异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        deviceInfoEntity.PORT = numericUpDown1.Value.ToString();
                    }

                    int i = sqlClient.Updateable(deviceInfoEntity).Where(a => a.DEVICE_NAME == selectDevice).ExecuteCommand();
                    if (i > 0)
                    {

                        MessageBox.Show("修改成功");
                    }
                    else
                    {
                        MessageBox.Show("保存数据失败，请重新重试", "修改异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("数据重复添加:"+ex.Message, "修改异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void AddDeviceForm_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectDevice))
            {
                SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();
                DeviceInfoEntity deviceInfoEntity;
                deviceInfoEntity = sqlClient.Queryable<DeviceInfoEntity>().Where(a => a.DEVICE_NAME == selectDevice).Single();
                if (deviceInfoEntity != null)
                {
                    textBox1.Text = deviceInfoEntity.IP;
                    devicetext.Text = deviceInfoEntity.DEVICE_NAME;
                    numericUpDown1.Value = Convert.ToDecimal(deviceInfoEntity.PORT);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
