using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using DBAccessProc.Schema;

namespace DBAccessProc
{
  public class DBAccessHelper
    {
      public DBAccessProc.Schema.TrainManagementDataSet TraDataManageMent;
   
      public DBAccessProc.Schema.TrainManagementDataSet TraDataManage
      {
          get { return TraDataManageMent; }
      }
      private static string m_sConnString = DBAccessProc.Common.DBConnectionString;
       public static string CONN_STRING 
       {
           get { return m_sConnString; }
           set { m_sConnString = value;}
        }


        /// <summary>
        /// 保存学员注册信息
        /// </summary>
       public static void SaveTraStudInfo(string sPidNo, string sTraineName, byte[] sPhoto, string sSex, DateTime dBirthDt, string sFingerprint1, int sDrvSchoolId,string sPhoneNo,string sFillPerson,string dFillTime,string sTrainerNo,string sFingerprint2,string sLicenseType,string sIcCardNo,string sPidTypeCd,string sTrainingMode,string sTraCarNo,double fBalance,string sAddress,int userType)
       {

           string SP_NAME = "sp_InsertTraStudInfo";
           SqlConnection connection = null;
           SqlTransaction trans = null;
           string[] tableName = { "TRA_STUDINFO" };

           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[20];

               int idx = 0;    //0
               parms[idx] = new SqlParameter("@i_pid_no", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sPidNo;
               ++idx;  //1
               parms[idx] = new SqlParameter("@i_traine_name", SqlDbType.VarChar, 20);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sTraineName;
               ++idx;  //2
               parms[idx] = new SqlParameter("@i_photo", SqlDbType.Image);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sPhoto;
               ++idx;  //3
               parms[idx] = new SqlParameter("@i_sex", SqlDbType.VarChar, 2);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sSex;
               ++idx;  //4
               parms[idx] = new SqlParameter("@i_birthdt", SqlDbType.DateTime);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = dBirthDt;
               ++idx; //5
               parms[idx] = new SqlParameter("@i_fingerprint1", SqlDbType.VarChar, 2000);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sFingerprint1;
               ++idx;    //6
               parms[idx] = new SqlParameter("@i_drivingschoolid", SqlDbType.Int);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sDrvSchoolId;
               ++idx;  //7
               parms[idx] = new SqlParameter("@i_phoneno", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sPhoneNo;
               ++idx;  //8
               parms[idx] = new SqlParameter("@i_fillinperson", SqlDbType.Image);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sFillPerson;
               ++idx;  //9
               parms[idx] = new SqlParameter("@i_fillintime", SqlDbType.DateTime);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = dFillTime;
               ++idx;  //10
               parms[idx] = new SqlParameter("@i_trainer_id", SqlDbType.VarChar,20);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sTrainerNo;
               ++idx; //11
               parms[idx] = new SqlParameter("@i_fingerprint2", SqlDbType.VarChar, 2000);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sFingerprint2;
               ++idx; //12
               parms[idx] = new SqlParameter("@i_licensetypecd", SqlDbType.VarChar, 2);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sLicenseType;
               ++idx;    //13
               parms[idx] = new SqlParameter("@i_iccardno", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sIcCardNo;
               ++idx;  //14
               parms[idx] = new SqlParameter("@i_pidtypecd", SqlDbType.VarChar, 2);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sPidTypeCd;
               ++idx;  //15
               parms[idx] = new SqlParameter("@i_training_mode", SqlDbType.VarChar, 2);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sTrainingMode;
               ++idx;  //16
               parms[idx] = new SqlParameter("@i_tra_car_no", SqlDbType.VarChar,20);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sTraCarNo;
               ++idx;  //17
               parms[idx] = new SqlParameter("@i_balance", SqlDbType.Float);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = fBalance;
               ++idx;  //18
               parms[idx] = new SqlParameter("@i_homeaddress", SqlDbType.Float);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sAddress;
			   ++idx;  //19
			   parms[idx] = new SqlParameter("@i_usertype", SqlDbType.Int);
			   parms[idx].Direction = ParameterDirection.Input;
			   parms[idx].Value = userType;
          
               SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
               trans.Commit();
               connection.Close();          
                
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("Save TRA_StudInfo  Error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }
        
       }
      /// <summary>
      /// 保存指纹
      /// </summary>
      /// <param name="sFingerPrint"></param>
      /// <param name="sPidNo"></param>
       public static void SaveTraineeFingerPrint(string sFingerPrint,string sPidNo)
       {
           string SP_NAME = "sp_SaveTraineeFingerPrint";
           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[2];
               int idx = 0;
               parms[idx] = new SqlParameter("@i_fingerprint1", SqlDbType.Int);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sFingerPrint;
               idx++;
               parms[idx] = new SqlParameter("@i_pid_no", SqlDbType.Int);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sPidNo;

               SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
               trans.Commit();
               connection.Close();
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("保存指纹信息出错: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }
       }

       /// <summary>
       /// 按身份证查询注册学员信息
       /// </summary>
       public static void GetTraStudInfoByPidNo(string sPidNo, TrainManagementDataSet TraineeDataTable)
       {
           string  SP_NAME = "sp_GetTraStudInfoByPidNo";
           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               string[] tableName = { "TraineeDataTable" };

               SqlParameter[] parms = new SqlParameter[1];
               parms[0] = new SqlParameter("@i_pid_no", SqlDbType.VarChar, 30);
               parms[0].Direction = ParameterDirection.Input;
               parms[0].Value = sPidNo;

               SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, TraineeDataTable, tableName, parms);
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("Get TRA_STUDINFO record error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }
      
       }
       /// <summary>
       /// 按序号查询注册学员信息
       /// </summary>
       public static void GetTraStudInfoBySeqNo(double dSeqNo, TrainManagementDataSet TraineeDataTable)
       {
           string SP_NAME = "sp_GetTraStudInfoBySeqNo";
           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               string[] tableName = { "TraineeDataTable" };

               SqlParameter[] parms = new SqlParameter[1];
               parms[0] = new SqlParameter("@i_seq_no", SqlDbType.Decimal);
               parms[0].Direction = ParameterDirection.Input;
               parms[0].Value = dSeqNo;

               SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, TraineeDataTable, tableName, parms);
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("按序号查询注册学员信息错误: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }

       }
	   /// <summary>
	   /// 按条件查询注册学员信息
	   /// </summary>
	   public static void GetTraStudInfo(string sPidNo, string sTraineName, string sTrainerNo, string sLicenseType, int sDrvSchoolId, DateTime dtBegin, DateTime dtEnd, string isdispalayCk, TrainManagementDataSet TraineeDataTable)
	   {
		   string SP_NAME = "sp_GetTraStudInfo";
		   SqlConnection connection = null;
		   SqlTransaction trans = null;
		   try
		   {
			   connection = new SqlConnection(CONN_STRING);
			   connection.Open();
			   string[] tableName = { "TraineeDataTable" };

			   SqlParameter[] parms = new SqlParameter[8];
			   int idx = 0;
			   parms[idx] = new SqlParameter("@i_pid_no", SqlDbType.VarChar, 30);
			   parms[idx].Direction = ParameterDirection.Input;
			   parms[idx].Value = sPidNo;
			   ++idx;  //1
			   parms[idx] = new SqlParameter("@i_traine_name", SqlDbType.VarChar, 20);
			   parms[idx].Direction = ParameterDirection.Input;
			   parms[idx].Value = sTraineName;
			   ++idx;  //2
			   parms[idx] = new SqlParameter("@i_trainer_id", SqlDbType.VarChar, 20);
			   parms[idx].Direction = ParameterDirection.Input;
			   parms[idx].Value = sTrainerNo;
			   ++idx; //3
			   parms[idx] = new SqlParameter("@i_licensetypecd", SqlDbType.VarChar, 2);
			   parms[idx].Direction = ParameterDirection.Input;
			   parms[idx].Value = sLicenseType;
			   ++idx;    //4
			   parms[idx] = new SqlParameter("@i_drivingschoolid", SqlDbType.Int);
			   parms[idx].Direction = ParameterDirection.Input;
			   parms[idx].Value = sDrvSchoolId;
			   ++idx;  //5
			   parms[idx] = new SqlParameter("@i_begin_dt", SqlDbType.DateTime);
			   parms[idx].Direction = ParameterDirection.Input;
			   parms[idx].Value = dtBegin;
			   ++idx;  //6
			   parms[idx] = new SqlParameter("@i_end_dt", SqlDbType.DateTime);
			   parms[idx].Direction = ParameterDirection.Input;
			   parms[idx].Value = dtEnd;
			   ++idx;  //7
			   parms[idx] = new SqlParameter("@i_isdispalyck", SqlDbType.VarChar, 2);
			   parms[idx].Direction = ParameterDirection.Input;
			   parms[idx].Value = isdispalayCk;


			   SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, TraineeDataTable, tableName, parms);
		   }
		   catch (Exception ex)
		   {
			   if (trans != null)
				   trans.Rollback();
			   throw new ApplicationException("Get TRA_STUDINFO record error: " + ex.Message);
		   }
		   finally
		   {
			   if (connection != null && connection.State == ConnectionState.Open)
				   connection.Close();

			   if (connection != null)
				   connection.Dispose();
		   }
	   }




        /// <summary>
        /// 保存学员预约信息
        /// </summary>
       public static void SaveTraBookInfo(string examCd, string sPidNo, string sTraineName, double dThisBalance, string sBookPerson, string dBookTime, string sTrainSession, string sTrainerNo, string sLicenseType, string sBrand, string dFillTime, string sPidTypeCd, string sBookLicenseNm, string sTraCarNo, double fRechargeAmount, string sCheckStatus, string sisupd, double moneyAmount, string sTraMode, int nTriesAmount, float fMileageAmount, string sBillMode) 
         {
             string SP_NAME = "sp_InsertTra_Book_Info";
             SqlConnection connection = null;
             SqlTransaction trans = null;
             try
             {
                 connection = new SqlConnection(CONN_STRING);
                 connection.Open();
                 trans = connection.BeginTransaction();

                 SqlParameter[] parms = new SqlParameter[22];
                 int idx = 0;    //0
                 parms[idx] = new SqlParameter("@i_examCd", SqlDbType.VarChar, 30);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = examCd;
                 idx++;    //1
                 parms[idx] = new SqlParameter("@i_pid_no", SqlDbType.VarChar, 30);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = sPidNo;
                 ++idx;  //2
                 parms[idx] = new SqlParameter("@i_traine_name", SqlDbType.VarChar, 20);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = sTraineName;
                 ++idx;  //3
                 parms[idx] = new SqlParameter("@i_this_balance", SqlDbType.Float);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = dThisBalance;
                 ++idx;  //4
                 parms[idx] = new SqlParameter("@i_bookperson", SqlDbType.VarChar, 30);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = sBookPerson;
                 ++idx; //5
                 parms[idx] = new SqlParameter("@i_booktime", SqlDbType.VarChar, 100);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = dBookTime;
                 ++idx;    //6
                 parms[idx] = new SqlParameter("@i_trainsession", SqlDbType.VarChar, 200);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = sTrainSession;
                 ++idx;  //7
                 parms[idx] = new SqlParameter("@i_trainer_no", SqlDbType.VarChar, 20);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = sTrainerNo;
                 ++idx;  //8
                 parms[idx] = new SqlParameter("@i_licensetypecd", SqlDbType.VarChar, 2);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = sLicenseType;
                 ++idx;  //9
                 parms[idx] = new SqlParameter("@i_brand", SqlDbType.VarChar, 20);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = sBrand;
                 ++idx;  //10
                 parms[idx] = new SqlParameter("@i_filltime", SqlDbType.VarChar, 50);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = dFillTime;
                 ++idx; //11
                 parms[idx] = new SqlParameter("@i_pidtypecd", SqlDbType.VarChar, 2);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = sPidTypeCd;
                 ++idx;    //12
                 parms[idx] = new SqlParameter("@i_booklicensenm", SqlDbType.VarChar, 10);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = sBookLicenseNm;
                 ++idx;  //13
                 parms[idx] = new SqlParameter("@i_auto_id", SqlDbType.VarChar, 10);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = sTraCarNo;
                 ++idx;  //14
                 parms[idx] = new SqlParameter("@i_recharge_amount", SqlDbType.Float);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = fRechargeAmount;
                 ++idx;  //15
                 parms[idx] = new SqlParameter("@i_checkstatus", SqlDbType.VarChar, 2);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = sCheckStatus;
                 ++idx;  //16
                 parms[idx] = new SqlParameter("@i_isupd", SqlDbType.VarChar, 2);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = sisupd;
                 ++idx;  //17
                 parms[idx] = new SqlParameter("@i_Banlance", SqlDbType.Float);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value =  moneyAmount;
                 ++idx;  //18
                 parms[idx] = new SqlParameter("@i_training_mode", SqlDbType.VarChar, 20);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = sTraMode;
                 ++idx;  //19
                 parms[idx] = new SqlParameter("@i_tries_amount", SqlDbType.Int);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = nTriesAmount;
                 ++idx;  //20
                 parms[idx] = new SqlParameter("@i_mileage_amount", SqlDbType.Float);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = fMileageAmount;
                 ++idx;  //21
                 parms[idx] = new SqlParameter("@i_bill_mode", SqlDbType.VarChar, 20);
                 parms[idx].Direction = ParameterDirection.Input;
                 parms[idx].Value = sBillMode;
                 SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
                 trans.Commit();
                 connection.Close();

             }
             catch (Exception ex)
             {
                 if (trans != null)
                     trans.Rollback();

                 throw new ApplicationException("Save TRA_BOOK_INFO  Error: " + ex.Message);
             }
             finally
             {
                 if (connection != null && connection.State == ConnectionState.Open)
                     connection.Close();

                 if (connection != null)
                     connection.Dispose();
             }
         }


       /// <summary>
       /// 查询余额信息
       /// </summary>
       public static Double GetBalanceByPidNo(string sPidNo)
      {
          SqlConnection connection = null;
          SqlTransaction trans = null;
          try
          {
              connection = new SqlConnection(CONN_STRING);
              connection.Open();
              DataSet balance = new DataSet();
              string sqltext = "select  balance from  TRA_STUDINFO where pid_no='"+sPidNo+"'";
              SqlCommand cmd = new SqlCommand(sqltext,connection);
              SqlDataAdapter sda = new SqlDataAdapter(cmd);
              sda.Fill(balance,"BalanceTable");
              var dBalance = 0.00;
              if (balance.Tables["BalanceTable"].Rows.Count > 0)
              {
                   dBalance = double.Parse(balance.Tables["BalanceTable"].Rows[0]["BALANCE"].ToString());
               }
              return dBalance;
            
          }
          catch (Exception ex)
          {
              if (trans != null)
                  trans.Rollback();              
              throw new ApplicationException("Get Balance record error: " + ex.Message);
           
          }
          finally
          {
              if (connection != null && connection.State == ConnectionState.Open)
                  connection.Close();

              if (connection != null)
                  connection.Dispose();
             
          }
        

      }
       /// <summary>
       /// 更新余额信息
       /// </summary>
       public static void UpdBalanceByPidNo(string sPidNo,Double dBalance,int[] sBookSeqNo)
       {
           string sqltext = string.Empty;         
           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();               
               for (int i = 0; i < sBookSeqNo.Length; i++)
               {
				   sqltext = "update  TRA_STUDINFO  set balance=0，UNDER_BALANCE=0  where pid_no='" + sPidNo + "'" + "" + "update TRA_BOOK_INFO set CHECKSTATUS='SK' where SEQ_NO='" + sBookSeqNo[i] + "'";
                   SqlCommand cmd = new SqlCommand(sqltext, connection);
                   cmd.ExecuteNonQuery();   
               }              
               connection.Close();
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();
               throw new ApplicationException("UpdBalanceByPidNo error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }
       }
       
       /// <summary>
       /// 按条件查询预约学员信息
       /// </summary>
       public static void GetTraBookByRange(string sPidNo,string sName, DateTime strTime,DateTime endTime,string sRunningMode, TrainManagementDataSet BookTrainingDataTable)
       {
           string SP_NAME = "sp_GetTraBookByRange";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();

               string[] tableName = { "BookTrainingDataTable" };

               SqlParameter[] parms = new SqlParameter[5];
               int index = 0;
               parms[index] = new SqlParameter("@i_pid_no", SqlDbType.VarChar, 30);
               parms[index].Direction = ParameterDirection.Input;
               parms[index].Value = sPidNo;
               ++index;
               parms[index] = new SqlParameter("@i_name", SqlDbType.VarChar,30);
               parms[index].Direction = ParameterDirection.Input;
               parms[index].Value = sName;
               ++index;
               parms[index] = new SqlParameter("@i_strtime", SqlDbType.DateTime);
               parms[index].Direction = ParameterDirection.Input;
               parms[index].Value = strTime;
               ++index;
               parms[index] = new SqlParameter("@i_endtime", SqlDbType.DateTime);
               parms[index].Direction = ParameterDirection.Input;
               parms[index].Value = endTime;
               ++index;
               parms[index] = new SqlParameter("@i_running_mode", SqlDbType.VarChar, 30);
               parms[index].Direction = ParameterDirection.Input;
               parms[index].Value = sRunningMode;

               SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, BookTrainingDataTable, tableName, parms);

           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("Get TRA_BOOK_INFO record error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }

       }
       /// <summary>
       /// 查询今日预约信息
       /// </summary>
       public static void GetTraBookToday(string sPidNo, string sName, DateTime strTime, DateTime endTime, string sRunningMode, TrainManagementDataSet BookTrainingDataTable)
       {
           string SP_NAME = "sp_GetTraBookToday";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();

               string[] tableName = { "BookTrainingDataTable" };

               SqlParameter[] parms = new SqlParameter[5];
               int index = 0;
               parms[index] = new SqlParameter("@i_pid_no", SqlDbType.VarChar, 30);
               parms[index].Direction = ParameterDirection.Input;
               parms[index].Value = sPidNo;
               ++index;
               parms[index] = new SqlParameter("@i_name", SqlDbType.VarChar, 30);
               parms[index].Direction = ParameterDirection.Input;
               parms[index].Value = sName;
               ++index;
               parms[index] = new SqlParameter("@i_strtime", SqlDbType.DateTime);
               parms[index].Direction = ParameterDirection.Input;
               parms[index].Value = strTime;
               ++index;
               parms[index] = new SqlParameter("@i_endtime", SqlDbType.DateTime);
               parms[index].Direction = ParameterDirection.Input;
               parms[index].Value = endTime;
               ++index;
               parms[index] = new SqlParameter("@i_running_mode", SqlDbType.VarChar, 30);
               parms[index].Direction = ParameterDirection.Input;
               parms[index].Value = sRunningMode;

               SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, BookTrainingDataTable, tableName, parms);

           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("Get TRA_BOOK_INFO record error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }

       }
       /// <summary>
       /// 按序号查询预约学员信息
       /// </summary>
       public static void GetTraBookBySeqNo(double dSeqNo,string sRunningMode, TrainManagementDataSet BookTrainingDataTable)
       {
           string SP_NAME = "sp_GetTraBookBySeqNo";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();

               string[] tableName = { "BookTrainingDataTable" };

               SqlParameter[] parms = new SqlParameter[2];
               int index = 0;
               parms[index] = new SqlParameter("@i_seq_no", SqlDbType.Decimal);
               parms[index].Direction = ParameterDirection.Input;
               parms[index].Value = dSeqNo;
               ++index;
               parms[index] = new SqlParameter("@i_running_mode", SqlDbType.VarChar, 30);
               parms[index].Direction = ParameterDirection.Input;
               parms[index].Value = sRunningMode;
               SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, BookTrainingDataTable, tableName, parms);

           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("sp_GetTraBookBySeqNo error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }

       }
       /// <summary>
       /// 更新签到学员状态
       /// </summary>
	   public static int TraineeCheckIn(string sSeqNo, string sCheckStatus, string pidNo, string diskID,double balance)
       {
           string SP_NAME = "sp_UpdTraBookStatus";
           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[6];
               int Index = 0;
               parms[Index] = new SqlParameter("@i_seq_no", SqlDbType.VarChar, 30);
               parms[Index].Direction = ParameterDirection.Input;
               parms[Index].Value = sSeqNo;            
               ++Index;//1
               parms[Index] = new SqlParameter("@i_checkstatus", SqlDbType.VarChar, 2);
               parms[Index].Direction = ParameterDirection.Input;
               parms[Index].Value = sCheckStatus;
			   ++Index;//2
			   parms[Index] = new SqlParameter("@i_pidno", SqlDbType.VarChar, 30);
			   parms[Index].Direction = ParameterDirection.Input;
			   parms[Index].Value = pidNo;
			   ++Index;//3
			   parms[Index] = new SqlParameter("@i_diskID", SqlDbType.VarChar, 100);
			   parms[Index].Direction = ParameterDirection.Input;
			   parms[Index].Value = diskID;
			   ++Index;//4
			   parms[Index] = new SqlParameter("@i_balance", SqlDbType.Float);
			   parms[Index].Direction = ParameterDirection.Input;
			   parms[Index].Value = balance;
			   ++Index;//5
			   parms[Index] = new SqlParameter("@i_out", SqlDbType.Int);
			   parms[Index].Direction = ParameterDirection.Output;	
			    SqlHelper.ExecuteNonQuery(trans,CommandType.StoredProcedure,SP_NAME, parms);
				var result = parms[5].Value.ToString();       
				trans.Commit();				 
               connection.Close();
			   return int.Parse(result);
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();
               throw new ApplicationException("Upd TRA_BOOK_INFO record error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }
       }
       /// <summary>
       /// 取消预约，删除记录
       /// </summary>
       public static void DelTraBookInfo(string sSeqNo)
       {

           string SP_NAME = "sp_DelTraBookInfo";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[1];
               int Index = 0;
               parms[Index] = new SqlParameter("@i_seq_no", SqlDbType.VarChar, 30);
               parms[Index].Direction = ParameterDirection.Input;
               parms[Index].Value = sSeqNo;
               //++Index;//1
               //parms[Index] = new SqlParameter("@i_checkstatus", SqlDbType.VarChar, 2);
               //parms[Index].Direction = ParameterDirection.Input;
               //parms[Index].Value = sCheckStatus;

               SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
               trans.Commit();
               connection.Close();
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("Upd TRA_BOOK_INFO record error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }
       }
       /// <summary>
       /// 更新学员余额信息
       /// </summary>
       public static void rechargeAccount(string sSeqNo, double Balance, string sPidNo, string sRecharge_Per,string sReason)
       {

           string SP_NAME = "sp_UpdTraStudBalance";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[5];
               int Index = 0;
               parms[Index] = new SqlParameter("@i_seq_no", SqlDbType.VarChar, 30);
               parms[Index].Direction = ParameterDirection.Input;
               parms[Index].Value = sSeqNo;
               Index++;//1
               parms[Index] = new SqlParameter("@i_balance", SqlDbType.Float, 100);
               parms[Index].Direction = ParameterDirection.Input;
               parms[Index].Value = Balance;
               Index++;//2
               parms[Index] = new SqlParameter("@i_pid_no", SqlDbType.VarChar, 30);
               parms[Index].Direction = ParameterDirection.Input;
               parms[Index].Value = sPidNo;
               Index++;//3
               parms[Index] = new SqlParameter("@i_recharge_per", SqlDbType.VarChar, 30);
               parms[Index].Direction = ParameterDirection.Input;
               parms[Index].Value = sRecharge_Per;
			   Index++;//4			
               parms[Index] = new SqlParameter("@i_reason", SqlDbType.VarChar, 100);
               parms[Index].Direction = ParameterDirection.Input;
               parms[Index].Value = sReason;

               SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
               trans.Commit();
               connection.Close();
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("Upd TRA_BOOK_INFO record error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }
       }
       public static void InsRechargeRecord(double Balance, string sPidNo, string sRecharge_Per, string sReason)
       {

           string SP_NAME = "sp_insRechargeRecord";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[4];
               int Index = 0;
               parms[Index] = new SqlParameter("@i_balance", SqlDbType.Float, 100);
               parms[Index].Direction = ParameterDirection.Input;
               parms[Index].Value = Balance;
               Index++;//2
               parms[Index] = new SqlParameter("@i_pid_no", SqlDbType.VarChar, 30);
               parms[Index].Direction = ParameterDirection.Input;
               parms[Index].Value = sPidNo;
               Index++;//3
               parms[Index] = new SqlParameter("@i_recharge_per", SqlDbType.VarChar, 100);
               parms[Index].Direction = ParameterDirection.Input;
               parms[Index].Value = sRecharge_Per;
               Index++;//4
               parms[Index] = new SqlParameter("@i_reason", SqlDbType.VarChar, 100);
               parms[Index].Direction = ParameterDirection.Input;
               parms[Index].Value = sReason;

               SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
               trans.Commit();
               connection.Close();
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("sp_insRechargeRecord error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }
       }
       /// <summary>
       /// 更新学员本次训练费用
       /// </summary>
       public static void UpdTraBookTraBalance(string sSeqNo, double Balance)
       {
           string SP_NAME = "sp_UpdTraBookInfoTraBalance";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {

               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();
    

               SqlParameter[] parms = new SqlParameter[2];
               int Index = 0;
               parms[Index] = new SqlParameter("@i_seq_no", SqlDbType.VarChar, 30);
               parms[Index].Direction = ParameterDirection.Input;
               parms[Index].Value = sSeqNo;
               Index++;//1
               parms[Index] = new SqlParameter("@i_balance", SqlDbType.Float, 100);
               parms[Index].Direction = ParameterDirection.Input;
               parms[Index].Value = Balance;
               
               SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
               trans.Commit();
               connection.Close();
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("sp_UpdTraBookInfoTraBalance error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }
       }
        /// <summary>
        /// 保存驾校信息
        /// </summary>
       public static void SaveDrvSchool(int iDrvId, string sDsName, string sAddress, string sPhoneNo, string sDsShortName, string sContact, string isupd)
       {

           string SP_NAME = "sp_InsertDrvSchool";
           SqlConnection connection = null;
           SqlTransaction trans = null;
           string[] tableName = { "DRIVING_SCHOOL" };

           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[7];

               int idx = 0;    //0        
               parms[idx] = new SqlParameter("@i_drvid", SqlDbType.Int);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = iDrvId;
               ++idx;    //1    
               parms[idx] = new SqlParameter("@i_ds_name", SqlDbType.VarChar, 50);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sDsName;
               ++idx;  //2
               parms[idx] = new SqlParameter("@i_address", SqlDbType.VarChar, 300);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sAddress;
               ++idx;  //3
               parms[idx] = new SqlParameter("@i_phone_no", SqlDbType.VarChar, 50);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sPhoneNo;
               ++idx;  //4
               parms[idx] = new SqlParameter("@i_ds_short_name", SqlDbType.VarChar, 50);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sDsShortName;
               ++idx; //5
               parms[idx] = new SqlParameter("@i_contact", SqlDbType.VarChar, 20);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sContact;
               ++idx;//6
               parms[idx] = new SqlParameter("@i_isupd", SqlDbType.VarChar, 2);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = isupd;





               SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
               trans.Commit();
               connection.Close();

           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("Save DRIVING_SCHOOL  Error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }

       }



       /// <summary>
       /// 获取驾校信息
       /// </summary>

       public static void GetDrvSchool(TrainManagementDataSet DrvSchoolDataTable)
       {

           string SP_NAME = "sp_GetDrvSchool";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               string[] tableName = { "DrvSchoolDataTable" };

               SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, DrvSchoolDataTable, tableName);
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("Get DRIVING_SCHOOL record error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }

       }

       /// <summary>
       /// 删除对应ID的驾校信息
       /// </summary>

       public static void DelDrvSchool(int sDrvSchoolId)
       {

           string SP_NAME = "sp_DelDrvSchool";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[1];
               parms[0] = new SqlParameter("@i_drvschoolid", SqlDbType.Int);
               parms[0].Direction = ParameterDirection.Input;
               parms[0].Value = sDrvSchoolId;

               SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
               trans.Commit();
               connection.Close();
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("DEL DrvSchool record error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }
       }

       /// <summary>
       /// 保存车辆信息
       /// </summary>
       public static void SaveTraCarInfo(string sAutoId, string sAutoTypeCd, string sAutoName, string sBrand, string sModel, string sStation, double dAutoLength, double dAutoWith, double dAutoHeight, string sPlateNo, string sExamCd, string sAutoStatusCd, string sIpAddress, string sVtIpAddress, string sTrainerId, string isupd)
       {

           string SP_NAME = "sp_InsertTraCar";
           SqlConnection connection = null;
           SqlTransaction trans = null;
           string[] tableName = { "TRA_CAR_INFO" };

           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[16];

               int idx = 0;    //0
               parms[idx] = new SqlParameter("@i_auto_id", SqlDbType.VarChar, 20);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sAutoId;
               ++idx;  //1
               parms[idx] = new SqlParameter("@i_auto_type_cd", SqlDbType.VarChar, 2);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sAutoTypeCd;
               ++idx;  //2
               parms[idx] = new SqlParameter("@i_auto_name", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sAutoName;
               ++idx;  //3
               parms[idx] = new SqlParameter("@i_brand", SqlDbType.VarChar, 20);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sBrand;
               ++idx;  //4
               parms[idx] = new SqlParameter("@i_model", SqlDbType.VarChar, 20);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sModel;
               ++idx; //5
               parms[idx] = new SqlParameter("@i_station_id", SqlDbType.VarChar, 20);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sStation;
               ++idx;    //6
               parms[idx] = new SqlParameter("@i_auto_length", SqlDbType.Float);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = dAutoLength;
               ++idx;  //7
               parms[idx] = new SqlParameter("@i_auto_width", SqlDbType.Float);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = dAutoWith;
               ++idx;  //8
               parms[idx] = new SqlParameter("@i_auto_height", SqlDbType.Float);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = dAutoHeight;
               ++idx;  //9
               parms[idx] = new SqlParameter("@i_plate_no", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sPlateNo;
               ++idx;  //10
               parms[idx] = new SqlParameter("@i_exam_cd", SqlDbType.VarChar, 2);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sExamCd;
               ++idx; //11
               parms[idx] = new SqlParameter("@i_auto_status_cd", SqlDbType.VarChar, 2);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sAutoStatusCd;
               ++idx;  //12
               parms[idx] = new SqlParameter("@i_ip_address", SqlDbType.VarChar, 20);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sIpAddress;
               ++idx;  //13
               parms[idx] = new SqlParameter("@i_vt_ip_address", SqlDbType.VarChar, 50);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sVtIpAddress;
               ++idx; //14
               parms[idx] = new SqlParameter("@i_trainer_id", SqlDbType.VarChar, 20);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sTrainerId;
               ++idx; //15
               parms[idx] = new SqlParameter("@i_idupd", SqlDbType.VarChar, 2);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = isupd;





               SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
               trans.Commit();
               connection.Close();

           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("Save TRA_CAR_INFO  Error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }

       }



       /// <summary>
       /// 获取车辆信息
       /// </summary>
       public static void GetTraCar(TrainManagementDataSet TraCarDataTable)
       {

           string SP_NAME = "sp_GetTraCar";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               string[] tableName = { "TraCarDataTable" };

               SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, TraCarDataTable, tableName);
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("Get Tra_Car record error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }

       }

       /// <summary>
       /// 删除对应编号的教练车信息
       /// </summary>

       public static void DelTraCarNo(string sTraCarNo)
       {

           string SP_NAME = "sp_DelTraCarNo";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[1];
               parms[0] = new SqlParameter("@i_tracarno", SqlDbType.VarChar, 20);
               parms[0].Direction = ParameterDirection.Input;
               parms[0].Value = sTraCarNo;

               SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
               trans.Commit();
               connection.Close();
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("DEL TraCar record error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }
       }
       /// <summary>
       /// 保存教练员信息
       /// </summary>
       public static void SaveTrainer(string sTrainerId, string sTrainerName, string sUnit, string sPhoneNo, byte[] sPhoto, string sFingerprint, string sIcCardNo, string sAllowAbleLicenseType, DateTime dTermofvakidity, string sStatus, string sAllowAbleAutoId, int sTrainerDrv, string sTrainerPidNo, string sidupd)
       {

           string SP_NAME = "sp_InsertTrainer";
           SqlConnection connection = null;
           SqlTransaction trans = null;
           string[] tableName = { "TRAINER" };

           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[14];

               int idx = 0;    //0
               parms[idx] = new SqlParameter("@i_trainer_id", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sTrainerId;
               ++idx;  //1
               parms[idx] = new SqlParameter("@i_trainer_name", SqlDbType.VarChar, 20);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sTrainerName;
               ++idx;  //2
               parms[idx] = new SqlParameter("@i_unit", SqlDbType.VarChar, 50);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sUnit;
               ++idx;  //3
               parms[idx] = new SqlParameter("@i_phone_no", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sPhoneNo;
               ++idx;  //4
               parms[idx] = new SqlParameter("@i_photo", SqlDbType.Image);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sPhoto;
               ++idx; //5
               parms[idx] = new SqlParameter("@i_fingerprint ", SqlDbType.VarChar, 2000);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sFingerprint;
               ++idx;    //6
               parms[idx] = new SqlParameter("@i_iccardno ", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sIcCardNo;
               ++idx;  //7
               parms[idx] = new SqlParameter("@i_allowablelicensetype", SqlDbType.VarChar, 2);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sAllowAbleLicenseType;
               ++idx;  //8
               parms[idx] = new SqlParameter("@i_termofvalidity ", SqlDbType.DateTime);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = dTermofvakidity;
               ++idx;  //9
               parms[idx] = new SqlParameter("@i_status", SqlDbType.VarChar, 10);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sStatus;
               ++idx;  //10
               parms[idx] = new SqlParameter("@i_allowableautoid", SqlDbType.VarChar, 10);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sAllowAbleAutoId;
               ++idx;  //11
               parms[idx] = new SqlParameter("@i_drvschoolid", SqlDbType.Int);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sTrainerDrv;
               ++idx;  //12
               parms[idx] = new SqlParameter("@i_pidno", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sTrainerPidNo;
               ++idx; //13
               parms[idx] = new SqlParameter("@i_replacePhoto", SqlDbType.VarChar, 1);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sidupd;

               SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
               trans.Commit();
               connection.Close();

           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("Save TRAINER  Error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }

       }

       /// <summary>
       /// 获取教练员信息
       /// </summary>
       public static void GetTrainer(TrainManagementDataSet TrainerDataTable)
       {

           string SP_NAME = "sp_GetTrainer";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               string[] tableName = { "TrainerDataTable" };

               SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, TrainerDataTable, tableName);
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("Get Trainer record error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }

       }

       /// <summary>
       /// 删除对应编号的教练员信息
       /// </summary>

       public static void DelTrainer(string sTrainerNo)
       {

           string SP_NAME = "sp_DelTrainer";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[1];
               parms[0] = new SqlParameter("@i_trainerid", SqlDbType.VarChar, 30);
               parms[0].Direction = ParameterDirection.Input;
               parms[0].Value = sTrainerNo;

               SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
               trans.Commit();
               connection.Close();
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("DEL TraCar record error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }
       }
       /// <summary>
       /// 保存管理员信息
       /// </summary>
       public static void SaveTraManager(string sTraManagerId, string sPassword, string sTraManagerName, string sUnit, string sPhoneNo, byte[] sPhoto, string sFingerprint, string sIcCardNo, string sIpAddress, string sAuthManagement, string sReplacePhoto)
       {

           string SP_NAME = "sp_InsertTraManager";
           SqlConnection connection = null;
           SqlTransaction trans = null;
           string[] tableName = { "TRA_MANAGER" };

           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[11];

               int idx = 0;    //0
               parms[idx] = new SqlParameter("@i_tra_manager_id", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sTraManagerId;
               ++idx;  //1
               parms[idx] = new SqlParameter("@i_password", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sPassword;
               ++idx;  //2
               parms[idx] = new SqlParameter("@i_tra_manager_name", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sTraManagerName;
               ++idx;  //3
               parms[idx] = new SqlParameter("@i_unit", SqlDbType.VarChar, 50);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sUnit;
               ++idx;  //4
               parms[idx] = new SqlParameter("@i_phone_no", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sPhoneNo;
               ++idx; //5
               parms[idx] = new SqlParameter("@i_photo", SqlDbType.Image);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sPhoto;
               ++idx;    //6
               parms[idx] = new SqlParameter("@i_fingerprint", SqlDbType.VarChar, 2000);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sFingerprint;
               ++idx;  //7
               parms[idx] = new SqlParameter("@i_iccardno", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sIcCardNo;
               ++idx;  //8
               parms[idx] = new SqlParameter("@i_ip_address ", SqlDbType.VarChar, 200);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sIpAddress;
               ++idx;  //9
               parms[idx] = new SqlParameter("@i_authmanagement", SqlDbType.VarChar, 20);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sAuthManagement;
               ++idx;  //10
               parms[idx] = new SqlParameter("@i_replacePhoto", SqlDbType.VarChar, 1);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sReplacePhoto;



               SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
               trans.Commit();
               connection.Close();

           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("Save TRA_MANAGER  Error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }

       }



       /// <summary>
       /// 获取管理员信息
       /// </summary>
       public static void GetTraManager(TrainManagementDataSet TraManagerDataTable)
       {

           string SP_NAME = "sp_GetTraManager";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               string[] tableName = { "TraManagerDataTable" };

               SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, TraManagerDataTable, tableName);
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("Get TRA_MANAGER record error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }

       }

       /// <summary>
       /// 删除对应ID的用户信息
       /// </summary>

       public static void DelTraMnaager(string sTraManagerId)
       {

           string SP_NAME = "sp_DelTraManager";

           SqlConnection connection = null;
           SqlTransaction trans = null;
           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[1];
               parms[0] = new SqlParameter("@i_tramanagerid", SqlDbType.VarChar, 30);
               parms[0].Direction = ParameterDirection.Input;
               parms[0].Value = sTraManagerId;

               SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
               trans.Commit();
               connection.Close();
           }
           catch (Exception ex)
           {
               if (trans != null)
                   trans.Rollback();

               throw new ApplicationException("DEL TraCar record error: " + ex.Message);
           }
           finally
           {
               if (connection != null && connection.State == ConnectionState.Open)
                   connection.Close();

               if (connection != null)
                   connection.Dispose();
           }
       }


       /// <summary>
       /// 保存基本配置信息
       /// </summary>
       public static void SaveTraSetting(string sRsTxt, string sRsValue, string Notes,string sCarType)
       {

           string SP_NAME = "sp_InsertTraSetting";
           SqlConnection connection = null;
           SqlTransaction trans = null;
           string[] tableName = { "TRA_SETTING" };

           try
           {
               connection = new SqlConnection(CONN_STRING);
               connection.Open();
               trans = connection.BeginTransaction();

               SqlParameter[] parms = new SqlParameter[4];

               int idx = 0;    //0
               parms[idx] = new SqlParameter("@i_rstxt", SqlDbType.VarChar, 30);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sRsTxt;
               ++idx;  //1
               parms[idx] = new SqlParameter("@i_value", SqlDbType.VarChar, 200);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sRsValue;
               ++idx;  //2
               parms[idx] = new SqlParameter("@i_notes", SqlDbType.VarChar, 200);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = Notes;
               ++idx;  //3
               parms[idx] = new SqlParameter("@i_car_type", SqlDbType.VarChar, 100);
               parms[idx].Direction = ParameterDirection.Input;
               parms[idx].Value = sCarType; 

                SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
                trans.Commit();
                connection.Close();

            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();

                throw new ApplicationException("Save TRA_Setting  Error: " + ex.Message);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();

                if (connection != null)
                    connection.Dispose();
            }

        }
        /// <summary>
        /// 获取基本配置信息
        /// </summary>
        public static void GetTraSetting(TrainManagementDataSet Tra_Setting)
        {

            string SP_NAME = "sp_GetTraSetting";

            SqlConnection connection = null;
            SqlTransaction trans = null;
            try
            {
                connection = new SqlConnection(CONN_STRING);
                connection.Open();
                string[] tableName = { "Tra_Setting" };

                SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, Tra_Setting, tableName);
            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();

                throw new ApplicationException("Get Tra_Setting record error: " + ex.Message);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();

                if (connection != null)
                    connection.Dispose();
            }

        }
        /// <summary>
        /// 获取项目信息
        /// </summary>
        public static void GetEiCd(TrainManagementDataSet Ei_Cd_List)
        {

            string SP_NAME = "sp_GetEiCdList";

            SqlConnection connection = null;
            SqlTransaction trans = null;
            try
            {
                connection = new SqlConnection(CONN_STRING);
                connection.Open();
                string[] tableName = { "EI_CD_LIST" };

                SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, Ei_Cd_List, tableName);
            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();

                throw new ApplicationException("Get Tra_Setting record error: " + ex.Message);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();

                if (connection != null)
                    connection.Dispose();
            }

        }

        /// <summary>
        /// 获取扣分代码
        /// </summary>
        public static void GetVlCd(TrainManagementDataSet Vl_Cd_List)
        {

            string SP_NAME = "sp_GetVlCdList";

            SqlConnection connection = null;
            SqlTransaction trans = null;
            try
            {
                connection = new SqlConnection(CONN_STRING);
                connection.Open();
                string[] tableName = { "VIOLATION_CD_LIST" };

                SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, Vl_Cd_List, tableName);
            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();

                throw new ApplicationException("Get Tra_Setting record error: " + ex.Message);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();

                if (connection != null)
                    connection.Dispose();
            }

        }

        /// <summary>
        /// 删除对应txt的基本配置信息
        /// </summary>

        public static void DelTraSetting(string sTxt)
        {

            string SP_NAME = "sp_DelTraSetting";

            SqlConnection connection = null;
            SqlTransaction trans = null;
            try
            {
                connection = new SqlConnection(CONN_STRING);
                connection.Open();
                trans = connection.BeginTransaction();

                SqlParameter[] parms = new SqlParameter[1];
                parms[0] = new SqlParameter("@i_traTxt", SqlDbType.VarChar, 30);
                parms[0].Direction = ParameterDirection.Input;
                parms[0].Value = sTxt;

                SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
                trans.Commit();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();

                throw new ApplicationException("DEL TraSetting record error: " + ex.Message);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();

                if (connection != null)
                    connection.Dispose();
            }
        }

        /// <summary>
        /// 保存训练过程信息
        /// </summary>
        public static int SaveTraProcessInfo(string AutoId,string PidNo,string Date,DateTime TrainStartTs,DateTime TrainEndTs,string Trainer,string AutoType,double TrainTime,int TraBookSeqNo,int TrainTries, double TrainMileage)
        {
                SqlConnection connection = null;
                SqlTransaction trans = null;
                string SP_NAME = "sp_InsertTraProcessInfo";
           try
            {
                connection = new SqlConnection(CONN_STRING);
                connection.Open();
                trans = connection.BeginTransaction();                
                SqlCommand cmd = new SqlCommand(SP_NAME, connection);
                cmd.Transaction = trans;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@i_auto_id", SqlDbType.VarChar,20));
                cmd.Parameters["@i_auto_id"].Value = AutoId;
                cmd.Parameters.Add(new SqlParameter("@i_pid_no", SqlDbType.VarChar,30));
                cmd.Parameters["@i_pid_no"].Value = PidNo;
                cmd.Parameters.Add(new SqlParameter("@i_train_dt", SqlDbType.VarChar,20));     
                cmd.Parameters["@i_train_dt"].Value = Date; 
                cmd.Parameters.Add(new SqlParameter("@i_train_start_ts", SqlDbType.DateTime));
                cmd.Parameters["@i_train_start_ts"].Value = TrainStartTs; 
                cmd.Parameters.Add(new SqlParameter("@i_train_end_ts", SqlDbType.DateTime));
                cmd.Parameters["@i_train_end_ts"].Value = TrainEndTs; 
                cmd.Parameters.Add(new SqlParameter("@i_trainer_name", SqlDbType.VarChar,30));
                cmd.Parameters["@i_trainer_name"].Value = Trainer;
                cmd.Parameters.Add(new SqlParameter("@i_auto_type_cd", SqlDbType.VarChar,10)); 
                cmd.Parameters["@i_auto_type_cd"].Value = AutoType;
                cmd.Parameters.Add(new SqlParameter("@i_train_time", SqlDbType.Float, 20));
                cmd.Parameters["@i_train_time"].Value = TrainTime;
                cmd.Parameters.Add(new SqlParameter("@i_trabook_seq_no", SqlDbType.Int, 20));
                cmd.Parameters["@i_trabook_seq_no"].Value = TraBookSeqNo;
                cmd.Parameters.Add(new SqlParameter("@i_tra_tries", SqlDbType.Int, 20));
                cmd.Parameters["@i_tra_tries"].Value = TrainTries;
                cmd.Parameters.Add(new SqlParameter("@i_tra_mileage", SqlDbType.Float, 20));
                cmd.Parameters["@i_tra_mileage"].Value = TrainMileage;
                cmd.Parameters.Add(new SqlParameter("@i_train_proc_seqno", SqlDbType.Int));
                cmd.Parameters["@i_train_proc_seqno"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                var procSeqNo= int.Parse(cmd.Parameters["@i_train_proc_seqno"].Value.ToString());
                trans.Commit();             
                connection.Close();
                return procSeqNo;
            }      
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();

                throw new ApplicationException("保存训练过程信息  Error: " + ex.Message);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();

                if (connection != null)
                    connection.Dispose();
            }
            
        }


        /// <summary>
        /// 保存训练过程扣分信息
        /// </summary>
        public static void SaveTraProcessPoints(int iSeqNo,string sEiCode,DateTime dEiTsTamp,string sTraModel,string sTraType)
        {
            
            string SP_NAME = "sp_InsertTraProcessPoints";
            SqlConnection connection = null;
            SqlTransaction trans = null;
            string[] tableName = { "TRA_PROCESS_POINTS" };

            try
            {
                connection = new SqlConnection(CONN_STRING);
                connection.Open();
                trans = connection.BeginTransaction();

                SqlParameter[] parms = new SqlParameter[5];

                int idx = 0;    //0
                parms[idx] = new SqlParameter("@i_seq_no", SqlDbType.Int);
                parms[idx].Direction = ParameterDirection.Input;
                parms[idx].Value = iSeqNo;
                ++idx;  //1
                parms[idx] = new SqlParameter("@i_ei_code", SqlDbType.VarChar, 10);
                parms[idx].Direction = ParameterDirection.Input;
                parms[idx].Value = sEiCode;
                ++idx;  //2
                parms[idx] = new SqlParameter("@i_ei_ts_tamp", SqlDbType.DateTime);
                parms[idx].Direction = ParameterDirection.Input;
                parms[idx].Value = dEiTsTamp;
                ++idx;  //3
                parms[idx] = new SqlParameter("@i_tra_mode", SqlDbType.VarChar,2);
                parms[idx].Direction = ParameterDirection.Input;
                parms[idx].Value = sTraModel;
                ++idx;  //4
                parms[idx] = new SqlParameter("@i_tra_type", SqlDbType.VarChar,2);
                parms[idx].Direction = ParameterDirection.Input;
                parms[idx].Value = sTraType;
               


                SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
                trans.Commit();
                connection.Close();

            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();

                throw new ApplicationException("Save TRA_PROCESS_POINTS  Error: " + ex.Message);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();

                if (connection != null)
                    connection.Dispose();
            }
            
        }



        /// <summary>
        /// 查询学员训练结果
        /// </summary>
        public static void GetTraProcess(string sExamCd, string CarType, string sTrainerId, int iDrvSchoolId, string dStartTs, string dEndTs, string TraName, string sPidNo, string autoid,int traBookSeqNo,  TrainManagementDataSet TraManagerDataTable)
        {
            string SP_NAME = "sp_GetProcess";

            SqlConnection connection = null;
            SqlTransaction trans = null;
            try
            {
                connection = new SqlConnection(CONN_STRING);
                connection.Open();
           
                string[] tableName = { "TraProcessInfoDataTable" };
                SqlParameter[] parms = new SqlParameter[10];
               
                int index = 0;//0   
                parms[index] = new SqlParameter("@i_examcd", SqlDbType.VarChar, 20);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = sExamCd;
                ++index;// 1     
                parms[index] = new SqlParameter("@i_cartype", SqlDbType.VarChar,30);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = CarType;
                ++index;//2          
                parms[index] = new SqlParameter("@i_trainerid", SqlDbType.VarChar, 30);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = sTrainerId;
                ++index;//3               
                parms[index] = new SqlParameter("@i_drvschoolid", SqlDbType.Int, 30);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = iDrvSchoolId;
                ++index;// 4    
                parms[index] = new SqlParameter("@i_start_ts", SqlDbType.VarChar,50);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = dStartTs;
                ++index;// 5        
                parms[index] = new SqlParameter("@i_end_ts", SqlDbType.VarChar,50);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = dEndTs;           
                ++index;//6            
                parms[index] = new SqlParameter("@i_traname", SqlDbType.VarChar, 30);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = TraName;
                ++index;//7           
                parms[index] = new SqlParameter("@i_trapidno", SqlDbType.VarChar, 30);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = sPidNo;
                //++index;// 8      
                //parms[index] = new SqlParameter("@i_eino", SqlDbType.VarChar,30);
                //parms[index].Direction = ParameterDirection.Input;
                //parms[index].Value = EiNo;
                //++index;//9          
                //parms[index] = new SqlParameter("@i_vlno", SqlDbType.VarChar, 30);
                //parms[index].Direction = ParameterDirection.Input;
                //parms[index].Value = VlNo;
                //++index;//10           
                //parms[index] = new SqlParameter("@i_tramodel", SqlDbType.VarChar, 30);
                //parms[index].Direction = ParameterDirection.Input;
                //parms[index].Value = traSisModel;
                ++index;//10           
                parms[index] = new SqlParameter("@i_autoid", SqlDbType.VarChar, 30);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = autoid;
                ++index;//10           
                parms[index] = new SqlParameter("@i_trabookseqno", SqlDbType.Int);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = traBookSeqNo;
                SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, TraManagerDataTable, tableName, parms);
                
            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();

                throw new ApplicationException("Get Tra_Process record error: " + ex.Message);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();

                if (connection != null)
                    connection.Dispose();
            }
        }

        /// <summary>
        /// 查询学员训练相信信息
        /// </summary>
        public static void GetTraProcessPoints(int traSeqNo, TrainManagementDataSet TraManagerDataTable)
        {
            string SP_NAME = "sp_GetProcessPoints";

            SqlConnection connection = null;
            SqlTransaction trans = null;
            try
            {
                connection = new SqlConnection(CONN_STRING);
                connection.Open();

                string[] tableName = { "TraProcessPointsDataTable" };
                SqlParameter[] parms = new SqlParameter[1];

                int index = 0;//0   
                parms[index] = new SqlParameter("@i_proSeqNo", SqlDbType.Int);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = traSeqNo;
             
                SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, TraManagerDataTable, tableName, parms);

            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();

                throw new ApplicationException("Get Tra_ProcessPoints record error: " + ex.Message);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();

                if (connection != null)
                    connection.Dispose();
            }
        }
       
       /// <summary>
        /// 获取指定学员的训练许可
        /// </summary>
        public static void getTrainLicense(int sBookSeqNo, string sPidNo, TrainManagementDataSet TraManagerDataTable)
        {
            string SP_NAME = "sp_getTrainLicense";

            SqlConnection connection = null;
            SqlTransaction trans = null;
            try
            {
                connection = new SqlConnection(CONN_STRING);
                connection.Open();

                string[] tableName = { "TraTrainLicenseDataTable" };
                SqlParameter[] parms = new SqlParameter[2];

                int index = 0;//0               
                parms[index] = new SqlParameter("@i_bookSeqNo", SqlDbType.Int);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = sBookSeqNo;
                ++index;// 1             
                parms[index] = new SqlParameter("@i_pidNo", SqlDbType.VarChar);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = sPidNo;                    
                SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, TraManagerDataTable, tableName, parms);

            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();

                throw new ApplicationException("Get TraTrainLicense record error: " + ex.Message);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();

                if (connection != null)
                    connection.Dispose();
            }
        }
        /// <summary>
        /// 获取充值和消费信息
        /// </summary>
        public static void getTraFinance(string sPidNo,string sName,int drvNo,DateTime strTime,DateTime endTime, TrainManagementDataSet TraManagerDataTable)
        {
            string SP_NAME = "sp_getTraFinance";

            SqlConnection connection = null;
            SqlTransaction trans = null;
            try
            {
                connection = new SqlConnection(CONN_STRING);
                connection.Open();

                string[] tableName = { "TraFinanceDataTable" };
                SqlParameter[] parms = new SqlParameter[5];

                int index = 0;//0               
                parms[index] = new SqlParameter("@i_pidno", SqlDbType.VarChar);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = sPidNo;
                ++index;// 1  
                parms[index] = new SqlParameter("@i_name", SqlDbType.VarChar);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = sName;
                ++index;// 2 
                parms[index] = new SqlParameter("@i_drvno", SqlDbType.Int);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = drvNo;
                ++index;// 3 
                parms[index] = new SqlParameter("@i_strtime", SqlDbType.DateTime);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = strTime;
                ++index;// 4
                parms[index] = new SqlParameter("@i_endtime", SqlDbType.DateTime);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = endTime;
                SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, TraManagerDataTable, tableName, parms);

            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();

                throw new ApplicationException("Get TraTrainLicense record error: " + ex.Message);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();

                if (connection != null)
                    connection.Dispose();
            }
        }

        /// <summary>
        /// 获取充值记录
        /// </summary>
        public static void getRechargeRecord(string sPidNo, DateTime strTime, DateTime endTime, TrainManagementDataSet TraManagerDataTable)
        {
            string SP_NAME = "sp_getRechargeRecord";

            SqlConnection connection = null;
            SqlTransaction trans = null;
            try
            {
                connection = new SqlConnection(CONN_STRING);
                connection.Open();

                string[] tableName = { "RECHARGE_RECORDDataTable" };
                SqlParameter[] parms = new SqlParameter[3];

                int index = 0;//0               
                parms[index] = new SqlParameter("@i_pidno", SqlDbType.VarChar);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = sPidNo;
                ++index;// 1
                parms[index] = new SqlParameter("@i_strtime", SqlDbType.DateTime);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = strTime;
                ++index;// 2
                parms[index] = new SqlParameter("@i_endtime", SqlDbType.DateTime);
                parms[index].Direction = ParameterDirection.Input;
                parms[index].Value = endTime;
                SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, TraManagerDataTable, tableName, parms);

            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();

                throw new ApplicationException("Get TraTrainLicense record error: " + ex.Message);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();

                if (connection != null)
                    connection.Dispose();
            }
        }

		public static void setStudentType(string pidNo, int stuType)
		{
			string SP_NAME = "sp_updTraStuTypeByPidNo";
			SqlConnection connection = null;
			SqlTransaction trans = null;
			string[] tableName = { "TRA_STUDINFO" };

			try
			{
				connection = new SqlConnection(CONN_STRING);
				connection.Open();
				trans = connection.BeginTransaction();

				SqlParameter[] parms = new SqlParameter[2];

				int idx = 0;    //0
				parms[idx] = new SqlParameter("@i_pid_no", SqlDbType.VarChar,30);
				parms[idx].Direction = ParameterDirection.Input;
				parms[idx].Value = pidNo;
				++idx;  //1
				parms[idx] = new SqlParameter("@i_stu_type", SqlDbType.Int);
				parms[idx].Direction = ParameterDirection.Input;
				parms[idx].Value = stuType;


				SqlHelper.ExecuteNonQuery(trans, SP_NAME, parms);
				trans.Commit();
				connection.Close();

			}
			catch (Exception ex)
			{
				if (trans != null)
					trans.Rollback();

				throw new ApplicationException("Save TRA_PROCESS_POINTS  Error: " + ex.Message);
			}
			finally
			{
				if (connection != null && connection.State == ConnectionState.Open)
					connection.Close();

				if (connection != null)
					connection.Dispose();
			}
		}

	  ///<sunmary>
	  ///匹配用户名密码
	  ///</sunmary>
	  ///
		public static void GetExaminer(string examinerId, string password, TrainManagementDataSet dataset)
		{
			//string ConnString = Common.DBConnectionString;
			const string SP_NAME = "sp_getexaminer";
			SqlConnection connection = null;
			SqlTransaction trans = null;
			try
			{
				connection = new SqlConnection(CONN_STRING);
				connection.Open();
				string[] tableName = { "TraManagerDataTable" };
				SqlParameter[] parms = new SqlParameter[2];
				parms[0] = new SqlParameter("@i_examr_id", SqlDbType.VarChar, 30);
				parms[0].Direction = ParameterDirection.Input;
				parms[0].Value = examinerId;

				parms[1] = new SqlParameter("@i_pwd", SqlDbType.VarChar, 30);
				parms[1].Direction = ParameterDirection.Input;
				parms[1].Value = password;

				SqlHelper.FillDataset(CONN_STRING, CommandType.StoredProcedure, SP_NAME, dataset, tableName, parms);
				//dataset=(AppCodeDataset)SqlHelper.ExecuteDataset(connection,CommandType.StoredProcedure,SP_NAME);
				// Get the parent and child columns of the two tables.
			}
			catch (Exception ex)
			{
				if (trans != null)
					trans.Rollback();

				throw ex;
			}
			finally
			{
				if (connection != null && connection.State == ConnectionState.Open)
					connection.Close();

				if (connection != null)
					connection.Dispose();
			}
		}


    }
}
