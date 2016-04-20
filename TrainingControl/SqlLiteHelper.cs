using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using Newtonsoft.Json;

namespace TrainingControl
{
    public static class SqlLiteHelper
    {

        /// <summary>
        /// The lock object
        /// </summary>
        private readonly static object LockObject = new object();
        /// <summary>
        /// 数据库全路径
        /// </summary>
        private static readonly string ConnString = "Data Source= Backup.dat";

        /// <summary>
        /// 判断是否有该表，没有则创建
        /// </summary>
        /// <param name="tableName">表名</param>
        private static void CreatTabel(string tableName,string structureInfo)
        {
            try
            {
                ////判断表是否存在
                string sql = "SELECT COUNT(*) FROM sqlite_master where type='table' and name='" + tableName + "'";
                using (SQLiteConnection conn = new SQLiteConnection(ConnString))
                {
                    conn.Open();
                    var cmd = new SQLiteCommand(sql, conn);
                    int recordCount = (int)(long)cmd.ExecuteScalar();   ////如果存在返回1，不存在返回0
                    if (recordCount == 0)
                    {
                        cmd.CommandText = structureInfo;
                        cmd.Connection = conn;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("检查表结构时出错：" + ex.Message);
            }
        }

        /// <summary>
        /// Creates the traninees table.
        /// </summary>
        private static void CreateTranineesTable()
        {
            SqlLiteHelper.CreatTabel("Traninees", "CREATE TABLE Traninees (sfzmhm VARCHAR(30) NOT NULL,currenttime VARCHAR( 10 ),licenseinfo TEXT);");
        }

        /// <summary>
        /// Creates the traning proc information.
        /// </summary>
        private static void CreateTraningProcInfo()
        {
            SqlLiteHelper.CreatTabel("TraningProcInfo", "CREATE TABLE TraningProcInfo (sfzmhm VARCHAR(30) NOT NULL,currenttime VARCHAR( 10 ),procinfo TEXT);");
        }

        /// <summary>
        /// Creates the charg proc information.
        /// </summary>
        private static void CreateChargProcInfo()
        {
            SqlLiteHelper.CreatTabel("ChargeProcInfo", "CREATE TABLE ChargeProcInfo ( sfzmhm VARCHAR( 30 )  NOT NULL,autoid VARCHAR( 10 ),starttime VARCHAR( 25 ),endtime  VARCHAR( 25 ),mode VARCHAR( 20 ),currentmileage NUMERIC( 20 ),currentminutes NUMERIC( 20 ),surplustimes INT );");
        }

        /// <summary>
        /// 创建所有需要用到的表
        /// </summary>
        public static void CreateAllTable()
        {
            CreateChargProcInfo();
            CreateTranineesTable();
            CreateTraningProcInfo();
        }

        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="cmdparams">params</param>
        /// <returns>影响行数</returns>
        private static int ExecuteNonQuery(string sql, IEnumerable<SQLiteParameter> cmdparams)
        {
            int count = 0;
            lock (LockObject)
            {
                using (var conn = new SQLiteConnection(ConnString))
                {
                    SQLiteTransaction mytrans = null;
                    conn.Open();
                    using (conn)
                    {
                        mytrans = conn.BeginTransaction();
                        var mycommand = new SQLiteCommand(sql, conn, mytrans);
                        try
                        {
                            mycommand.Parameters.AddRange(cmdparams.ToArray());
                            mycommand.CommandTimeout = 180;
                            count = mycommand.ExecuteNonQuery();
                            mytrans.Commit();
                        }
                        catch (Exception e)
                        {
                            mytrans.Rollback();
                            throw new ApplicationException(e.Message);
                        }
                        finally
                        {
                            mycommand.Dispose();
                        }
                    }
                }

            }
            return count;
        }

        /// <summary>
        /// Saves the charges information.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns></returns>
        public static int SaveChargesInfo(ChargesInfo info)
        {
            if (info != null)
            {
                List<SQLiteParameter> cmdparams = new List<SQLiteParameter>
                {
                    new SQLiteParameter("sfzmhm", info.PidNo),
                    new SQLiteParameter("autoid", info.AutoId),
                    new SQLiteParameter("starttime", info.StartTime),
                    new SQLiteParameter("endtime", info.EndTime),
                    new SQLiteParameter("mode", info.Mode),
                    new SQLiteParameter("currentmileage", info.CurrentMileage),
                    new SQLiteParameter("currentminutes", info.CurrentMinutes),
                    new SQLiteParameter("surplustimes", info.SurplusTimes)
                };
                return SqlLiteHelper.ExecuteNonQuery("insert into ChargeProcInfo (sfzmhm,autoid,starttime,endtime,mode,currentmileage,currentminutes,surplustimes) values (@sfzmhm,@autoid,@starttime,@endtime,@mode,@currentmileage,@currentminutes,@surplustimes)", cmdparams);
            }
            return 0;
        }

        /// <summary>
        /// Saves the traning information.
        /// </summary>
        /// <param name="license">The license.</param>
        /// <returns></returns>
        public static int SaveTraningInfo(TrainLicense license)
        {
            if (license != null)
            {
                List<SQLiteParameter> cmdparams = new List<SQLiteParameter>
                {
                    new SQLiteParameter("sfzmhm", license.PidNo),
                    new SQLiteParameter("currenttime", DateTime.Now.ToLocalTime().ToString()),
                    new SQLiteParameter("procinfo", JsonConvert.SerializeObject(license.TrainDetail))
                };
                return ExecuteNonQuery("insert into TraningProcInfo (sfzmhm,currenttime,procinfo) values (@sfzmhm,@currenttime,@procinfo)", cmdparams);
            }
            return 0;
        }

        /// <summary>
        /// Saves the traninees information.
        /// </summary>
        /// <param name="license">The license.</param>
        /// <returns></returns>
        public static int SaveTranineesInfo(TrainLicense license)
        {
            if (license != null)
            {
                List<SQLiteParameter> cmdparams = new List<SQLiteParameter>
                {
                    new SQLiteParameter("sfzmhm", license.PidNo),
                    new SQLiteParameter("currenttime", DateTime.Now.ToLocalTime().ToString()),
                    new SQLiteParameter("licenseinfo", JsonConvert.SerializeObject(license))
                };
                return SqlLiteHelper.ExecuteNonQuery("insert into Traninees (sfzmhm,currenttime,licenseinfo) values (@sfzmhm,@currenttime,@licenseinfo)", cmdparams);
            }
            return 0;
        }
    }
}
