
using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TRoaster.Core;
using TRoaster.Entity;
using TRoaster.Helper;
using TRoaster.Interface;

namespace TRoaster.Schedule.Job
{
    internal class RealTemperatureJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                JobDataMap dataMap = context.JobDetail.JobDataMap;

                ModBusTcpClient client = (ModBusTcpClient)dataMap.Get("client");
                TextBox textBox = (TextBox)dataMap.Get("messageBox");
                int[] tempData = client?.RealTemperature();
                String temperatureData = "";
                if (tempData != null)
                {
                    temperatureData = tempData[0].ToString().Trim();
                    //存库
                    Task.Run(() => {
                        SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();
                        RealTemperatureEntity temperatureEntity = new RealTemperatureEntity();
                        temperatureEntity.DEVICE_NAME = "TH-111";
                        temperatureEntity.CREATE_DATE = DateTime.Now;
                        temperatureEntity.TEMPERATURE = tempData[0].ToString();
                        //temperatureEntity.PROGRAM_NAME = "T123432452-10";

                        sqlClient.Insertable(temperatureEntity).ExecuteCommand();
                    }); 
                }
                textBox.Invoke(new Action(() =>
                {
                    textBox.AppendText("当前温度：" + temperatureData + "\r\n");
                }));
            });

        }
    }
}
