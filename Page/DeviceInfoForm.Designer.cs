namespace TRoaster
{
    partial class DeviceInfoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            deviceName = new DataGridViewTextBoxColumn();
            IP = new DataGridViewTextBoxColumn();
            Port = new DataGridViewTextBoxColumn();
            ServerAddr = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToOrderColumns = true;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { deviceName, IP, Port, ServerAddr });
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new Size(344, 450);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            dataGridView1.Click += dataGridView1_Click;
            // 
            // deviceName
            // 
            deviceName.DataPropertyName = "deviceName";
            deviceName.HeaderText = "设备名称";
            deviceName.Name = "deviceName";
            // 
            // IP
            // 
            IP.DataPropertyName = "IP";
            IP.HeaderText = "IP";
            IP.Name = "IP";
            // 
            // Port
            // 
            Port.DataPropertyName = "Port";
            Port.HeaderText = "Port";
            Port.Name = "Port";
            // 
            // ServerAddr
            // 
            ServerAddr.DataPropertyName = "ServerAddr";
            ServerAddr.HeaderText = "工控机";
            ServerAddr.Name = "ServerAddr";
            // 
            // DeviceInfoForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(344, 450);
            Controls.Add(dataGridView1);
            Name = "DeviceInfoForm";
            Text = "DeviceInfoForm";
            Load += DeviceInfoForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn 设备名称;
        private DataGridViewTextBoxColumn 工单;
        private DataGridViewTextBoxColumn deviceName;
        private DataGridViewTextBoxColumn IP;
        private DataGridViewTextBoxColumn Port;
        private DataGridViewTextBoxColumn ServerAddr;
    }
}