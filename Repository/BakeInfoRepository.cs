using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TRoaster.Core;
using TRoaster.Entity;
using TRoaster.Helper;
using static TRoaster.Log.LogHelper;

namespace TRoaster.Repository
{
    public class BakeInfoRepository
    {
        public int InsertInfo(BakeInfoEntity bakeInfoEntity) {
            try
            {
                SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();
                BakeInfoEntity bakeInfo = new BakeInfoEntity();
                bakeInfo.LOT_ID = bakeInfoEntity.LOT_ID;
                bakeInfo.BAKE_BEGIN_TIME = bakeInfoEntity.BAKE_BEGIN_TIME;
                bakeInfo.BAKE_END_TIME = bakeInfoEntity.BAKE_END_TIME;
                bakeInfo.DEVICE_ID = bakeInfoEntity.DEVICE_ID;
                bakeInfo.PROGROM_NAME = bakeInfo.PROGROM_NAME;
                bakeInfo.REMARK = bakeInfoEntity.REMARK;

                return sqlClient.Insertable(bakeInfo).ExecuteCommand();
            }
            catch (Exception ex)
            {

                Log4netHelper.Info("保存烘箱数据出现异常："+ex.Message);
                return 0;
            }
        }
        //db.Fastest<Student>().BulkCopy(students);
        public int InsertAllInfo(List<BakeInfoEntity> bakeInfoEntitys)
        {
            try
            {
                SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();

                return sqlClient.Fastest<BakeInfoEntity>().BulkCopy(bakeInfoEntitys);
            }
            catch (Exception ex)
            {
                ClientManager.AppendText("保存烘箱数据出现异常：" + ex.Message);
                Log4netHelper.Info("保存烘箱数据出现异常：" + ex.Message);
                return 0;
            }
        }
        public List<BakeInfoEntity> GetAllLotInfo(String deviceName,String programName,List<String> lots) {
            try
            {
                SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();

                return sqlClient.Queryable<BakeInfoEntity>().Where(a => a.DEVICE_ID == deviceName && a.PROGROM_NAME == programName && lots.Contains(a.LOT_ID) && a.BANK_INFO_FLAG =="0").ToList();
            }
            catch (Exception ex)
            {
                ClientManager.AppendText("查询烘箱数据出现异常：" + ex.Message);
                Log4netHelper.Info("查询烘箱数据出现异常：" + ex.Message);
                return null;
            }
        }
        public int UpdateFinishedDateTime(List<BakeInfoEntity> bakeInfoEntitys, string deviceName, string programName, List<String> works) {
            try
            {
                List<int> ids = new List<int>();
                if (bakeInfoEntitys != null) {
                    foreach (var item in bakeInfoEntitys)
                    {
                        ids.Add(item.Id);
                    }
                    bakeInfoEntitys.ForEach(bakeInfo => {
                        bakeInfo.BANK_INFO_FLAG = "1";
                    });
                }
                SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();
                return sqlClient.Updateable(bakeInfoEntitys).UpdateColumns(it => new { it.BAKE_END_TIME,it.BANK_INFO_FLAG }).Where(a => a.DEVICE_ID == deviceName && a.PROGROM_NAME == programName && works.Contains(a.LOT_ID) &&  ids.Contains(a.Id)).ExecuteCommand();
            }
            catch (Exception ex)
            {
                ClientManager.AppendText("更新烘箱烘烤结束时间数据出现异常：" + ex.Message);
                Log4netHelper.Info("更新烘箱烘烤结束时间数据出现异常：" + ex.Message);
                return 0;
            }
        }
        public int UpdateConstTempBeginTime(List<BakeInfoEntity> bakeInfoEntitys,string deviceName,string programName,List<String> works)
        {
            try
            {
                List<int> ids = new List<int>();
                if (bakeInfoEntitys != null)
                {

                    foreach (var item in bakeInfoEntitys)
                    { 
                        ids.Add(item.Id);
                    }
                }
                SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();

                return sqlClient.Updateable(bakeInfoEntitys).UpdateColumns(it => new { it.CONST_TEMP_BEGIN_TIME }).Where(a=>a.DEVICE_ID==deviceName && a.PROGROM_NAME == programName && works.Contains(a.LOT_ID) && ids.Contains(a.Id) && a.BANK_INFO_FLAG=="0").ExecuteCommand();
            }
            catch (Exception ex)
            {
                ClientManager.AppendText("更新烘箱常温开始时间数据出现异常：" + ex.Message);
                Log4netHelper.Info("更新烘箱常温开始时间数据出现异常：" + ex.Message);
                return 0;
            }
        }
        public int UpdateConstTempEndTime(List<BakeInfoEntity> bakeInfoEntitys, string deviceName, string programName, List<String> works)
        {
            try
            {
                List<int> ids = new List<int>();
                if (bakeInfoEntitys != null)
                {

                    foreach (var item in bakeInfoEntitys)
                    {
                        ids.Add(item.Id);
                    }
                }
                SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();

                return sqlClient.Updateable(bakeInfoEntitys).UpdateColumns(it => new { it.CONST_TEMP_END_TIME }).Where(a => a.DEVICE_ID == deviceName && a.PROGROM_NAME == programName && works.Contains(a.LOT_ID) && ids.Contains(a.Id) && a.BANK_INFO_FLAG == "0").ExecuteCommand();
            }
            catch (Exception ex)
            {
                ClientManager.AppendText("更新烘箱常温结束时间数据出现异常：" + ex.Message);
                Log4netHelper.Info("更新烘箱常温结束时间数据出现异常：" + ex.Message);
                return 0;
            }
        }
        internal int UpdateReason(List<BakeInfoEntity> bakeInfos, string deviceName, string programName, List<string> workCards)
        {
            try
            {
                List<int> ids = new List<int>();
                if (bakeInfos != null)
                {

                    foreach (var item in bakeInfos)
                    {
                        ids.Add(item.Id);
                    }
                }
                SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();

                return sqlClient.Updateable(bakeInfos).UpdateColumns(it => new { it.REMARK }).Where(a => a.DEVICE_ID == deviceName && a.PROGROM_NAME == programName && workCards.Contains(a.LOT_ID) && ids.Contains(a.Id) && a.BANK_INFO_FLAG == "0").ExecuteCommand();
            }
            catch (Exception ex)
            {
                ClientManager.AppendText("更新烘箱常温结束时间数据出现异常：" + ex.Message);
                Log4netHelper.Info("更新烘箱常温结束时间数据出现异常：" + ex.Message);
                return 0;
            }
        }
        internal int UpdateTempFlag(List<RealTemperatureEntity> tempDatas, string deviceName, string programName)
        {
            try
            {
                List<int> ids = new List<int>();
                 
                SqlSugarClient sqlClient = RTMDBConnection.GetDBConnection();

                return sqlClient.Updateable(tempDatas).UpdateColumns(it => new { it.TOASTER_TEMPERATURE_FLAG }).ExecuteCommand();
            }
            catch (Exception ex)
            {
                ClientManager.AppendText("更新烘箱温度Flag数据出现异常：" + ex.Message);
                Log4netHelper.Info("更新烘箱温度Flag数据出现异常：" + ex.Message);
                return 0;
            }
        }
    }
}
