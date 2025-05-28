using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRoaster.Core;
using TRoaster.Entity;
using TRoaster.Helper;
using TRoaster.Service;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Schedule.Job
{
    internal class LinkCheckJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                IScheduler scheduler = context.Scheduler;
                JobDataMap dataMap = context.JobDetail.JobDataMap;
               
                //1、断开连接
                String deviceName = dataMap.GetString("deviceName");
                bool isConn = GlobleCache.ConnectionDic[deviceName];
                if (!isConn) {
                    RoasterClient client = GlobleCache.deviceClients[deviceName];
                    DeviceModel deviceModel = GlobleCache.deviceInfos[deviceName];
                    client.Close();
                    //2、重连
                    if (deviceModel != null)
                    {
                        client.Connect(deviceModel.IP, int.Parse(deviceModel.Port));
                    }
                    if (!client.tcpClient.Online)
                    {
                        Log4netHelper.Info(deviceName + " 连接失败！");
                    }
                    else
                    {
                        Log4netHelper.Info(deviceName + " 连接成功！");
                    }
                }
                
                
                //JobKey jobKey = GlobleCache.jobkeys[deviceName + "_LinkCheck"];
                //TriggerKey triggerKey = GlobleCache.triggerkeys[deviceName + "_LinkCheck"];
                ////移除定时
                //scheduler.PauseJob(jobKey);//停止触发器  
                //scheduler.UnscheduleJob(triggerKey);//移除触发器  
                //scheduler.DeleteJob(jobKey);//删除任务  
            });
        }
    }
}
