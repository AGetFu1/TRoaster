using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TRoaster
{
    public partial class DeviceLabel : UserControl
    {
        private Label lblDeviceName;
        private Label lblTemperature;
        public Label lblWorkCard;
        private Label lblBatchInfo;
        private ContextMenuStrip contextMenu;
        public String IP;
        public int Port;
        public DeviceLabel()
        {
            InitializeComponent();
        }
        void DeviceStatusControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // 显示右键菜单
                contextMenu.Show(this, e.Location);
            }
        }

        // 菜单项点击事件处理
        void OnMenuItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                //MessageBox.Show("您点击了: " + menuItem.Text);
                clickAction?.Invoke(menuItem.Text, lblDeviceName.Text);
            }
        }
    }
}
