using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl.Triggers;
using Quartz.Spi;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Sockets;
using System.Windows.Forms;
using TouchSocket.Sockets;
using TRoaster.Core;
using TRoaster.Entity;
using TRoaster.Helper;
using TRoaster.Interface;
using TRoaster.Repository;
using TRoaster.Schedule;
using TRoaster.Schedule.Job;
using TRoaster.Service;
using static TRoaster.Log.LogHelper;

namespace TRoaster
{
    public partial class Form1 : Form
    {
        IScheduler scheduler;
        public Form1()
        {
            InitializeComponent();
            //easyModbusManager.connectionPropertiesListChanged += new EasyModbusManager.ConnectionPropertiesListChanged(UpdateListBox);
            //easyModbusManager.valuesChanged += new EasyModbusManager.ValuesChanged(UpdateDataGridView);
        }

        private void UpdateDataGridView(object sender)
        {

        }
        private void UpdateListBox(object sender)
        {

        }
        private Point mPoint = new Point();
        private int positionX = 20;
        private int positionY = 20;
        private int panelWidth;
        const int buttonWidth = 150; // 每个DeviceButton的宽度  
        const int buttonHeight = 150; // 每个DeviceButton的高度  
        const int gapWidth = 20;     // 按钮之间的空隙
        internal BindingList<DeviceModel> deviceInfoLists = new BindingList<DeviceModel>();
         



        private void Form1_Load(object sender, EventArgs e)
        {
            GlobleCache.textBox = textBox1;
            InitializeLotInfo();
            GlobleCache.splitContainer = splitContainer1;
            //DeviceinfoRepository deviceinfoRepository = new DeviceinfoRepository();
            //List<DeviceInfoEntity> deviceInfoEntities = deviceinfoRepository.GetDeviceInfos();
            //ClientManager clientManager = new ClientManager();

            //foreach (var item in deviceInfoEntities)
            //{
            // AddDeviceLabel(item);

            //    //BindingJob(item);
            //    DeviceModel device = new DeviceModel();
            //    device.IP = item.IP;
            //    device.Port = item.PORT;
            //    device.ServerAddr = item.SERVICE_ADDR;
            //    device.deviceName = item.DEVICE_NAME;
            //    GlobleCache.deviceInfos[item.DEVICE_NAME] = device;
            //    RoasterClient client = new RoasterClient(item.DEVICE_NAME, 1024 * 5);
            //    client.Connect(item.IP, int.Parse(item.PORT));
            //    clientManager.AddDeviceClient(item.DEVICE_NAME, client);
            //}
        }

        private void InitializeLotInfo()
        {
            string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            INIFile iniFile = new INIFile(path + "lotinfo.ini");
            String deviceName = iniFile.IniReadValue("DeviceInfo", "name");
            String IP = iniFile.IniReadValue("DeviceInfo", "IP");
            String Port = iniFile.IniReadValue("DeviceInfo", "PORT");
            DeviceModel device = new DeviceModel();
            device.IP = IP;
            device.Port = Port;
            device.deviceName = deviceName;
            DeviceInfoEntity deviceInfo = new DeviceInfoEntity();
            deviceInfo.PORT = Port;
            deviceInfo.IP = IP;
            deviceInfo.DEVICE_NAME = deviceName;
            AddDeviceLabel(deviceInfo);
            GlobleCache.deviceInfos[deviceName] = device;
            RoasterClient client = new RoasterClient(deviceName, 1024 * 5);
            client.Connect(IP, int.Parse(Port));
            ClientManager clientManager = new ClientManager();
            clientManager.AddDeviceClient(deviceName, client);
            String DeviceName = iniFile.IniReadValue("Lot", "DeviceName");
            String WorkCards = iniFile.IniReadValue("Lot", "WorkCards");
            Log4netHelper.Info("WorkCards:" + WorkCards);
            String programName = iniFile.IniReadValue("Lot", "programName");
            Log4netHelper.Info("programName:"+ programName);
           
            String BakeBeginTime = iniFile.IniReadValue("Lot", "BakeBeginTime");
            String BakeEndTime = iniFile.IniReadValue("Lot", "BakeEndTime");
            String ConstTempBeginTime = iniFile.IniReadValue("Lot", "ConstTempBeginTime");
            String ConstTempEndTime = iniFile.IniReadValue("Lot", "ConstTempEndTime");
            String Result = iniFile.IniReadValue("Lot", "Result");
            
            //String stepCode = iniFile.IniReadValue("Lot", "stepCode");
            //String stepName = iniFile.IniReadValue("Lot", "stepName");
            String stage = iniFile.IniReadValue("Lot", "stage");
            LotInfoModel lotInfoModel = new LotInfoModel();
            lotInfoModel.programName = programName;
            lotInfoModel.DeviceName = deviceName;
            lotInfoModel.Result = Result;
            //lotInfoModel.stepcode = stepCode;
            //lotInfoModel.stepname = stepName;
            lotInfoModel.Stage = stage;
            if (!string.IsNullOrEmpty(BakeEndTime))
            {
                lotInfoModel.BakeEndTime = Convert.ToDateTime(BakeEndTime);
            }
            if (!string.IsNullOrEmpty(BakeBeginTime))
            {
                lotInfoModel.BakeBeginTime = Convert.ToDateTime(BakeBeginTime);
            }
            if (!string.IsNullOrEmpty(ConstTempBeginTime))
            {
                lotInfoModel.ConstTempBeginTime = Convert.ToDateTime(ConstTempBeginTime);
            }
            if (!string.IsNullOrEmpty(ConstTempEndTime))
            {
                lotInfoModel.ConstTempEndTime = Convert.ToDateTime(ConstTempEndTime);
            }
            List<CardModel> cardModels = new List<CardModel>();
            if (!string.IsNullOrEmpty(WorkCards))
            {
                if (WorkCards.Contains(";"))
                {
                    List<string> lotInfos = WorkCards.Split(';').ToList();
                    foreach (var item in lotInfos)
                    {
                        if (!string.IsNullOrEmpty(item)) {
                            CardModel cardModel = new CardModel();
                            string[] lotinfo = item.Split('&');
                            cardModel.WordCard = lotinfo[0];
                            cardModel.stepcode = lotinfo[1];
                            cardModel.stepname = lotinfo[2];
                            cardModels.Add(cardModel);
                        } 
                    }
                }
                else {
                    string[] lotinfo = WorkCards.Split('&');
                    CardModel cardModel = new CardModel(); 
                    cardModel.WordCard = lotinfo[0];
                    cardModel.stepcode = lotinfo[1];
                    cardModel.stepname = lotinfo[2];
                    cardModels.Add(cardModel);
                    
                } 
            }
            lotInfoModel.Cards = cardModels;
            GlobleCache.lotinfoDic[deviceName] = lotInfoModel;
            if (!string.IsNullOrEmpty(programName)) {
                AddRecipeList(DeviceName, programName);
            } 
            if (lotInfoModel != null && !string.IsNullOrEmpty(lotInfoModel.programName)) {
                SearchAndSetId(DeviceName, lotInfoModel);
            }
            StartLinkCheckJob(deviceName);
        }

        private void StartLinkCheckJob(string deviceName) {
            Task.Run(async () => {
                JobFactory jobFactory = new JobFactory();
                scheduler = ScheduleFactory.GetScheduler();
                await scheduler.Start();

                Dictionary<string, Object> messagedic1 = new Dictionary<string, Object>();
                //messagedic1["messageBox"] = textBox1;
                messagedic1["deviceName"] = deviceName;

                JobKey lotjobKey = new JobKey(deviceName + "LinkCheckjob", deviceName + "LinkCheckjob");
                TriggerKey lottriggerKey = new TriggerKey(deviceName + "LinkChecktrriger", deviceName + "LinkCheckgroup");
                GlobleCache.jobkeys.Add(deviceName + "_LinkCheck", lotjobKey);
                GlobleCache.triggerkeys.Add(deviceName + "_LinkCheck", lottriggerKey);
                IJobDetail statusjob = jobFactory.CreateJob<LinkCheckJob>(lotjobKey.Name, lotjobKey.Group, messagedic1);
                ITrigger statustrriger = jobFactory.CreateTrigger(lottriggerKey.Name, lottriggerKey.Group, 60*2);
                scheduler.ScheduleJob(statusjob, statustrriger).Wait();
            });
                
        }
        private void SearchAndSetId(string deviceName, LotInfoModel lotInfoModel)
        {
            List<CardModel> cards = new List<CardModel>();
            //Search Id from DB
            List<String> works = new List<string>();
            lotInfoModel.Cards.ForEach(f => {
                works.Add(f.WordCard);
            });
            BakeInfoRepository bakeInfoRepository = new BakeInfoRepository();
            List<BakeInfoEntity> bakeInfos = bakeInfoRepository.GetAllLotInfo(deviceName, lotInfoModel.programName, works);
            //Set Id to tempTB
            if (bakeInfos != null && bakeInfos.Count > 0)
            {
                bakeInfos.ForEach(x => {
                    CardModel cardModel = new CardModel();
                    cardModel.Id = x.Id;
                    cardModel.WordCard = x.LOT_ID;
                    cardModel.stepname = x.STEP_NAME;
                    cardModel.stepcode = x.STEP_CODE;
                    cards.Add(cardModel);
                });
                LotManager lotManager = new LotManager();
                lotManager.SetIds(deviceName, cards);
            }
        }
        private void AddRecipeList(string deviceName, string programName) {
          
            Task.Run(() => {
                RecipeInfoRepository recipeInfoRepository = new RecipeInfoRepository();
                LotManager lotManager = new LotManager();
                var repis = lotManager.GetRecipeList(deviceName);
                if (repis == null)
                {
                    List<RecipeModel> recipeModels = recipeInfoRepository.GetRecipeList(programName);

                    if (recipeModels != null && recipeModels.Count > 0)
                    {
                        lotManager.AddRecipes(deviceName, recipeModels);
                    }
                }
            });
        }
        private void ClientEventHandler(string eventName, string deviceName)
        {
            RoasterClient client = GlobleCache.deviceClients[deviceName];
            DeviceModel deviceModel = GlobleCache.deviceInfos[deviceName];
            //ModBusTcpClient tcpClient = new ModBusTcpClient(deviceModel.IP, int.Parse(deviceModel.Port), deviceName);
            //tcpClient.receiveAction += ReceiveData;
            switch (eventName)
            {
                case "连接": connect(client, deviceModel); break;
                case "断开": disconnect(client); break;
                //case "启用": enable(tcpClient); break;
                //case "停止": disable(tcpClient); break;
                //case "开门": Open(tcpClient); break;
                //case "关门": Close(tcpClient); break;
                case "状态": RealDeviceStatus(client, deviceName); break;
                case "工单": FetchWorkCard(deviceName); break;
                    //case "温度": RealTemperature(tcpClient); break;

            }
        }

        private void FetchWorkCard(string deviceName)
        {
            LotManager lotManager = new LotManager();
            LotInfoModel lotInfo = lotManager.GetLotInfo(deviceName);
            if (lotInfo != null && lotInfo.Cards != null && lotInfo.Cards.Count > 0)
            {
                lotInfo.Cards.ForEach(a =>
                {
                    Log4netHelper.Info("当前工单：" + a.WordCard+",工序名称："+a.stepname+",工序代码："+a.stepcode);
                });
            }
            else
            {
                Log4netHelper.Info("当前未加工");
            }

        }

        private void RealDeviceStatus(RoasterClient client, string deviceName)
        {
            bool isOnline = client.tcpClient.Online;
            if (isOnline)
            {
                Log4netHelper.Info("连接状态现在是OK的");
            }
            else
            {
                Log4netHelper.Info("当前未能进行与远端连接");
            }

        }

        private void connect(RoasterClient client, DeviceModel deviceModel)
        {
            if (!client.tcpClient.Online)
            {
                client.Close();
                client.Connect(deviceModel.IP, int.Parse(deviceModel.Port));
            }

        }

        private void disconnect(RoasterClient tcpClient)
        {
            tcpClient.Close();
        }

        private void ReceiveData(string obj)
        {
            textBox1.AppendText("接受到数据" + obj);
        }
        //private void FetchWorkCard(ModBusTcpClient tcpClient)
        //{
        //    string workname = tcpClient?.FetchWorkCard();
        //    textBox1.AppendText(workname.Trim());
        //}
        //private void RealTemperature(ModBusTcpClient tcpClient)
        //{
        //    int[] tem = tcpClient?.RealTemperature();
        //    if (tem != null)
        //    {
        //        var s = tem[0].ToString().Trim();
        //        textBox1.AppendText(s);
        //    }
        //}
        //private void RealDeviceStatus(ModBusTcpClient tcpClient,String deviceName)
        //{
        //    int status = tcpClient.RealDeviceStatus();
        //    //1--运行、2--待机、3--报警、4--烘烤完成
        //    if (status == 1)
        //    {
        //        GlobleCache.deviceLableInfos[deviceName].BackColor = Color.Green;
        //        textBox1.AppendText("运行");
        //    }
        //    else if (status == 2)
        //    {

        //        GlobleCache.deviceLableInfos[deviceName].BackColor = Color.Yellow;
        //        textBox1.AppendText("待机");
        //    }
        //    else if (status == 3)
        //    {
        //        GlobleCache.deviceLableInfos[deviceName].BackColor = Color.Red;
        //        textBox1.AppendText("报警");
        //    }
        //    else if (status == 4)
        //    {
        //        GlobleCache.deviceLableInfos[deviceName].BackColor= Color.DeepPink;

        //        textBox1.AppendText("烘烤完成");
        //    }
        //}

        private void addDevice_Click(object sender, EventArgs e)
        {
            //1、添加设备信息
            AddDeviceForm addDeviceForm = new AddDeviceForm();
            addDeviceForm.triggerSource = "1";
            addDeviceForm.DeviceInfoAction += AddDeviceLabel;
            addDeviceForm.Show();

            //2、添加页面

            //var country = IniUtil.ReadProfileValue(@"D:\test.ini",	//文件路径
            //         "SysEnv",	//对应ini文件的[zhangsan]
            //         "IP",		//对应[zhangsan]下的country
            //         "CHN");
            //IniUtil.WriteProfileValue(@"D:\烘箱\test.ini",
            //                     "SysEnv3 ",
            //                      "IP",
            //                      "127.0.0.1");
            //IniUtil.WriteProfileValue(@"D:\烘箱\test.ini",
            //         "SysEnv2 ",
            //          "IP2",
            //          "127.0.0.1");
            //MESInterface mES = new MESInterface();
            //var lots = mES.getlot();


        }
        private void BindingJob(DeviceInfoEntity deviceinfo)
        {
            String deviceName = deviceinfo.DEVICE_NAME;
            JobFactory jobFactory = new JobFactory();
            scheduler = ScheduleFactory.GetScheduler();
            scheduler.Start();

            Dictionary<string, Object> messagedic1 = new Dictionary<string, Object>();
            messagedic1["client"] = new ModBusTcpClient(deviceinfo.IP, int.Parse(deviceinfo.PORT), deviceName);
            messagedic1["messageBox"] = textBox1;
            messagedic1["deviceName"] = deviceName;
            JobKey statusjobKey = new JobKey(deviceName + "statusjob", deviceName + "statusgroup");
            TriggerKey statustriggerKey = new TriggerKey(deviceName + "statustrriger", deviceName + "statusgroup");
            GlobleCache.jobkeys.Add(deviceName + "_status", statusjobKey);
            GlobleCache.triggerkeys.Add(deviceName + "_status", statustriggerKey);
            IJobDetail statusjob = jobFactory.CreateJob<DeviceStatusJob>(statustriggerKey.Name, statustriggerKey.Group, messagedic1);
            ITrigger statustrriger = jobFactory.CreateTrigger(statustriggerKey.Name, statustriggerKey.Group, 10);
            scheduler.ScheduleJob(statusjob, statustrriger).Wait();

            Dictionary<string, Object> messagedic = new Dictionary<string, Object>();
            messagedic["client"] = new ModBusTcpClient(deviceinfo.IP, int.Parse(deviceinfo.PORT), deviceName);
            messagedic["messageBox"] = textBox1;
            JobKey temperjobKey = new JobKey(deviceName + "temperjob", deviceName + "tempergroup");
            TriggerKey tempertriggerKey = new TriggerKey(deviceName + "tempertrriger", deviceName + "tempergroup");
            GlobleCache.jobkeys.Add(deviceName + "_temper", temperjobKey);
            GlobleCache.triggerkeys.Add(deviceName + "_temper", tempertriggerKey);
            IJobDetail tempjob = jobFactory.CreateJob<RealTemperatureJob>(temperjobKey.Name, temperjobKey.Group, messagedic);
            ITrigger temperTrigger = jobFactory.CreateTrigger(tempertriggerKey.Name, tempertriggerKey.Group, 10);
            scheduler.ScheduleJob(tempjob, temperTrigger).Wait();
        }
        private void AddDeviceLabel(DeviceInfoEntity deviceinfo)
        {
            DeviceLabel deviceLabel = new DeviceLabel();
            deviceLabel.clickAction += ClientEventHandler;
            deviceLabel.BackColor = SystemColors.GradientActiveCaption;
            deviceLabel.BatchInfo = "";
            deviceLabel.DeviceName = deviceinfo.DEVICE_NAME;
            deviceLabel.IP = deviceinfo.IP;
            try
            {
                deviceLabel.Port = int.Parse(deviceinfo.PORT);
            }
            catch (Exception)
            {
                MessageBox.Show("端口为非法数字，请检查", "添加报错");
            }
            GetPosition();
            deviceLabel.Location = new Point(positionX, positionY);
            deviceLabel.Name = deviceinfo.DEVICE_NAME;
            deviceLabel.Size = new Size(150, 150);
            deviceLabel.TabIndex = 0;
            deviceLabel.Temperature = "温度：";
            deviceLabel.WorkCard = "工单：";
            deviceLabel.BatchInfo = "程序：";
            deviceLabel.toolTip1.SetToolTip(deviceLabel.lblWorkCard, "暂无作业");
            splitContainer1.Panel1.Controls.Add(deviceLabel);
            DeviceModel deviceModel = new DeviceModel();
            deviceModel.Port = deviceinfo.PORT;
            deviceModel.IP = deviceinfo.IP;
            deviceModel.deviceName = deviceinfo.DEVICE_NAME;
            deviceModel.ServerAddr = deviceinfo.SERVICE_ADDR;
            deviceInfoLists.Add(deviceModel);
            GlobleCache.deviceLableInfos[deviceinfo.DEVICE_NAME] = deviceLabel;
            positionX += buttonWidth + gapWidth;
        }
        public void GetPosition()
        {

            //int i =GetLabelNum(splitContainer1.Panel1.Controls);

            if (positionX + buttonWidth + gapWidth > splitContainer1.Panel1.Width)
            {
                positionX = 50; // 重置X坐标为下一行的起始位置  
                positionY += buttonHeight + gapWidth; // 更新Y坐标到下一行  
            }
        }

        public int GetLabelNum(Control.ControlCollection controls)
        {
            int count = 0;
            foreach (Control c in controls)
            {
                if (c is DeviceLabel)
                {
                    count++;
                }
                count += GetLabelNum(c.Controls); // 递归遍历子控件
            }
            return count;
        }
        private void updateDevice_Click(object sender, EventArgs e)
        {
            DeviceInfoForm deviceInfoForm = new DeviceInfoForm(scheduler);
            deviceInfoForm.deviceModels = deviceInfoLists;
            deviceInfoForm.operationBtn = 1;
            deviceInfoForm.Show();
        }

        private void deleteDevice_Click(object sender, EventArgs e)
        {
            DeviceInfoForm deviceInfoForm = new DeviceInfoForm(scheduler);
            deviceInfoForm.deleteDevice += deleteDeviceByName;
            deviceInfoForm.deviceModels = deviceInfoLists;
            deviceInfoForm.operationBtn = 2;
            deviceInfoForm.Show();
        }

        private void deleteDeviceByName(string obj)
        {
            splitContainer1.Panel1.Controls.RemoveByKey(obj);
        }
    }
}
