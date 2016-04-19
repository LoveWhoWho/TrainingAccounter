using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using System.Security.Cryptography;

namespace TrainingControl
{

    public class ReadAndWrite
    {
        //private List<string> _serialNumber = new List<string>();


        /// <summary>
        /// 管理端读取许可信息
        /// </summary>
        /// <param name="sPidNo">学员的身份证号</param>
        /// <returns>该学员的训练次数</returns>
        //public double ReadTrainLic(string sPidNo)
        //{
        //    List<TrainLicense> tra = null;
        //    double dTrainBalance = 0;
        //    var s = DriveInfo.GetDrives();
        //    foreach (var drive in s)
        //    {
        //        if (drive.DriveType == DriveType.Removable)
        //        {
        //            tra = ReadTrainLicense(drive.Name);
        //        }
        //    }
        //    if (tra != null && sPidNo == tra.PidNo && tra.TrainDetail != null)
        //    {
        //        if (tra.TrainDetail.TrainTime > 0)
        //            dTrainBalance = tra.TrainDetail.TrainTime;
        //        else if (tra.TrainDetail.TrainTries > 0)
        //            dTrainBalance = tra.TrainDetail.TrainTries;
        //        else if (tra.TrainDetail.TrainMileage > 0)
        //            dTrainBalance = tra.TrainDetail.TrainMileage;
        //    }
        //    return dTrainBalance;
        //}

        /// <summary>
        /// 读取U盘中的许可信息
        /// </summary>
        /// <returns>成功则返回许可类的实例，否则为NULL</returns>
		/// 
		
		private string[] _strWholeRead;
		public string[] StrWholeRead
		{
			get { return  _strWholeRead; }
			set { value = _strWholeRead; }
		}
		/// <summary>
		/// 检测U盘ＩＤ
		/// </summary>
		/// <param name="devNames"></param>
		/// <returns></returns>
		public bool CompareDisk(string diskName,string pidNo,string diskId)
		{
			bool isResult = false;
			var result = ReadTrainLicense(diskName);
			if (result != null)
			{
				foreach (var item in result)
				{
					if (item.PidNo == pidNo)
					{
						if (this.StrWholeRead[1] == diskId)
						{
							isResult = true;
						}
					}
				}
				return isResult;
			}
			else
			{
				MessageBox.Show("读取的数据为NULL，请重试");
				return false;
			}
		}
        public List<TrainLicense> ReadTrainLicense(string devNames)
        {
            List<string> _serialNumber = matchDriveLetterWithSerial(devNames.Substring(0,2));
            TrainingControl.DsRsrc dsrrc = new DsRsrc();            
            List<TrainLicense> listTraLic = null;
            string sPath = "";
            string sJsonTrainDetail = "";
            string[] strTraLic = new string[]{"",""};
            string strDecrypt = "";
            if (devNames != "")//读取数据用
            {
                sPath = devNames + "TrainLicense.dat";
                if (File.Exists(sPath))
                {
                    try
                    {
                      using(StreamReader m_swPrntr = new StreamReader(sPath, Encoding.UTF8, true))
                        {		
                            strDecrypt = m_swPrntr.ReadToEnd();
                            sJsonTrainDetail = AESDecrypt(strDecrypt, "sjhy3852");
							_strWholeRead = strTraLic = sJsonTrainDetail.Split('*');
							if (_serialNumber.Contains(strTraLic[1].Trim()/*"4C532000000315116251"*/))
                            {
                                listTraLic = JsonConvert.DeserializeObject<List<TrainLicense>>(strTraLic[0]);
                            }
                            else
                            {
                                MessageBox.Show(devNames + "该设备不合法！");
                                listTraLic = null;
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("许可无效！");
                        listTraLic = null;					
                    }
                }
            }
            return listTraLic;
        }

        public List<TrainLicense> ReadTrainLicense()
        {
            int typeError = 0;
            List<TrainLicense> tra;
            DataTable dt = new DataTable();
            dt.Columns.Add("盘符", typeof(string));
            dt.Columns.Add("卷标", typeof(string));
            SelectQuery selectQuery = new SelectQuery("select * from win32_logicaldisk");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(selectQuery);

            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = searcher.Get().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ManagementObject disk = (ManagementObject)enumerator.Current;
                    try
                    {
                        string DriveType = disk["DriveType"].ToString();
                        string text = DriveType;
                        if (text != null)
                        {
                            if (text == "2")
                            {
                                typeError = 1;
                                DataRow dr = dt.NewRow();
                                dr[0] = disk["Name"].ToString();
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("未知设备！" + ex);
                    }
                }
            }
            if (dt.Rows.Count > 0)
            {
                
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        string sPath = dr["盘符"].ToString();
                        string _prntrLogFile = sPath + "\\TrainLicense.dat";
                        if (File.Exists(_prntrLogFile))
                        {
                            List<string> _serialNumber = matchDriveLetterWithSerial(sPath.Substring(0, 2));
                            using (StreamReader _swPrntr = new StreamReader(_prntrLogFile, Encoding.UTF8, true))
                            {
                                string strDecrypt = _swPrntr.ReadToEnd();
                                string sJsonTrainDetail = AESDecrypt(strDecrypt, "sjhy3852");
                                if (sJsonTrainDetail.Contains("*"))
                                {
                                    string[] strTraLic = sJsonTrainDetail.Split('*');
                                    if (_serialNumber.Contains(strTraLic[1].Trim()))
                                    {
                                        tra = JsonConvert.DeserializeObject<List<TrainLicense>>(strTraLic[0]);
                                        typeError = 2;
                                    }
                                    else
                                    {
                                        MessageBox.Show(sPath + "该设备不合法！");
                                        tra = null;
                                    }
                                    //string ens = "[{\"TraBookSeqNo\":144,\"Date\":\"2015-08-18\",\"Name\":\"杨虎城\",\"PidNo\":\"610632198708172013\",\"Session\":\"\",\"AutoId\":\"B201\",\"Trainer\":\"\",\"AutoType\":\"B2\",\"ChargeMode\":\"Tries\",\"Photo\":\"\",\"Fingerprint\":\"\",\"TimeLmt\":0.33,\"TriesLmt\":1,\"MileageLmt\":0.0,\"TrainDetail\":null},{\"TraBookSeqNo\":143,\"Date\":\"2015-08-18\",\"Name\":\"张少华\",\"PidNo\":\"131121198701010230\",\"Session\":\"\",\"AutoId\":\"B201\",\"Trainer\":\"\",\"AutoType\":\"B2\",\"ChargeMode\":\"Tries\",\"Photo\":\"\",\"Fingerprint\":\"\",\"TimeLmt\":0.33,\"TriesLmt\":1,\"MileageLmt\":0.0,\"TrainDetail\":null},{\"TraBookSeqNo\":142,\"Date\":\"2015-08-18\",\"Name\":\"李腾飞\",\"PidNo\":\"131124198312070239\",\"Session\":\"\",\"AutoId\":\"B201\",\"Trainer\":\"\",\"AutoType\":\"B2\",\"ChargeMode\":\"Tries\",\"Photo\":\"\",\"Fingerprint\":\"\",\"TimeLmt\":0.33,\"TriesLmt\":1,\"MileageLmt\":0.0,\"TrainDetail\":null}]*5B830F0005CF";
                                    //string s = AESEncrypt(ens, "sjhy3852");
                                }
                                else
                                {
                                    tra = null;
                                }
                            }
                            
                            return tra;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("发生错误！" + ex);
                    }
                }
            }
            if (typeError == 0)
            {
                MessageBox.Show("没有检测到移动存储设备，请检查");
                tra = null;
            }
            else if (typeError == 1)
            {
                MessageBox.Show("读取到的设备中没有相关文件。");
                tra = null;
            }
            else
            {
                tra = null;
            }
            return tra;
        }

        public string GetLicensePath()
        {
            string sPath = "";
            List<string> dir = new List<string>();
            SelectQuery selectQuery = new SelectQuery("select * from win32_logicaldisk");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(selectQuery);
            foreach (ManagementObject disk in searcher.Get())
            {
                //驱动器类型
                string DriveType;
                try
                {
                    DriveType = disk["DriveType"].ToString();
                    switch (DriveType)
                    {
                        case "2":
                            dir.Add(disk["Name"].ToString());
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("未知设备！" + ex);
                    return sPath;
                }
            }
            foreach (string s in dir)
            {
                try
                {
                    sPath = s + "\\TrainLicense.dat";
                    if (File.Exists(sPath))
                    {
                        return s;
                    }
                    else
                        return string.Empty;
                }
                catch (Exception ex)
                {
                    throw new Exception("找不到许可路径，" + ex.Message);
                }
            }
            return "";
        }

        /// <summary>
        /// 读取移动盘符 
        /// </summary>
        /// <returns></returns>
        public DataTable ReadDisk()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("盘符", typeof(string));
            dt.Columns.Add("卷标", typeof(string));
            SelectQuery selectQuery = new SelectQuery("select * from win32_logicaldisk");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(selectQuery);

            foreach (ManagementObject disk in searcher.Get())
            {
                //驱动器类型
                string DriveType;
                try
                {
                    DriveType = disk["DriveType"].ToString();
                    if (DriveType == "2")
                    {
                        if (disk["Name"] != null && disk["Name"].ToString() != "")
                            dt.Rows.Add(disk["Name"].ToString(), disk["VolumeName"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("未知设备！" + ex);
                }
            }
            return dt;
        }
        
        /// <summary>
        /// 获取指定路径U盘序列号,sDiskPath=""时获取本机所有U盘序列号
        /// </summary>
        /// <param name="sDiskPath">U盘路径</param>
        /// <returns>返回List<string>U盘序列号</returns>
        public List<string> matchDriveLetterWithSerial(string sDiskPath)
        {
            List<string> _serialNumber = new List<string>();
            string sDiskName = "";
            var _searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDiskToPartition");
            foreach (ManagementObject dm in _searcher.Get())
            {
                sDiskName = getValueInQuotes(dm["Dependent"].ToString());
                if (sDiskName != sDiskPath && sDiskPath != "")
                    continue;
                string[] diskArray = getValueInQuotes(dm["Antecedent"].ToString()).Split(',');
                string driveNumber = diskArray[0].Remove(0, 6).Trim();
                var disks = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
                foreach (ManagementObject disk in disks.Get())
                {
                    if (disk["Name"].ToString() == ("\\\\.\\PHYSICALDRIVE" + driveNumber) & disk["InterfaceType"].ToString() == "USB")
                    {
                        _serialNumber.Add(parseSerialFromDeviceID(disk["PNPDeviceID"].ToString()));
                    }
                }
            }
            return _serialNumber;
        }


        private static string parseSerialFromDeviceID(string deviceId)
        {
            var splitDeviceId = deviceId.Split('\\');
            var arrayLen = splitDeviceId.Length - 1;
            var serialArray = splitDeviceId[arrayLen].Split('&');
            var serial = serialArray[0];
            return serial;
        }

        private static string getValueInQuotes(string inValue)
        {
            var posFoundStart = inValue.IndexOf("\"");
            var posFoundEnd = inValue.IndexOf("\"", posFoundStart + 1);
            var parsedValue = inValue.Substring(posFoundStart + 1, (posFoundEnd - posFoundStart) - 1);
            return parsedValue;
        }
        /// <summary>
        /// 创建日志文件
        /// </summary>
        /// <param name="sMsg"></param>
        public void CreateLogFileAndWriteLog(string sPath, string sMsg,string usbSerialId)
        {
            StreamWriter m_sw;
            string m_sLogFile = sPath + "\\" + "TrainLicense.dat";

            if (File.Exists(m_sLogFile))
            {
                if (!File.Exists(sPath + "\\TrainLicense" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + ".dat"))
                {
                    File.Copy(m_sLogFile, sPath + "\\TrainLicense" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + ".dat", true);
                }
            }
            using (m_sw = new StreamWriter(m_sLogFile, false, System.Text.Encoding.UTF8))
            {
                try
                {
                    if (m_sw != null)
                    {
                        sMsg = AESEncrypt(sMsg + "*" + usbSerialId, "sjhy3852");
                        m_sw.WriteLine(sMsg);
                        m_sw.Flush();
                    }
                }
                finally
                {
                    m_sw.Close();
                    m_sw.Dispose();
                }
            }
        }
        /// <summary>
        /// AES 加密(高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法)
        /// </summary>
        /// <param name="EncryptString">待加密密文</param>
        /// <param name="EncryptKey">加密密钥</param>
        /// <returns></returns>
        public string AESEncrypt(string EncryptString, string EncryptKey)
        {
            if (string.IsNullOrEmpty(EncryptString)) { throw (new Exception("密文不得为空")); }

            if (string.IsNullOrEmpty(EncryptKey)) { throw (new Exception("密钥不得为空")); }

            string m_strEncrypt = "";

            byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");

            using (Rijndael m_AESProvider = Rijndael.Create())
            {
            try
            {
                byte[] m_btEncryptString = Encoding.Default.GetBytes(EncryptString);

                MemoryStream m_stream = new MemoryStream();

                    CryptoStream m_csstream = new CryptoStream(m_stream,
                        m_AESProvider.CreateEncryptor(Encoding.Default.GetBytes(EncryptKey), m_btIV),
                        CryptoStreamMode.Write);
                m_csstream.Write(m_btEncryptString, 0, m_btEncryptString.Length); 
                m_csstream.FlushFinalBlock();
                m_strEncrypt = Convert.ToBase64String(m_stream.ToArray());
                m_stream.Close(); 
                m_stream.Dispose();
                m_csstream.Close(); 
                m_csstream.Dispose();
            }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    m_AESProvider.Clear();
                }
            }
            return m_strEncrypt;
        }
        /// <summary>
        /// AES 解密(高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法)
        /// </summary>
        /// <param name="DecryptString">待解密密文</param>
        /// <param name="DecryptKey">解密密钥</param>
        /// <returns></returns>
        public string AESDecrypt(string DecryptString, string DecryptKey)
        {
            if (string.IsNullOrEmpty(DecryptString)) { throw (new Exception("密文不得为空")); }

            if (string.IsNullOrEmpty(DecryptKey)) { throw (new Exception("密钥不得为空")); }

            string m_strDecrypt = "";

            byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");

            Rijndael m_AESProvider = Rijndael.Create();

            try
            {
                byte[] m_btDecryptString = Convert.FromBase64String(DecryptString);

                using (MemoryStream m_stream = new MemoryStream())

                {
                    using (CryptoStream m_csstream = new CryptoStream(m_stream,
                        m_AESProvider.CreateDecryptor(Encoding.Default.GetBytes(DecryptKey), m_btIV),
                        CryptoStreamMode.Write))
                    {

                m_csstream.Write(m_btDecryptString, 0, m_btDecryptString.Length); 
                m_csstream.FlushFinalBlock();

                m_strDecrypt = Encoding.Default.GetString(m_stream.ToArray());

                m_stream.Close(); 
                m_stream.Dispose();

                m_csstream.Close(); 
                m_csstream.Dispose();
            }
                }
            }
            catch (IOException ex) { throw ex; }
            catch (CryptographicException ex) { throw ex; }
            catch (ArgumentException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            finally { m_AESProvider.Clear(); }

            return m_strDecrypt;
        }
     } 
	
}
