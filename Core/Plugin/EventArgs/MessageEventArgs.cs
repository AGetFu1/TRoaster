using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;

namespace TRoaster.Core.Plugin.EventArgs
{
    public class MessageEventArgs:PluginEventArgs
    {
        public String Message { get; set; }
    }
}
