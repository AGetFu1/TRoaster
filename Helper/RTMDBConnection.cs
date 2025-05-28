using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRoaster.Helper
{
    internal static class RTMDBConnection
    {
        public static SqlSugarClient GetDBConnection()
        {
            SqlSugarClient sqlSugarClient = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = "Data Source = 192.168.99.165; Database = TMS; User ID = sa; Password = Sartm@123.com; Connect Timeout = 30;Pooling=true;Max Pool Size=1000;Min Pool Size=10;",
                //ConnectionString = "Data Source=192.168.99.158,1434;Initial Catalog=TMS;Persist Security Info=True;User ID=udba;Password=Ch5jAkMr",
                IsAutoCloseConnection = true
            });
            sqlSugarClient.Aop.OnLogExecuting = (sql, pars) =>
            {
                var s1 = sql;
            };

            return sqlSugarClient;
        }
        public static SqlSugarClient GetDBConnection1()
        {
            SqlSugarClient sqlSugarClient = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = "Data Source = 192.168.99.165; Database = TMS; User ID = sa; Password = Sartm@123.com; Connect Timeout = 30;Pooling=true;Max Pool Size=1000;Min Pool Size=10;",
                //ConnectionString = "Data Source=192.168.99.158,1434;Initial Catalog=TMS;Persist Security Info=True;User ID=udba;Password=Ch5jAkMr",
                IsAutoCloseConnection = true
            });
            sqlSugarClient.Aop.OnLogExecuting = (sql, pars) =>
            {
                var s1 = sql;
            };

            return sqlSugarClient;
        }
    }
}
