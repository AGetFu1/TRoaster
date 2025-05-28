using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Core
{
    internal static class DataParser
    {
        public static string ParserCommondName(string message) {
            string commondName = "";
            //心跳：OV-379,heart,,workorder,;
            //烘烤工单：OV-379,heart,,workorder,WA020G30102 - 02;
            //开始录入工单：OV-379,heart,workorderstart,workorder,;
            //录入工单完成：OV-379,heart,workorderend,workorder,;
            //删除当前工单：OV-379,heart,workorderdelete,workorder,WA020G30102 - 02;
            //删除全部工单：OV-379,heart,workorderdeleteall,workorder,;
            //烘烤开始：OV-379,heart,bakestart,workorder,;
            //烘烤结束：OV-379,heart,bakeend,workorder,;
            //实时温度：OV-379,heart,125.2,workorder,;
            if (!string.IsNullOrEmpty(message)) {
                if (!message.EndsWith(';'))
                {
                    Log4netHelper.Info("没有结束符，数据不完整");
                }
                else {
                   string[] datas =  message.Split(',');
                    if (datas.Length > 0) {
                        commondName = datas[2];
                    }
                }
            }
            return commondName;
        }
        public static bool IsMatch(string message, OperationCommands commonName)
        {
            int commondInt;
            if (!string.IsNullOrEmpty(message))
            {
                String mess = message.Replace("\r\n", "").Replace("\n", "");
                String[] datas = mess.Split(",");
                if (!mess.Contains(';'))
                {
                    Log4netHelper.Info("没有结束符，数据不完整");
                    return false;
                }

                if (datas.Length >= 4) 
                {
                    int.TryParse(datas[0], out commondInt);
                    if ((OperationCommands)commondInt == commonName) {
                        return true;
                    } 
                }
                 
            }
            return false;
        }
        public static string GetData(string message)
        {
            
            if (!string.IsNullOrEmpty(message))
            {
                String mess = message.Replace("\r\n", "").Replace("\n", "").Replace(";","");
                string[] datas = mess.Split(",");
                if (datas.Length >= 4) {
                    return datas[3];
                } 
            }
            return "";
        }
        public static string GetDeviceName(string message)
        {

            if (!string.IsNullOrEmpty(message))
            {
                String mess = message.Replace("\r\n", "").Replace("\n", "");
                string[] datas = mess.Split(",");
                if (datas.Length >= 4)
                {
                    return datas[1];
                }
            }
            return "";
        }
    }
}
