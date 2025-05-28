using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;
using TRoaster.Core.Plugin.EventArgs;
using TRoaster.Service;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Core.Plugin
{
    public class WorkorderDeletePlugin : PluginBase, ITcpReceivedPlugin<TcpClient>
    {

        public async Task OnTcpReceived(TcpClient client, ReceivedDataEventArgs e)
        {
            ByteBlock byteBlock = e.ByteBlock;

            string msg = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);

            bool isMatch = DataParser.IsMatch(msg, OperationCommands.DeleteCurrentWorkOrder);
            if (isMatch)
            {
                string deviceName = DataParser.GetDeviceName(msg);
                string workCard = DataParser.GetData(msg);
                Log4netHelper.Info("从当前批次信息中删除当前工单");
                //回复
                ReplyMessage(client, deviceName);
                //从当前批次信息中删除当前工单
                Task.Run(() => {
                    LotManager lotManager = new LotManager();
                    lotManager.RemoveCurrentWorkCard(deviceName, workCard);
                });
                e.Handled = true;
            }
            await e.InvokeNext();
        }
        private void ReplyMessage(TcpClient client, string deviceName)
        {
            try
            {
                string sendMsg = deviceName + ",,;";
                client.Send(sendMsg);
                Log4netHelper.Info("删除当前工单消息回复：" + sendMsg);
            }
            catch (Exception ex)
            {
                Log4netHelper.Info("删除当前工单回复数据出错:" + ex.Message);
            }
        }
    }
}
