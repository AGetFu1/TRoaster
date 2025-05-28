using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;
using TRoaster.Core.Plugin.EventArgs;
using TRoaster.Entity;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Core.Plugin
{
    ////心跳：1,OV-379,,;
    public class HeartPlugin : PluginBase, ITcpReceivedPlugin<TcpClient>
    {
        
        public async Task OnTcpReceived(TcpClient client, ReceivedDataEventArgs e)
        {
            ByteBlock byteBlock = e.ByteBlock;
             
            string msg = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);

            bool isMatch = DataParser.IsMatch(msg, OperationCommands.Heartbeat);
            if (isMatch)
            {
                
                string deviceName = DataParser.GetDeviceName(msg);
                Log4netHelper.Info("收到心跳包："+ deviceName);
                
                //保活
                //心跳：OV - 379,,;
                string sendMsg = deviceName + ",,;";
                client.Send(sendMsg);
                Log4netHelper.Info("心跳消息回复：" + sendMsg);
                e.Handled = true;
            }
            await e.InvokeNext(); 
        }
    }
}
