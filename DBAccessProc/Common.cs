using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using EncDecLib;
using System.Net;
using System.Text;
using System.IO;

namespace DBAccessProc
{
	/// <summary>
	/// Summary description for Common.
	/// </summary>
	public class Common
	{

		#region Member Variables

		private static string _packageName = string.Empty;
		private static string _schema = string.Empty;
        private static string _dbConnectionString = string.Empty;
        private static string _OracleDBConnectionString = string.Empty;
        private static string _OracleFPDBConnectionString = string.Empty;
        private static string _SqlFPDBConnectionString = string.Empty;

		#endregion

		#region Constructor

		public Common()
		{
		}

		#endregion

		#region Properties

		public static string ConfigFileName
		{
			get
			{
				return ConfigurationManager.ConfigFileName;
			}
		}

        public static String OracleDBConnectionString
        {
            get
            {
                if (_OracleDBConnectionString == string.Empty)
                {
                    try
                    {
                        string sEncrypted = ConfigurationManager.AppSettings["OracleDBConnectionString"];
                        ClsEncDec ced = new ClsEncDec();
                        _OracleDBConnectionString = ced.DecryptWork(sEncrypted);
                    }
                    catch
                    {
                        _OracleDBConnectionString = "Data Source=58.42.245.133:1521:orcl1;User Id=DRV_KMD;Password=DRV_KMD;Integrated Security=no;";
                    }
                }

                return _OracleDBConnectionString;
            }
        }

        public static String OracleSchema
        {
            get
            {                
                try
                {
                    return ConfigurationManager.AppSettings["OracleSchema"]; 
                }
                catch
                {
                    return "drv_admin";
                }
            }
        }

        public static String OracleFPDBConnectionString
        {
            get
            {
                if (_OracleFPDBConnectionString == string.Empty)
                {
                    try
                    {
                        string sEncrypted = ConfigurationManager.AppSettings["OracleFPDBConnectionString"];
                        ClsEncDec ced = new ClsEncDec();
                        _OracleFPDBConnectionString = ced.DecryptWork(sEncrypted);
                    }
                    catch
                    {
                        _OracleFPDBConnectionString = "Data Source=JJMIS;User Id=zwyz;Password=zwyz;Integrated Security=no;";   //10.25.20.7
                    }
                }

                return _OracleFPDBConnectionString;
            }
        }

        public static String SqlFPDBConnectionString
        {
            get
            {
                if (_SqlFPDBConnectionString == string.Empty)
                {
                    try
                    {
                        string sEncrypted = ConfigurationManager.AppSettings["SqlFPDBConnectionString"];
                        ClsEncDec ced = new ClsEncDec();
                        _SqlFPDBConnectionString = ced.DecryptWork(sEncrypted);
                    }
                    catch
                    {
                        _SqlFPDBConnectionString = "Data Source=10.161.119.119;Initial Catalog=Vehicle;User Id=FPUser;Password=FPUser"; 
                    }
                }

                return _SqlFPDBConnectionString;
            }
        }		

		public static String DBConnectionString
		{
			get
			{
                if (_dbConnectionString == string.Empty)
                {
                    try
                    {
                        string sEncrypted = ConfigurationManager.AppSettings["SqlDBConnectionString"];
                        ClsEncDec ced = new ClsEncDec();
                        //_dbConnectionString = ced.DecryptWork(sEncrypted);
                        string sPlainTxt = ced.DecryptWork(sEncrypted);
                        int nIdx = sPlainTxt.LastIndexOf(';');
                        string sIPAddress = sPlainTxt.Substring(nIdx + 1);
                        if (sIPAddress == string.Empty)
                            _dbConnectionString = "Data Source=YHC\\YHCSERVER;Initial Catalog=TRADBTEST;User Id=sa;Password=iddqd_3852;";
                         //   _dbConnectionString = ced.DecryptWork(sEncrypted);
                        else
                        {
                            bool bIsFound = false;
                            IPHostEntry myIPHostEntry = Dns.GetHostEntry(Dns.GetHostName());
                            foreach (IPAddress myIPAddress in myIPHostEntry.AddressList)
                            {
                                if (myIPAddress.ToString() == sIPAddress)
                                    bIsFound = true;
                            }

                            if (!bIsFound)
                            {
                                MessageBox.Show("使用不在限定IP的设备，无法登陆系统！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Environment.Exit(0);
                            }
                            else
                                _dbConnectionString = sPlainTxt.Substring(0, nIdx);
                        }
                    }
                    catch
                    {
                        try
                        {
                            _dbConnectionString = "Data Source=" + ConfigurationManager.AppSettings["SqlServerName"] + ";Initial Catalog=DETSDB_KM2;User Id=ExamAppUser;Password=iddqd_4497;";
                        }
                        catch
                        {
                            _dbConnectionString = "Data Source=" + System.Environment.MachineName + ";Initial Catalog=DETSDB;User Id=ExamAppUser;Password=iddqd_4497;";
                        }
                    }
                }

                return _dbConnectionString; 
			}
		}

		public static String CheckBoxHeight
		{
			get
			{
				return ConfigurationManager.AppSettings["CheckBoxHeight"]; 
			}
		}
		public static String CheckBoxWidth
		{
			get
			{
				return ConfigurationManager.AppSettings["CheckBoxWidth"]; 
			}
		}

		public static String QuestionWidth
		{
			get
			{
				return ConfigurationManager.AppSettings["QuestionWidth"]; 
			}
		}
		
		public static String QuestionHeight
		{
			get
			{
				return ConfigurationManager.AppSettings["QuestionHeight"]; 
			}
		}

		
		public static String SeperateSpace
		{
			get
			{
				return ConfigurationManager.AppSettings["SeperateSpace"]; 
			}
		}

		public static String QickAccessLinkWidth
		{
			get
			{
				return ConfigurationManager.AppSettings["QickAccessLinkWidth"]; 
			}
		}

		public static String QickAccessLinkHeight
		{
			get
			{
				return ConfigurationManager.AppSettings["QickAccessLinkHeight"]; 
			}
		}
		public static String TotalPerLine
		{
			get
			{
				return ConfigurationManager.AppSettings["TotalPerLine"]; 
			}
		}

		public static String UseFont
		{
			get
			{
				return ConfigurationManager.AppSettings["UseFont"]; 
			}
		}
		
		public static String QuestionHeightWithImage
		{
			get
			{
				return ConfigurationManager.AppSettings["QuestionHeightWithImage"]; 
			}
		}	

		public static String TimerIntervalTime
		{
			get
			{
				return ConfigurationManager.AppSettings["TimerIntervalTime"]; 
			}
		}
        
        public static String WebCamName
		{
			get
			{
                return ConfigurationManager.AppSettings["WebCamName"]; 
			}
		}
        
        public static String VideoSource
        {
            get
            {
                return ConfigurationManager.AppSettings["VideoSource"];
            }
        }

        public static String VideoStandard
        {
            get
            {
                return ConfigurationManager.AppSettings["VideoStandard"];
            }
        }

        public static String CorpName
		{
			get
			{
				return ConfigurationManager.AppSettings["CorpName"]; 
			}
		}

        public static String PhotoWidth
        {
            get
            {
                return ConfigurationManager.AppSettings["PhotoWidth"];
            }
        }

        public static String PhotoHeight
        {
            get
            {
                return ConfigurationManager.AppSettings["PhotoHeight"];
            }
        }

        public static String ExamPaper
		{
			get
			{
                return ConfigurationManager.AppSettings["ExamPaper"]; 
			}
		}

        public static String Examiner
		{
			get
			{
                return ConfigurationManager.AppSettings["Examiner"]; 
			}
		}

        public static String Examinee
		{
			get
			{
                return ConfigurationManager.AppSettings["Examinee"]; 
			}
		}

        public static String ScoreRatio
		{
			get
			{
                return ConfigurationManager.AppSettings["ScoreRatio"]; 
			}
		}

        public static String Exit
		{
			get
			{
                return ConfigurationManager.AppSettings["Exit"]; 
			}
		}
        
        public static String Print
		{
			get
			{
                return ConfigurationManager.AppSettings["Print"]; 
			}
		}

        public static String ExamOption
		{
			get
			{
                return ConfigurationManager.AppSettings["ExamOption"]; 
			}
		}

        public static String PeSetting
		{
			get
			{
                return ConfigurationManager.AppSettings["PeSetting"]; 
			}
		}

        public static String ActionItems
		{
			get
			{
                return ConfigurationManager.AppSettings["ActionItems"]; 
			}
		}

        public static String PrintFormatTitle
        {
            get
            {
                return ConfigurationManager.AppSettings["PrintFormatTitle"];
            }
        }

        public static String PrintFormatOption
        {
            get
            {
                return ConfigurationManager.AppSettings["PrintFormatOption"];
            }
        }

        public static String PrintOutputFilePath
		{
			get
			{
                return ConfigurationManager.AppSettings["PrintOutputFilePath"]; 
			}
		}

        public static String SeatId
        {
            get
            {
                return ConfigurationManager.AppSettings["SeatId"];
            }
        }

        public static String LoginMode
        {
            get
            {
                return ConfigurationManager.AppSettings["LoginMode"];
            }
        }

        public static String VideoFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["VideoFilePath"];
            }
        }
        
        public static String TrackImageFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["TrackImageFilePath"];
            }
        }
		public static String PrintFilePath
		{
			get
			{
				return ConfigurationManager.AppSettings["PrintFilePath"];
			}
		}

        public static String GarageImageFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["GarageImageFilePath"];
            }
        }
        
        public static String TempFilePath
		{
			get
			          {
                return ConfigurationManager.AppSettings["TempFilePath"]; 
			}
		}
		public static String TemplateFilePath
		{
			get
			{
				return ConfigurationManager.AppSettings["TemplateFilePath"];
			}
		} 
        public static String RemoteAccessOption
		{
			get
			{
                return ConfigurationManager.AppSettings["RemoteAccessOption"]; 
			}
		}

        public static String ExamPlaceCode
		{
			get
			{
                return ConfigurationManager.AppSettings["ExamPlaceCode"]; 
			}
		}

        public static String MgmtDepartment
        {
            get
            {
                return ConfigurationManager.AppSettings["MgmtDepartment"];
            }
        }

        public static String InterfaceType
        {
            get
            {
                return ConfigurationManager.AppSettings["InterfaceType"];
            }
        }

        public static String RemoteAccessUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["RemoteAccessUrl"];
            }
        }

        public static String RemoteAccessCredential
        {
            get
            {                
                try
                {
                    string sEncrypted = ConfigurationManager.AppSettings["RemoteAccessCredential"];
                    ClsEncDec ced = new ClsEncDec();
                    return ced.DecryptWork(sEncrypted);
                }
                catch
                {
                    return "km2_13;111111";
                }        
            }
        }

        public static String ExamTimeUsageThreshold
        {
            get
            {
                return ConfigurationManager.AppSettings["ExamTimeUsageThreshold"];
            }
        }

        public static String UploadExamResultSN
        {
            get
            {
                return ConfigurationManager.AppSettings["UploadExamResultSN"];
            }
        }

        public static String NetExamineeSN
        {
            get
            {
                return ConfigurationManager.AppSettings["NetExamineeSN"];
            }
        }

        public static String DownloadExamePhotoSN
        {
            get
            {
                return ConfigurationManager.AppSettings["DownloadExamePhotoSN"];
            }
        }

        public static String SensorOCXOption
        {
            get
            {
                return ConfigurationManager.AppSettings["SensorOCXOption"];
            }
        }

        public static String ExamCode
        {
            get
            {
                return ConfigurationManager.AppSettings["ExamCode"];
            }
        }

        public static String FPAuthenPlaceOption
        {
            get
            {
                return ConfigurationManager.AppSettings["FPAuthenPlaceOption"];
            }
        }

        public static String AutoUploadTimerInterval
        {
            get
            {
                return ConfigurationManager.AppSettings["AutoUploadTimerInterval"];
            }
        }

        public static string GetConfigKeyValue(string strKey)
        {
            return ConfigurationManager.AppSettings[strKey];
        }

        public static String GarageDeviceIndexMap
        {
            get
            {
                return ConfigurationManager.AppSettings["GarageDeviceIndexMap"];
            }
        }
        /// <summary>
        /// 运行模式
        /// </summary>
        public static String RunningMode
        {
            get
            {
                return ConfigurationManager.AppSettings["RunningMode"];
            }
        }

        public static String NetExamineeReSN
        {
            get
            {
                return ConfigurationManager.AppSettings["NetExamineeReSN"];
            }
        }

        public static String FingerPrintSensorOcx
        {
            get
            {
                return ConfigurationManager.AppSettings["FingerPrintSensorOcx"];
            }
        }

        public static String UploadExamResultUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["UploadExamResultUrl"];
            }
        }

        public static String NetExamineeInfoUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["NetExamineeInfoUrl"];
            }
        }

        public static String ReDoExamTimeDelay
        {
            get
            {
                return ConfigurationManager.AppSettings["ReDoExamTimeDelay"];
            }
        }

        public static String AccessID
        {
            get
            {
                return ConfigurationManager.AppSettings["AccessID"];
            }
        }

        public static String UpldExamResultSN
        {
            get
            {
                return ConfigurationManager.AppSettings["UpldExamResultSN"];
            }
        }

        public static String DwldPreasignSN
        {
            get
            {
                return ConfigurationManager.AppSettings["DwldPreasignSN"];
            }
        }

        public static String DwldGroupSN
        {
            get
            {
                return ConfigurationManager.AppSettings["DwldGroupSN"];
            }
        }

        public static String DwldDetailSN
        {
            get
            {
                return ConfigurationManager.AppSettings["DwldDetailSN"];
            }
        }

        public static String DwldPhotoSN
        {
            get
            {
                return ConfigurationManager.AppSettings["DwldPhotoSN"];
            }
        }

        public static String DigiSignServer
        {
            get
            {
                return ConfigurationManager.AppSettings["DigiSignServer"];
            }
        }

        public static String AutoCheckInOption
        {
            get
            {
                return ConfigurationManager.AppSettings["AutoCheckInOption"];
            }
        }

        public static String VoiceRate
        {
            get
            {
                return ConfigurationManager.AppSettings["VoiceRate"];
            }
        }

        public static String VoiceName
        {
            get
            {
                return ConfigurationManager.AppSettings["VoiceName"];
            }
        }
        public static int ICRConnectionOption
        {
            get
            {
                int result;
                if (Int32.Parse(ConfigurationManager.AppSettings["ICRConnectionOption"]) == 0)
                    result = 1001;
                else
                    result = Int32.Parse(ConfigurationManager.AppSettings["ICRConnectionOption"]);
                return result;
            }
        }
        public static string BeginOrderPhotograph
        {
            get
            {
                return ConfigurationManager.AppSettings["BeginOrderPhotograph"];
            }
        }
        public static string EndOrderPhotograph
        {
            get
            {
                return ConfigurationManager.AppSettings["EndOrderPhotograph"];
            }
        }
        public static string BeginDoorControlPhotograph
        {
            get
            {
                return ConfigurationManager.AppSettings["BeginDoorControlPhotograph"];
            }
        }
        public static string EndDoorControlPhotograph
        {
            get
            {
                return ConfigurationManager.AppSettings["EndDoorControlPhotograph"];
            }
        }
        public static string CheckInOption
        {
            get
            {
                return ConfigurationManager.AppSettings["CheckInOption"];
            }
        }

        public static string AuditAccessUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["AuditAccessUrl"];
            }
        }

        public static string NewKM2Option
        {
            get
            {
                return ConfigurationManager.AppSettings["NewKM2Option"];
            }
        }

        public static String JKXLH
        {
            get
            {
                return ConfigurationManager.AppSettings["JKXLH"];
            }
        }
		#endregion

		#region Methods
		public static int GetRandomIndex(int MaxNumber,Random rd)
		{
			return rd.Next(MaxNumber);			
		}

		public static void SetMaxForm(System.Windows.Forms.Form frm)
		{
			frm.Location=new Point(0,0);
			frm.ControlBox=false;		
			frm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			System.Drawing.Rectangle rect;
			rect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

			// ClientSize (member of Form) is just the drawable area
			frm.ClientSize = rect.Size;
		}

		public static string FormatQuestionString(Hashtable hstbl)
		{
			string question="";
			IDictionaryEnumerator tableEnumerator = hstbl.GetEnumerator();

			int idx=0;
			while ( tableEnumerator.MoveNext() )
			{
				question+=tableEnumerator.Value.ToString();//

				if(idx==(hstbl.Count-1))
					break;
				else
					question+=",";

				++idx;
			}	

			return question;
		}

		public static void ParseExamQuestionContent(string questionContent,ArrayList recList)
		{
			string delimStr = ",";
			char [] delimiter = delimStr.ToCharArray();
			int totalCharCnt =questionContent.Length;
			string[] examQuestionList = questionContent.Split(delimiter, totalCharCnt/2+1);
			foreach (string questionNo in examQuestionList)
			{
				recList.Add(questionNo);
			}
		}

		#endregion


	}

}
