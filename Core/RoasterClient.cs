using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TouchSocket.Core;
using TouchSocket.Sockets;
using TRoaster.Core.Plugin;
using TRoaster.Core.Plugin.EventArgs;
using static System.Windows.Forms.Control;
using static TRoaster.Log.LogHelper;
namespace TRoaster.Core
{
    public class RoasterClient
    {
        //当前连接设备名称
        private String deviceName;
        /// <summary>
        /// 连接成功事件
        /// </summary>
        internal event Action<string, bool> OnConnect;
        /// <summary>
        /// 接收通知事件
        /// </summary>
        internal event Action<byte[]> OnReceive;
        /// <summary>
        /// 已送通知事件
        /// </summary>
        internal event Action<int> OnSend;
        /// <summary>
        /// 断开连接通知事件
        /// </summary>
        internal event Action<string, bool> OnClose;
        public TcpClient tcpClient;
        int BufferInt = 0;
        /// <summary>
        /// 是否连接服务器
        /// </summary>
        public bool Connected
        {
            get
            {
                if (tcpClient == null)
                {
                    return false;
                }
                return tcpClient.Online;
            }
        }
        /// <summary>
        /// 设置基本配置
        /// </summary>
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        internal RoasterClient(string deviceName, int receiveBufferSize)
        {
            this.deviceName = deviceName;
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ip">ip地址或域名</param>
        /// <param name="port">连接端口</param>
        internal async void Connect(string ip, int port)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connected += this.MessageEventHandler;//成功连接到服务器
                tcpClient.Disconnected += this.DisconnectEventHandler;//从服务器断开连接，当连接不成功时不会触发。
                //tcpClient.Received += this.ReceivedEventHandler;
                SetUp(ip, port);
                await tcpClient.ConnectAsync();
                //var waitingClient = this.tcpClient.CreateWaitingClient(new WaitingOptions());

                //var bytes = waitingClient.SendThenReturn("ghaa".ToUTF8Bytes());
                //if (bytes != null)
                //{
                //    MessageBox.Show($"收到等待数据：{Encoding.UTF8.GetString(bytes)}");
                //}
                //receiver可以复用，不需要每次接收都新建
                //Thread thread =  new Thread(test);

            }
            catch (Exception ex)
            {
                Log4netHelper.Info($"连接服务端失败{ex.Message}");
                tcpClient.Logger.Error($"连接服务端失败{ex.Message}");
                //log4net.LogManager.GetLogger(typeof(TcpClients)).Error($"连接服务端失败{ex.Message}");
            }
        }
        public void test()
        {
            using (var receiver = tcpClient.CreateReceiver())
            {
                while (true)
                {
                    //tcpClient.Send(Console.ReadLine());

                    //receiverResult必须释放
                    using (var receiverResult = receiver.ReadAsync(CancellationToken.None))
                    {
                        if (receiverResult.Result.IsClosed)
                        {
                            //断开连接了
                            //MessageLog.ShowMsg($"receiverResult.Result.IsClosed:{receiverResult.Result.IsClosed}", deviceName);
                        }

                        //从服务器收到信息。
                        var mes = Encoding.UTF8.GetString(receiverResult.Result.ByteBlock.Buffer, 0, receiverResult.Result.ByteBlock.Len);
                        tcpClient.Logger.Info($"客户端接收到信息：{mes}");
                        //MessageLog.ShowMsg($"客户端接收到信息：{mes}", deviceName);
                        //如果是适配器信息，则可以直接获取receiverResult.RequestInfo;

                    }
                }
            }
        }
        private void SetUp(string ip, int port)
        {
            //声明配置
            TouchSocketConfig config = new TouchSocketConfig();
            config.SetRemoteIPHost(new IPHost(ip + ":" + port));
            config.ConfigurePlugins(a =>
            {
                a.UseReconnection()
                .SetTick(TimeSpan.FromSeconds(60))
                .UsePolling();
                a.Add<ReceviePlugin>();
                a.Add<TempPlugin>();
                a.Add<BakeEndPlugin>();
                a.Add<BakeStartPlugin>();
                a.Add<WorkorderDeleteAllPlugin>();
                a.Add<WorkorderDeletePlugin>();
                a.Add<WorkorderEndPlugin>();
                a.Add<WorkorderStartPlugin>();
                a.Add<HeartPlugin>();
                a.Add<ReadWorkCardPlugin>();
                a.Add<ConstTempStartPlugin>();
                a.Add<ConstTempEndPlugin>();
                //a.UseCheckClear()
                //     .SetCheckClearType(CheckClearType.All)
                //     .SetTick(TimeSpan.FromSeconds(3))
                //     .SetOnClose((c, t) =>
                //     {
                //         c.TryShutdown();
                //         c.SafeClose("超时无数据");
                //     });
            });
            config.ConfigureContainer(a =>
            {
                //a.AddLogger(new FileLogger("eaplogs"));//添加Mylog4netLogger日志
            });
            KeepAliveValue keepAliveValue = new KeepAliveValue();
            keepAliveValue.AckInterval = 10 * 1000;
            keepAliveValue.Interval = 10*1000;
            config.SetKeepAliveValue(keepAliveValue);
            tcpClient.Setup(config);
        }
        private Task ReceivedEventHandler<ISocketClient>(ISocketClient client11, ReceivedDataEventArgs e)
        {

            //从服务器收到信息
            //ReceivedDataEventArgs
            BufferInt += e.ByteBlock.Len;

            //MessageLog.ShowMsg($"Len:{e.ByteBlock.Len},BufferInt:{BufferInt},e.ByteBlock.Capacity;{e.ByteBlock.Capacity}", deviceName);
            tcpClient.Logger.Info($"Len:{e.ByteBlock.Len},BufferInt:{BufferInt}");
            Log4netHelper.Info($"Len:{e.ByteBlock.Len},BufferInt:{BufferInt}");
            string mes = Encoding.UTF8.GetString(e.ByteBlock.Buffer, 0, e.ByteBlock.Len);
            //MessageLog.ShowMsg($"接收消息:{mes}", deviceName);
            tcpClient.Logger.Info($"{mes}");
            Log4netHelper.Info($"{mes}");
            //MessageLog.ShowMsg($"{mes}", deviceName);
            if (e.ByteBlock.Len > 0)
            {
                byte[] data = new byte[e.ByteBlock.Len];
                Buffer.BlockCopy(e.ByteBlock, 0, data, 0, e.ByteBlock.Len);
                 
                if (OnReceive != null)
                {
                    OnReceive(data);
                }
            }
            else
            {
                //MessageLog.ShowMsg($"接收消息长度为0", deviceName);
                Log4netHelper.Info($"接收消息长度为0");
                tcpClient.Logger.Info($"接收消息长度为0");
                //Close();
            }
            tcpClient.ClearReceiver();
            return EasyTask.CompletedTask;
        }
        public Task MessageEventHandler<ITcpClient>(ITcpClient client, ConnectedEventArgs e)
        {
            //tcpClient.SetDataHandlingAdapter(new TerminatorPackageAdapter("/r/n"));
            //public delegate Task ConnectedEventHandler<TClient>(TClient client, ConnectedEventArgs e);
            Log4netHelper.Info("连接成功");
            Log4netHelper.Info("本地连接端口："+ tcpClient.MainSocket.LocalEndPoint.GetPort());
            DeviceLabel deviceLabel = GlobleCache.deviceLableInfos[deviceName];
            GlobleCache.ConnectionDic[deviceName] = true;
            if (deviceLabel != null)
            {
                deviceLabel.DeviceStatus = "working";
            }
            if (OnConnect != null)
            {
                OnConnect(deviceName, true);
            }
            
            return EasyTask.CompletedTask;
        }
        private Task DisconnectEventHandler<ITcpClient>(ITcpClient client, DisconnectEventArgs e)
        {
            //Task DisconnectEventHandler<TClient>(TClient client, DisconnectEventArgs e)
            Log4netHelper.Info("连接断开");
            DeviceLabel deviceLabel = GlobleCache.deviceLableInfos[deviceName];
            GlobleCache.ConnectionDic[deviceName] = false;
            if (deviceLabel != null)
            {
                deviceLabel.DeviceStatus = "offline";
            }
            if (OnClose != null)
            {
                
                OnClose(deviceName, false);
            }
            return EasyTask.CompletedTask;
        }
        public void Send(byte[] buffer, int offset, int length)
        {
            try
            {
                tcpClient.Logger.Info(Encoding.UTF8.GetString(buffer));

                if (tcpClient.CanSend)
                {
                    tcpClient.Send(buffer, offset, length);
                }

            }
            catch (Exception ex)
            {
                 
            }

        }
        public void Close()
        {
            tcpClient.Close();
        }
    }
}
