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
    public class WorkorderDeleteAllPlugin : PluginBase, ITcpReceivedPlugin<TcpClient>
    {

        public async Task OnTcpReceived(TcpClient client, ReceivedDataEventArgs e)
        {
            ByteBlock byteBlock = e.ByteBlock;

            string msg = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);

            bool isMatch = DataParser.IsMatch(msg, OperationCommands.DeleteAllWorkOrders);
            if (isMatch)
            {
                string deviceName = DataParser.GetDeviceName(msg);
                //回复
                ReplyMessage(client, deviceName);
                //从当前批次信息中删除所有工单
                Log4netHelper.Info("从当前批次信息中删除所有工单");
                Task.Run(() => {
                    LotManager lotManager =  new LotManager();
                    lotManager.RemoveLotInfo(deviceName);
                    lotManager.RemoveRecipeList(deviceName);
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
                Log4netHelper.Info("删除所有工单消息回复：" + sendMsg);
            }
            catch (Exception ex)
            {
                Log4netHelper.Info("删除所有工单消息回复数据出错:" + ex.Message);
            }
        }
    }
}
