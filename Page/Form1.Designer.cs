
using static TRoaster.Log.LogHelper;

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TRoaster.Core;

namespace TRoaster
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            splitContainer1 = new SplitContainer();
            groupBox1 = new GroupBox();
            panel1 = new Panel();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            updateDevice = new Button();
            deleteDevice = new Button();
            addDevice = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Bottom;
            textBox1.Location = new Point(0, 428);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(954, 313);
            textBox1.TabIndex = 2;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(textBox1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(groupBox1);
            splitContainer1.Size = new Size(1362, 741);
            splitContainer1.SplitterDistance = 954;
            splitContainer1.TabIndex = 3;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.Menu;
            groupBox1.Controls.Add(panel1);
            groupBox1.Controls.Add(updateDevice);
            groupBox1.Controls.Add(deleteDevice);
            groupBox1.Controls.Add(addDevice);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.FlatStyle = FlatStyle.Popup;
            groupBox1.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(404, 741);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "设备管理";
            // 
            // panel1
            // 
            panel1.Controls.Add(label8);
            panel1.Controls.Add(label7);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(222, 308);
            panel1.Name = "panel1";
            panel1.Size = new Size(176, 248);
            panel1.TabIndex = 3;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.BackColor = Color.LightSlateGray;
            label8.Location = new Point(93, 176);
            label8.Name = "label8";
            label8.Size = new Size(49, 19);
            label8.TabIndex = 7;
            label8.Text = "          ";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.Red;
            label7.Location = new Point(93, 124);
            label7.Name = "label7";
            label7.Size = new Size(49, 19);
            label7.TabIndex = 6;
            label7.Text = "          ";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.Yellow;
            label6.Location = new Point(93, 67);
            label6.Name = "label6";
            label6.Size = new Size(53, 19);
            label6.TabIndex = 5;
            label6.Text = "           ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.Lime;
            label5.Location = new Point(93, 23);
            label5.Name = "label5";
            label5.Size = new Size(45, 19);
            label5.TabIndex = 4;
            label5.Text = "         ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(27, 176);
            label4.Name = "label4";
            label4.Size = new Size(37, 19);
            label4.TabIndex = 3;
            label4.Text = "离线";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(27, 124);
            label3.Name = "label3";
            label3.Size = new Size(37, 19);
            label3.TabIndex = 2;
            label3.Text = "报警";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(27, 67);
            label2.Name = "label2";
            label2.Size = new Size(37, 19);
            label2.TabIndex = 1;
            label2.Text = "待机";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 23);
            label1.Name = "label1";
            label1.Size = new Size(37, 19);
            label1.TabIndex = 0;
            label1.Text = "运行";
            // 
            // updateDevice
            // 
            updateDevice.AutoSize = true;
            updateDevice.BackColor = SystemColors.InactiveBorder;
            updateDevice.BackgroundImageLayout = ImageLayout.Stretch;
            updateDevice.FlatStyle = FlatStyle.Popup;
            updateDevice.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            updateDevice.ForeColor = SystemColors.HotTrack;
            updateDevice.Location = new Point(164, 106);
            updateDevice.Name = "updateDevice";
            updateDevice.Size = new Size(98, 39);
            updateDevice.TabIndex = 2;
            updateDevice.Text = "修改设备";
            updateDevice.UseVisualStyleBackColor = false;
            updateDevice.Visible = false;
            updateDevice.Click += updateDevice_Click;
            // 
            // deleteDevice
            // 
            deleteDevice.AutoSize = true;
            deleteDevice.BackColor = SystemColors.InactiveBorder;
            deleteDevice.BackgroundImageLayout = ImageLayout.Stretch;
            deleteDevice.FlatStyle = FlatStyle.Popup;
            deleteDevice.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            deleteDevice.ForeColor = SystemColors.HotTrack;
            deleteDevice.Location = new Point(309, 106);
            deleteDevice.Name = "deleteDevice";
            deleteDevice.Size = new Size(98, 39);
            deleteDevice.TabIndex = 1;
            deleteDevice.Text = "删除设备";
            deleteDevice.UseVisualStyleBackColor = false;
            deleteDevice.Visible = false;
            deleteDevice.Click += deleteDevice_Click;
            // 
            // addDevice
            // 
            addDevice.AutoSize = true;
            addDevice.BackColor = SystemColors.InactiveBorder;
            addDevice.BackgroundImageLayout = ImageLayout.Stretch;
            addDevice.FlatStyle = FlatStyle.Popup;
            addDevice.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            addDevice.ForeColor = SystemColors.HotTrack;
            addDevice.Location = new Point(27, 106);
            addDevice.Name = "addDevice";
            addDevice.Size = new Size(98, 39);
            addDevice.TabIndex = 0;
            addDevice.Text = "添加设备";
            addDevice.UseVisualStyleBackColor = false;
            addDevice.Visible = false;
            addDevice.Click += addDevice_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1362, 741);
            Controls.Add(splitContainer1);
            Name = "Form1";
            Text = "烘箱";
            Load += Form1_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            RunClient nettyClient = new RunClient();
            nettyClient.RunClientAsync().Wait();
        }

        #endregion
        private TextBox textBox1;
        private SplitContainer splitContainer1;
        private GroupBox groupBox1;
        private Button addDevice;
        private Button deleteDevice;
        private Button updateDevice;
        private Panel panel1;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
    }
}
