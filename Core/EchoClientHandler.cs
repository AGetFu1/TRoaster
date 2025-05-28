using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRoaster.Core
{
    public class EchoClientHandler: SimpleChannelInboundHandler<IByteBuffer>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            if (msg != null)
            {
                Console.WriteLine("Receive From Server:" + msg.ToString(Encoding.UTF8));
            }
            ctx.WriteAsync(Unpooled.CopiedBuffer(msg));
        }
        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }
        public override void ChannelActive(IChannelHandlerContext context)
        {
            Console.WriteLine("发送Netty Data");
            context.WriteAndFlushAsync(Unpooled.CopiedBuffer(Encoding.UTF8.GetBytes("Hello World!")));
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine(exception);
            context.CloseAsync();
        }
    }
}