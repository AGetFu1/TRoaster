using log4net.Config;
using log4net;
using static TRoaster.Log.LogHelper;

namespace TRoaster
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Log4netHelper.Repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(Log4netHelper.Repository, new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + "/log4net.config"));
            Application.Run(new Form1());
        }
    }
}