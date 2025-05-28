using Quartz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TRoaster.Entity;
using TRoaster.Repository;
using TRoaster.Schedule;

namespace TRoaster
{
    public partial class DeviceInfoForm : Form
    {
        public BindingList<DeviceModel> deviceModels { get; set; }
        public int operationBtn;
        private IScheduler scheduler;
        public Action<String> deleteDevice { get; set; }
        public DeviceInfoForm(IScheduler scheduler)
        {
            InitializeComponent();
            this.scheduler = scheduler;
        }
        public Action<string> eqpAction { get; set; }
        private void dataGridView1_Click(object sender, EventArgs e)
        {
            int rowIndex = dataGridView1.CurrentCell.RowIndex;
            dataGridView1.Rows[rowIndex].Selected = true;
            String deviceName = dataGridView1.Rows[rowIndex].Cells[0].Value.ToString();
            if (operationBtn == 1)
            {
                
                AddDeviceForm addDevice = new AddDeviceForm();
                addDevice.button1.Text = "修改";
                addDevice.triggerSource = "2";
                addDevice.selectDevice = deviceName;
                addDevice.Show();
            }
            else if (operationBtn == 2) {
                if (MessageBox.Show("确认要删除吗", "删除设备", MessageBoxButtons.OKCancel) == DialogResult.OK) {
                    DeviceinfoRepository deviceinfoRepository = new DeviceinfoRepository();
                    int i =deviceinfoRepository.DeleteDeviceInfo(deviceName);
                    if (i > 0)
                    {
                        dataGridView1.Rows.RemoveAt(rowIndex);
                        removeJob(deviceName);
                         
                        bool cahceInfo = GlobleCache.deviceInfos.Remove(deviceName);
                        if (!cahceInfo) {
                            MessageBox.Show("未能移除缓存信息", "删除设备", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        bool mainPage = GlobleCache.deviceLableInfos.Remove(deviceName);
                        if (!cahceInfo)
                        {
                            MessageBox.Show("未能删除界面信息", "删除设备", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        deleteDevice?.Invoke(deviceName);
                        MessageBox.Show("删除成功", "删除设备", MessageBoxButtons.OK);
                    }
                    else {
                        MessageBox.Show("删除失败，请重试", "删除设备", MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void removeJob(String deviceName)
        {
            JobKey jobKey = GlobleCache.jobkeys[deviceName + "_status"];
            TriggerKey triggerKey = GlobleCache.triggerkeys[deviceName + "_status"];
            //移除定时
            scheduler.PauseJob(jobKey);//停止触发器  
            scheduler.UnscheduleJob(triggerKey);//移除触发器  
            scheduler.DeleteJob(jobKey);//删除任务  
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DeviceInfoForm_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = deviceModels;
            dataGridView1.Refresh();
              
        }
    }
}
