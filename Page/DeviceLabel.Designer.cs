namespace TRoaster
{
    partial class DeviceLabel
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        public Action<string,string> clickAction { get; set; }
        public ToolTip toolTip1;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 初始化标签控件
            lblDeviceName = new Label();
            lblTemperature = new Label();
            lblWorkCard = new Label();
            lblBatchInfo = new Label();
            // 设置标签的属性，使其内容居中
            lblDeviceName.TextAlign = ContentAlignment.MiddleCenter;
            lblTemperature.TextAlign = ContentAlignment.MiddleCenter;
            lblWorkCard.TextAlign = ContentAlignment.MiddleCenter;
            lblBatchInfo.TextAlign = ContentAlignment.MiddleCenter;
            lblDeviceName.Location = new Point(20, 10);
            lblTemperature.Location = new Point(20, 40);
            lblWorkCard.Location = new Point(20, 70);
            lblBatchInfo.Location = new Point(20, 100);

            // 添加标签到控件集合
            this.Controls.Add(lblDeviceName);
            this.Controls.Add(lblTemperature);
            this.Controls.Add(lblWorkCard);
            this.Controls.Add(lblBatchInfo);

            // 初始化右键菜单
            contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("连接", null, OnMenuItemClick);
            contextMenu.Items.Add("断开", null, OnMenuItemClick);
            contextMenu.Items.Add("启用", null, OnMenuItemClick);
            contextMenu.Items.Add("停止", null, OnMenuItemClick);
            contextMenu.Items.Add("开门", null, OnMenuItemClick);
            contextMenu.Items.Add("关门", null, OnMenuItemClick);
            contextMenu.Items.Add("工单", null, OnMenuItemClick);
            contextMenu.Items.Add("状态", null, OnMenuItemClick);
            contextMenu.Items.Add("温度", null, OnMenuItemClick);
            // 鼠标右键事件
            this.ContextMenuStrip = contextMenu;
            this.MouseClick += new MouseEventHandler(DeviceStatusControl_MouseClick);
            // 鼠标点击事件，用于区分左键和右键点击
            
            toolTip1 = new ToolTip();
             

            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientActiveCaption;
            Name = "DeviceLabel";
            ResumeLayout(false);
        }

        // 设备名称属性
        public string DeviceName
        {
            get { return lblDeviceName.Text; }
            set { lblDeviceName.Text = value; }
        }

        // 当前温度属性
        public string Temperature
        {
            get { return lblTemperature.Text; }
            set {
                
                if (this.lblBatchInfo.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        lblTemperature.Text = value;
                    }));
                }
                else
                {
                    lblTemperature.Text = value;
                }
            }
        }

        // 工卡号属性
        public string WorkCard
        {
            get { return lblWorkCard.Text; }
            set {
                if (this.lblWorkCard.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        lblWorkCard.Text = value;
                    }));
                }
                else
                {
                    lblWorkCard.Text = value;
                }
                
            }
        }

        // 批次信息属性
        public string BatchInfo
        {
            get { return lblBatchInfo.Text; }
            set {
                if (this.lblBatchInfo.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        lblBatchInfo.Text = value;
                    }));
                }
                else
                {
                    lblBatchInfo.Text = value;
                } 
            }
        }

        // 设备状态属性
        public string DeviceStatus
        {
            set
            {
                switch (value.ToLower())
                {
                    case "working":
                        this.BackColor = Color.Green; // 工作状态
                        break;
                    case "idle":
                        this.BackColor = Color.Yellow; // 空闲状态
                        break;
                    case "exception":
                        this.BackColor = Color.Red; // 异常状态
                        break;
                }
            }
        }
        #endregion
    }
}
