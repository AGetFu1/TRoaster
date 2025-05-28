using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TRoaster.Core.Plugin.EventArgs;

namespace TRoaster.Core.Plugin
{
    public interface IRevicePlugin : IPlugin
    {
        Task Revice(object sender, MessageEventArgs e);
    }
}
