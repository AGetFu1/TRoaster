using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;
using TRoaster.Log;
using static TRoaster.Log.LogHelper;
using System.ServiceModel.Channels;
using IChannel = DotNetty.Transport.Channels.IChannel;

namespace TRoaster.Core
{
    public class RunClient
    {
        public async Task RunClientAsync()
        {
            var group = new MultithreadEventLoopGroup();
            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        pipeline.AddLast(new EchoClientHandler());
                    }));
                
                IChannel clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500));
                if (clientChannel.Open)
                {
                    Log4netHelper.Info($"与 {clientChannel.RemoteAddress} 建立连接");
                    
                    _ = clientChannel.CloseCompletion.ContinueWith((t, s) =>
                    {
                        Log4netHelper.Info($"与 {clientChannel.RemoteAddress} 断开连接");
                        
                    }, this, TaskContinuationOptions.ExecuteSynchronously);
                }
                else
                {
                    Log4netHelper.Info($"clientChannel not open, retry after 5 s");
                 
                    //scheduleReconnect();//auto reconnect when connect failed.
                }

                Console.ReadLine();
                await clientChannel.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                await group.ShutdownGracefullyAsync();
            }
        }
    }
}
