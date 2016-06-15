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
        private static readonly string ConnString = "Data Source=" + DateTime.Now.ToString("yyyy-MM-dd") + "-Backup.dat";

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
            SqlLiteHelper.CreatTabel("Traninees", "CREATE TABLE Traninees (pidNo VARCHAR(30) NOT NULL,currentTime VARCHAR( 10 ),licenseInfo TEXT);");
        }

        /// <summary>
        /// Creates the traning proc information.
        /// </summary>
        private static void CreateTraningProcInfo()
        {
            SqlLiteHelper.CreatTabel("TraningProcInfo", "CREATE TABLE TraningProcInfo (pidNo VARCHAR(30) NOT NULL,timeStamp VARCHAR( 10 ),code varchar(20),mode varchar(10),type varchar(20));");
        }

        /// <summary>
        /// Creates the charg proc information.
        /// </summary>
        private static void CreateChargeProcInfo()
        {
            SqlLiteHelper.CreatTabel("ChargeProcInfo", "CREATE TABLE ChargeProcInfo (pidNo VARCHAR( 30 ),autoId VARCHAR( 10 ),chargeMode VARCHAR(50),operationType VARCHAR( 100 ),operationTime VARCHAR( 30 ),currenBalance NUMERIC( 15, 5 ),currentTries INT,remainingTime NUMERIC( 15, 3 ));");
        }

        /// <summary>
        /// 创建所有需要用到的表
        /// </summary>
        public static void CreateAllTable()
        {
            CreateChargeProcInfo();
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
        public static int SaveChargesProcInfo(ChargesInfo info)
        {
            if (info != null)
            {
                List<SQLiteParameter> cmdparams = new List<SQLiteParameter>
                {
                    new SQLiteParameter("pidNo", info.PidNo),
                    new SQLiteParameter("autoId", info.AutoId),
                    new SQLiteParameter("chargeMode", info.ChargeMode),
                    new SQLiteParameter("operationType", info.OperationType),
                    new SQLiteParameter("operationTime", info.OperationTime),
                    new SQLiteParameter("currenBalance", info.CurrenBalance),
                    new SQLiteParameter("currentTries", info.CurrentTries),
                    new SQLiteParameter("remainingTime", info.RemainingTime),
                };
                return SqlLiteHelper.ExecuteNonQuery("insert into ChargeProcInfo (pidNo,autoid,chargeMode,operationType,operationTime,currenBalance,currentTries,remainingTime) values (@pidNo,@autoid,@chargeMode,@operationType,@operationTime,@currenBalance,@currentTries,@remainingTime)", cmdparams);
            }
            return 0;
        }

        /// <summary>
        /// Saves the traning information.
        /// </summary>
        /// <param name="license">The license.</param>
        /// <returns></returns>
        public static int SaveTraningProInfo(string pidNo,TrainProc proc)
        {
            if (proc != null)
            {
                List<SQLiteParameter> cmdparams = new List<SQLiteParameter>
                {
                    new SQLiteParameter("pidNo", pidNo),
                    new SQLiteParameter("timeStamp", proc.Timestamp),
                     new SQLiteParameter("code", proc.Code),
                      new SQLiteParameter("mode", proc.Mode),
                    new SQLiteParameter("type", proc.Type)
                };
                return ExecuteNonQuery("insert into TraningProcInfo (pidNo,timeStamp,code,mode,type) values (@pidNo,@timeStamp,@code,@mode,@type)", cmdparams);
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
                    new SQLiteParameter("pidNo", license.PidNo),
                    new SQLiteParameter("currentTime", DateTime.Now.ToLocalTime().ToString()),
                    new SQLiteParameter("licenseInfo", JsonConvert.SerializeObject(license))
                };
                return SqlLiteHelper.ExecuteNonQuery("insert into Traninees (pidNO,currentTime,licenseInfo) values (@pidNo,@currentTime,@licenseInfo)", cmdparams);
            }
            return 0;
        }

        //public static TrainingDetail GetTrainningDetail(TrainLicense license)
        //{
        //    TrainingDetail detail = new TrainingDetail();
        //    detail.Name = license.Name;
        //    detail.PidNo = license.PidNo;
        //    detail.StartTime = license.TrainDetail.TrainStartTs.ToString("");
        //    detail.EndTime = license.TrainDetail.TrainEndTs.ToString("");
        //    detail.TrainningTime = (license.TrainDetail.TrainEndTs - license.TrainDetail.TrainStartTs).TotalMinutes.ToString("{0:N2}");
        //    detail.Balance = license.AccountBalance;
        //    detail.State = license.CheckLicense() == LicenseState.Normal ? "正常" : "余额不足";
        //    List<TrainProc> proc=license.TrainDetail.TrainProcList;
        //    var tries = from item in proc
        //                where item.Code == "10000" && item.Type == "S"
        //                select item;
        //    var itemCount = from item in proc
        //             where item.Code != "10000" && item.Type == "S"
        //             select item;
        //    var deduckPoints = from item in proc
        //             where item.Type == "P"
        //             select item.Code;
        //    detail.TrainingTries = tries.Count();
        //    detail.TrainingItemCount = itemCount.Count();
        //    detail.Deduck = deduckPoints.ToList();
        //}
        
    }
}
