using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBAccessProc;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Xml.Xsl;
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Data;
using TrainingAccounter;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace TrainingControl
{

    /// <summary>
    /// 驾校资源信息，如教练员、教练车、管理用户等系统相关资源。
    /// </summary>
    /// 
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        virtual internal protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class DsRsrc : DependencyObject 
    {
        public string StudyHasTime { get; set; }
        private static string[] m_BillingMode = {"按次数计费", "按时间计费", "按学时计费", "按里程计费" };
        private static string[] m_sWorkTime = { "05:00", "06:00", "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00", "24:00", "01:00", "02:00", "03:00", "04:00" };
        /// <summary>
        /// 计费模式列表
        /// </summary>
        public string[] BillingMode
        {
            get { return m_BillingMode; }
        }
        public bool IsdisplayCk { get; set; }
        public string sBillingMode
        {
            get
            {
                GetAllSetting();
                string traModel = string.Empty;
                if (TrainMangeDataSet.Tra_Setting.Rows.Count > 0)
                {
                    foreach (DataRow item in TrainMangeDataSet.Tra_Setting.Rows)
                    {
                        if (item["RS_TXT"].ToString() == "ChargingMode")
                            traModel = item["RS_VALUE"].ToString();
                    }
                }
                else
                    traModel = "";
                return traModel;
            }
        }
        public string[] WorkTime { get { return m_sWorkTime; } }//开始时间
        public static Dictionary<string, string> m_TraSettingValues = new Dictionary<string, string>();
        public static Dictionary<string, string> traDetail = new Dictionary<string, string>();
        XslCompiledTransform transfromResult = new XslCompiledTransform();
        AxLib.AxLibControl axLibCtl = null;
        public static Dictionary<string, string> TraDetail
        {
            get { return traDetail; }
            set { traDetail = value; }
        }
        private DBAccessProc.Schema.TrainManagementDataSet TrainMangeDataSet = new DBAccessProc.Schema.TrainManagementDataSet();
        public static ArrayList m_alTrainer;
        public static ArrayList m_alTrainAuto;
        public static ArrayList m_alDrvSchool;
        public static string[] m_TrainLicenseType;
        //定义依赖属性，用来绑定界面显示      
        public static readonly DependencyProperty NameProperty =
             DependencyProperty.Register("PidNo", typeof(string), typeof(DsRsrc));
        public static readonly DependencyProperty NameProperty1 =
             DependencyProperty.Register("Name", typeof(string), typeof(DsRsrc));   
        public static readonly DependencyProperty NameProperty2 =
             DependencyProperty.Register("MainBarText", typeof(string), typeof(DsRsrc));
        public static readonly DependencyProperty NameProperty3 =
             DependencyProperty.Register("Address", typeof(string), typeof(DsRsrc));
        public static readonly DependencyProperty NameProperty4 =
             DependencyProperty.Register("Photo", typeof(BitmapImage), typeof(DsRsrc));   
        public string PidNo
        {         
            get { return (string)GetValue(NameProperty); }
	        set { SetValue(NameProperty, value); } 
        }
         
        public string MainBarText
        {
            get { return (string)GetValue(NameProperty2); }
            set { SetValue(NameProperty2, value); } 
        }
      
        public string Name
        {
            get { return (string)GetValue(NameProperty1); }
            set { SetValue(NameProperty1, value); } 
        }
     
        public string Address
        {
            get { return (string)GetValue(NameProperty3); }
            set { SetValue(NameProperty3, value); } 
        }
     
        public BitmapImage Photo
        {
            get { return (BitmapImage)GetValue(NameProperty4); }
            set { SetValue(NameProperty4, value); } 
        }
       
        private bool checkDisk;
        public bool _checkDisk { get { return checkDisk; } set { value = checkDisk; } }
        //用户类型
        public bool IsManager { get; set; }
        //数据表
        public DBAccessProc.Schema.TrainManagementDataSet trainMangeDataSet
        {
            get { return TrainMangeDataSet; }
        }
        /// <summary>
        /// 基本配置信息列表
        /// </summary>
        public Dictionary<string, string> TraSettingValues
        {
            get { return m_TraSettingValues; }
            set { m_TraSettingValues = value; }
        }




        /// <summary>
        /// 教练员列表
        /// </summary>
        public ArrayList TrainerList
        {
            get { return m_alTrainer; }
        }
        /// <summary>
        /// 教练车列表
        /// </summary>
        public ArrayList TrainAutoList
        {
            get { return m_alTrainAuto; }
        }
        /// <summary>
        /// 驾校列表
        /// </summary>
        public ArrayList DrvSchoolList
        {
            get { return m_alDrvSchool; }
            set { m_alDrvSchool = value; }
        }

        /// <summary>
        /// 车型列表
        /// </summary>
        public string[] TrainLicenseType
        {
            get { return m_TrainLicenseType; }
            set { m_TrainLicenseType = value; }
        }
        ////照片处理的方法
        public BitmapImage getImageSource(byte[] buffer)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer, true);
            try
            {
                stream.Write(buffer, 0, buffer.Length);
                var newimg = new BitmapImage();
                newimg.BeginInit();
                newimg.StreamSource = new System.IO.MemoryStream(buffer);
                newimg.EndInit();
                return newimg;
            }
            catch (Exception ex)
            {
                stream.Close();
                throw new ApplicationException("显示图像错误。" + "," + ex.Message);
            }
            stream.Close();
        }

        //调用摄像头拍照
        public byte[] getPhotoFromCamra()
        {
            byte[] gbuffer = null;
            axLibCtl = new AxLib.AxLibControl();
            try
            {
                string picName = DBAccessProc.Common.TempFilePath + @"\" + System.DateTime.Now.ToString("HH-mm-ss") + ".jpg";
                axLibCtl.ps = new AxLib.PhotoSnapper(picName);
                axLibCtl.ps.ShowDialog();

                System.IO.FileInfo fi = new System.IO.FileInfo(picName);
                if (fi.Exists)
                {
                    axLibCtl.ImageBuffer(picName, ref gbuffer);
                }
                return gbuffer;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("系统错误：" + ex.Message);
                return null;
            }
        }
        public void ShowImageFromBuffer(byte[] buffer, System.Windows.Controls.Image myImg)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer, true);
            try
            {
                stream.Write(buffer, 0, buffer.Length);
                var newimg = new BitmapImage();
                newimg.BeginInit();
                newimg.StreamSource = new System.IO.MemoryStream(buffer);
                newimg.EndInit();
                // 设置 Image 控件的 Source 属性为 BitmapImage  
                myImg.Source = newimg;
            }
            catch (Exception ex)
            {
                stream.Close();
                throw new ApplicationException("显示图像错误。" + "," + ex.Message);
            }
            stream.Close();
        }

        public BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray, true);

            BitmapImage bmp = null;

            try
            {
                stream.Write(byteArray, 0, byteArray.Length);
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(byteArray);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            }

            return bmp;
        }

        //}
        //保存照片处理
        public byte[] PhotoSave(BitmapImage bmp)
        {
            using (Stream stm = bmp.StreamSource)
            {
                stm.Position = 0;
                BinaryReader br = new BinaryReader(stm);
                byte[] buff = br.ReadBytes((int)stm.Length);
                br.Close();
                return buff;
            }

        }
        /// <summary>
        /// 获取所有驾校信息
        /// </summary>
        public void getAllDrvSchool()
        {
            try
            {
                TrainMangeDataSet.DrvSchoolDataTable.Clear();
                DBAccessProc.DBAccessHelper.GetDrvSchool(TrainMangeDataSet);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("获取驾校信息出错，错误内容:" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 新增驾校
        /// </summary>
        public void AddDrvSchool(string IsUpd)
        {

            try
            {
                DBAccessProc.DBAccessHelper.SaveDrvSchool(DrvSchool.sDrvId, DrvSchool.sDrvName, DrvSchool.sDrvAddress, DrvSchool.sDrvPhoneNo, DrvSchool.sDrvDsShortName, DrvSchool.sDrvContact, IsUpd);
                if (IsUpd == "")
                    System.Windows.MessageBox.Show("保存驾校信息成功");
                else
                    System.Windows.MessageBox.Show("更新驾校信息成功");
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("保存驾校信息失败，错误信息：" + e.Message.ToString());
            }
        }


        /// <summary>
        /// 删除对应ID的驾校信息
        /// </summary>      
        public void delDrvSchool(string sDrvSchoolId)
        {
            try
            {
                if (IsRegEx("^(-?[0-9]*[.]*[0-9]{0,3})$", sDrvSchoolId))
                {
                    DBAccessProc.DBAccessHelper.DelDrvSchool(Convert.ToInt32(sDrvSchoolId));
                }
                else
                {
                    System.Windows.MessageBox.Show("删除驾校信息失败，传入的驾校ID非数字!");
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("删除驾校信息失败，错误信息：" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 判断输入的是否是数字
        /// </summary>
        public bool IsRegEx(string regExValue, string itemValue)
        {
            try
            {
                Regex regex = new System.Text.RegularExpressions.Regex(regExValue);
                if (regex.IsMatch(itemValue)) return true;
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
           
        }
        /// <summary>
        /// 获取指定学员信息
        /// </summary>
        public void getTraStudinfo(string sPidNo)
        {
            try
            {
                TrainMangeDataSet.TraineeDataTable.Clear();
                DBAccessHelper.GetTraStudInfoByPidNo(sPidNo, TrainMangeDataSet);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("获取学员信息出错，错误内容:" + e.Message.ToString());
            }
        }
        /// <summary>
        /// 查询学员信息
        /// </summary>
        /// <param name="sPidNo">身份证</param>
        /// <param name="sName">姓名</param>
        /// <param name="sSex">性别</param>
        /// <param name="sLicenseType">车型</param>
        /// <param name="sTrainer">教练员</param>
        /// <param name="drSchoolId">驾校ID</param>
        /// <param name="sBeginDt">开始时间</param>
        /// <param name="sEndDt">结束时间</param>
        public void SearchTrainee(string m_sPidNo, string m_sName, string m_sTrainerNo, string m_sAutoType, int m_sDrvSchoolId, DateTime m_sBeginDt, DateTime m_sEndDt, string isdispalyCK)
        {
            TrainMangeDataSet.TraineeDataTable.Clear();
            DBAccessHelper.GetTraStudInfo(m_sPidNo, m_sName, m_sTrainerNo, m_sAutoType, m_sDrvSchoolId, m_sBeginDt, m_sEndDt, isdispalyCK, TrainMangeDataSet);
        }
        /// <summary>
        /// 获取指定学员信息
        /// </summary>
        public void getTraStudinfoBySeqNo(double dSeqNo)
        {
            try
            {
                TrainMangeDataSet.TraineeDataTable.Clear();
                DBAccessHelper.GetTraStudInfoBySeqNo(dSeqNo, TrainMangeDataSet);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("获取学员信息出错，错误内容:" + e.Message.ToString());
            }
        }
        /// <summary>
        /// 获取指定学员许可信息
        /// </summary>
        public void getTraTrainLicense(string sBookSeqNo, string sPidNo)
        {
            try
            {
                TrainMangeDataSet.TraTrainLicenseDataTable.Clear();
                DBAccessHelper.getTrainLicense(int.Parse(sBookSeqNo), sPidNo, TrainMangeDataSet);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("获取学员许可信息出错，错误内容:" + e.Message.ToString());
            }
        }


        /// <summary>
        /// 获取训练过程信息
        /// </summary>
        public void getTraProcessInfo(string sExamCd, string CarType, string sTrainerId, int iDrvSchoolId, string dStartTs, string dEndTs, string TraName, string sPidNo, string EiNo, string VlNo, string traSisModel, string autoid, int traBookSeqNo)
        {
            try
            {
                TrainMangeDataSet.TraProcessInfoDataTable.Clear();
                DBAccessHelper.GetTraProcess(sExamCd, CarType, sTrainerId, iDrvSchoolId, dStartTs, dEndTs, TraName, sPidNo, autoid, traBookSeqNo, TrainMangeDataSet);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("获取学员训练信息出错，错误内容:" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 获取训练过程信息
        /// </summary>
        public void getTraProcessInfo(string sExamCd, string CarType, string sTrainerId, int iDrvSchoolId, string dStartTs, string dEndTs, string TraName, string sPidNo, string autoid, int traBookSeqNo)
        {
            try
            {
                TrainMangeDataSet.TraProcessInfoDataTable.Clear();
                DBAccessHelper.GetTraProcess(sExamCd, CarType, sTrainerId, iDrvSchoolId, dStartTs, dEndTs, TraName, sPidNo, autoid, traBookSeqNo, TrainMangeDataSet);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("获取学员训练信息出错，错误内容:" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 获取训练扣分详细信息
        /// </summary>
        public void getTraProcessInfoPoints(int seqNo)
        {
            try
            {
                TrainMangeDataSet.TraProcessPointsDataTable.Clear();
                DBAccessHelper.GetTraProcessPoints(seqNo, TrainMangeDataSet);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("获取学员训练信息出错，错误内容:" + e.Message.ToString());
            }
        }
        /// <summary>
        /// 获取所有训练车辆信息
        /// </summary>
        public void getAllTraCar()
        {
            try
            {
                TrainMangeDataSet.TraCarDataTable.Clear();
                DBAccessProc.DBAccessHelper.GetTraCar(TrainMangeDataSet);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("获取训练车信息出错，错误内容:" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 获取所有教练员
        /// </summary>
        public void GetAllTrainer()
        {
            try
            {
                TrainMangeDataSet.TrainerDataTable.Clear();
                DBAccessProc.DBAccessHelper.GetTrainer(TrainMangeDataSet);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("获取教练员信息出错，错误内容:" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 保存教练员
        /// </summary>
        public void AddTrainer(string wform)
        {
            try
            {
                DBAccessProc.DBAccessHelper.SaveTrainer(Trainer.sTrainerId, Trainer.sTrainerName, "", Trainer.sTrainerPhone, Trainer.sTrainerImage, "", "", Trainer.sTrainerCtype, DateTime.Now, "", Trainer.sTrainerCarid, Convert.ToInt32(Trainer.sTrainerDrv), Trainer.sTrainerPidNo, wform);
                if (wform == "")
                    System.Windows.Forms.MessageBox.Show("保存教练员信息成功！");
                else
                    System.Windows.Forms.MessageBox.Show("更新教练员信息成功！");
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("保存教练员信息出错，错误内容:" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 保存教练车
        /// </summary>
        public void AddTraCar(string wform)
        {
            try
            {
                DBAccessProc.DBAccessHelper.SaveTraCarInfo(TrainAuto.sCarNo, TrainAuto.sCarType, TrainAuto.sCarName, TrainAuto.sCarBrand, "", "", Convert.ToDouble(TrainAuto.sCarLength), Convert.ToDouble(TrainAuto.sCarWidth), Convert.ToDouble(TrainAuto.sCarHeight), TrainAuto.sPlateNo, "", TrainAuto.sCarStatus, TrainAuto.sCarHostIp, TrainAuto.sCarVideoIp, TrainAuto.sCarTrainer, wform);
                if (wform == "")
                    System.Windows.Forms.MessageBox.Show("保存教练车信息成功！");
                else
                    System.Windows.Forms.MessageBox.Show("更新教练车信息成功！");
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("保存教练车信息出错，错误内容:" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 删除对应编号的训练车信息
        /// </summary>      
        public void delTraCar(string sTraCarNo)
        {
            try
            {
                DBAccessProc.DBAccessHelper.DelTraCarNo(sTraCarNo);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("删除教练车信息失败，错误信息：" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 删除对应编号的教练员信息
        /// </summary>      
        public void delTrainer(string sTrainerNo)
        {
            try
            {
                DBAccessProc.DBAccessHelper.DelTrainer(sTrainerNo);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("删除教练员信息失败，错误信息：" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        public void GetAllUser()
        {
            try
            {
                TrainMangeDataSet.TraManagerDataTable.Clear();
                DBAccessProc.DBAccessHelper.GetTraManager(TrainMangeDataSet);

            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("获取用户信息出错，错误内容:" + e.Message.ToString());
            }
        }


        /// <summary>
        /// 保存用户信息
        /// </summary>
        public void AddTraManager(string wform)
        {
            try
            {
                DBAccessProc.DBAccessHelper.SaveTraManager(TraUser.sUserId, TraUser.sUserPwd, TraUser.sUserName, "", TraUser.sUserPhone, TraUser.sUserPhoto, "", "", "", TraUser.sUserType, wform);
                if (wform == "")
                    System.Windows.Forms.MessageBox.Show("保存用户信息成功！");
                else
                    System.Windows.Forms.MessageBox.Show("更新用户信息成功！");

            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("保存用户信息出错，错误内容:" + e.Message.ToString());
                return;
            }
        }

        /// <summary>
        /// 删除对应编号的用户
        /// </summary>      
        public void delTraManager(string sTraManagerId)
        {
            try
            {
                DBAccessProc.DBAccessHelper.DelTraMnaager(sTraManagerId);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("删除用户信息失败，错误信息：" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 获取基本配置信息
        /// </summary>
        public void GetAllSetting()
        {
            try
            {
                TrainMangeDataSet.Tra_Setting.Clear();
                TraSettingValues.Clear();
                DBAccessProc.DBAccessHelper.GetTraSetting(TrainMangeDataSet);
                foreach (DataRow row in TrainMangeDataSet.Tra_Setting.Rows)
                {
                    if (row["RS_TXT"].ToString() == "CarType")
                        m_TrainLicenseType = row["RS_VALUE"].ToString().Split(',');
                    TraSettingValues.Add(row["RS_TXT"].ToString(), row["RS_VALUE"].ToString());
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("获取基本信息出错，错误内容:" + e.Message.ToString());
            }
        }
        /// <summary>
        /// 保存基本配置信息
        /// </summary>
        public void AddTraSetting()
        {
            try
            {
                foreach (string keys in TraSettingValues.Keys)
                {
                    var temp = TraSettingValues[keys].Split('|');
                    if (temp.Length > 1)
                    {
                        DBAccessProc.DBAccessHelper.SaveTraSetting(keys, temp[0], temp[1], temp[2]);
                    }
                }
                System.Windows.MessageBox.Show("保存基本信息成功");
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("保存基本信息出错，错误内容:" + e.Message.ToString());
            }
        }


        /// <summary>
        /// 获取考生预约信息
        /// </summary>
        public void GetTraBook(string sPidNo, string traname, DateTime dbookts, DateTime dbookes)
        {
            try
            {
                TrainMangeDataSet.BookTrainingDataTable.Clear();
                DBAccessProc.DBAccessHelper.GetTraBookByRange(sPidNo, traname, dbookts, dbookes, DBAccessProc.Common.RunningMode, TrainMangeDataSet);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("获取考生基本信息出错，错误内容:" + e.Message.ToString());
            }
        }
        /// <summary>
        /// 获取考生今日预约信息
        /// </summary>
        public void GetTraBookToday(string sPidNo, string traname, DateTime dbookts, DateTime dbookes)
        {
            try
            {
                TrainMangeDataSet.BookTrainingDataTable.Clear();
                DBAccessProc.DBAccessHelper.GetTraBookToday(sPidNo, traname, dbookts, dbookes, DBAccessProc.Common.RunningMode, TrainMangeDataSet);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("获取考生基本信息出错，错误内容:" + e.Message.ToString());
            }
        }
        /// <summary>
        /// 通过SEQNO获取单个考生预约信息
        /// </summary>
        public void GetTraBookBySeqNo(double SeqNo)
        {
            try
            {
                TrainMangeDataSet.BookTrainingDataTable.Clear();
                DBAccessProc.DBAccessHelper.GetTraBookBySeqNo(SeqNo, DBAccessProc.Common.RunningMode, TrainMangeDataSet);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("通过序号获取考生基本信息出错，错误内容:" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 保存预约信息
        /// </summary>
        public void AddTraBookInfo(string sIsupd)
        {
            try
            {
                DBAccessProc.DBAccessHelper.SaveTraBookInfo(TraBook.sExamCd, TraBook.sPidNo, TraBook.sName, TraBook.dThisBalance, "", TraBook.dTrainDt, TraBook.sSession, TraBook.sTrainer, TraBook.sCarType, TraBook.sCarBrand, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "A", TraBook.sTraCarPlateNo, TraBook.sTraCarNo, Convert.ToDouble(TraBook.sTraAmount), "UK", sIsupd, Convert.ToDouble(TraBook.sBalance), DBAccessProc.Common.RunningMode, TraBook.nTriesAmount, TraBook.dMileageAmount, TraBook.sBillMode);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("保存预约信息出错，错误内容:" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 进行签到动作
        /// </summary>
        public int TraCheckIn(string sSeqNo, string sCheckedstatus, string pidNo, string diskId, double balance)
        {
            try
            {
                int result = DBAccessProc.DBAccessHelper.TraineeCheckIn(sSeqNo, sCheckedstatus, pidNo, diskId, balance);
                return result;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("签到出错，错误内容:" + e.Message.ToString());
                return -1;
            }

        }
        /// <summary>
        /// 取消预约
        /// </summary>
        public void TraBookCancel(string sSeqNo)
        {
            try
            {
                DBAccessProc.DBAccessHelper.DelTraBookInfo(sSeqNo);
                System.Windows.MessageBox.Show("取消预约成功！");
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("取消预约出错，错误内容:" + e.Message.ToString());
            }
        }
        /// <summary>
        /// 进行充值
        /// </summary>
        public void RechargeAccount(string sSeqNo, double Balance, string PidNo, string sRecharge_Per, string sReson)
        {
            try
            {
                DBAccessProc.DBAccessHelper.rechargeAccount(sSeqNo, Balance, PidNo, sRecharge_Per, sReson);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("充值出现错误，错误内容:" + e.Message.ToString());
                return;
            }
        }
        public void InsRechargeRecord(double Balance, string sPidNo, string sRecharge_Per, string sReason)
        {
            DBAccessHelper.InsRechargeRecord(Balance, sPidNo, sRecharge_Per, sReason);
        }
        /// <summary>
        /// 获取训练许可
        /// </summary>
        public void getTrainLicense(string sBookSeqNo, string sPidNo)
        {
            try
            {
                TrainMangeDataSet.TraTrainLicenseDataTable.Clear();
                DBAccessProc.DBAccessHelper.getTrainLicense(int.Parse(sBookSeqNo), sPidNo, TrainMangeDataSet);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("获取许可信息失败，错误内容:" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 获取该车型计费标准
        /// </summary>
        /// <param name="sChargeMode">计费模式</param>
        /// <param name="sCarType">车型</param>
        /// <returns></returns>
        public double GetChargingStandard(string sChargeMode, string sCarType)
        {
            string sChargingStandard = "";
            double dChargingStandard = 0;

            string sRxTxt = "ChargingStandard" + sCarType;
            string sRxtStudy = "StudyHasTime" + sCarType;
            GetAllSetting();
            try
            {
                sChargingStandard = TraSettingValues[sRxTxt].ToString();
                string sStudyHas = TraSettingValues[sRxtStudy].ToString();
                string[] sChSt = sChargingStandard.Split('*');
                for (int i = 0; i < sChSt.Length; i++)
                {

                    var standarry = sChSt[i].Split(':');
                    if (sChargeMode == standarry[0].ToString())
                    {
                        dChargingStandard = Double.Parse(standarry[1].ToString());
                    }
                }
                this.StudyHasTime = sStudyHas;
                return dChargingStandard;
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(sCarType + "车型的计费标准还未配置！");
                return 0;
            }
            return dChargingStandard;
        }


        ///<summary>
        ///结算方法
        ///</summary>
        public List<Double> settlementByStu(Double dChargingStandard/*收费标准*/, Double actualTrainings/*当前训练总量*/, Double bookAmount/*预约金额*/, Double accountBalance/*预约结束后的账户余额*/)
        {
            List<Double> resultSettlement = new List<double>();
            var bookBalance = Math.Round(bookAmount - dChargingStandard * actualTrainings, 2);
            var actualBalance = Math.Round(bookBalance + accountBalance, 2);
            resultSettlement.Add(bookBalance);
            resultSettlement.Add(actualBalance);
            return resultSettlement;
        }

        /// <summary>
        /// 设置学员类型：0=普通学员、1=VIP1、2=VIP2、9=教练员、99管理员
        /// </summary>	
        public void SetStudentType(string pidNo, int stuType)
        {
            try
            {
                DBAccessProc.DBAccessHelper.setStudentType(pidNo, stuType);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("充值出现错误，错误内容:" + e.Message.ToString());
                return;
            }
        }
        /// <summary>
        /// 打印相关
        /// </summary>
        /// <param name="sXslFile"></param>
        /// <param name="printFiles"></param>
        public string PrintDoc(string sXslFile, string printFiles, int id)
        {
            string htmlFileName = string.Empty;
            try
            {
                if (id == 0)
                {
                    htmlFileName = printFiles + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + ".html";
                }
                else
                {
                    htmlFileName = printFiles + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + "detail.html";
                }
                FileInfo htmlFile = new FileInfo(htmlFileName);
                if (htmlFile.Exists)
                {
                    if (System.Windows.MessageBox.Show(@"统计单已经存在，是否重新生成？", "提示",
                        System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Information) == System.Windows.MessageBoxResult.OK)
                    {
                        htmlFile.Delete();
                    }
                    else
                    {
                        return htmlFileName;
                    }
                }
                //generate report
                string reportFileName = "";
                DataView dview = null;
                DBAccessProc.Schema.TrainManagementDataSet dataset = new DBAccessProc.Schema.TrainManagementDataSet();
                if (id == 0)
                {
                    dview = trainMangeDataSet.TraProcessInfoDataTable.DefaultView;
                    //dview.RowFilter = "PID_NO='" + sPidNo + "'AND EXAMID_NO='" + sExamidNo + "'AND EXAM_DT = '" +
                    //				  sExamDt + "'";
                    dview.Sort = "seq_no asc";
                    DataTable dv = dview.ToTable();
                    for (int i = 0; i < dv.Rows.Count; i++)
                    {
                        dataset.TraProcessInfoDataTable.ImportRow(dv.Rows[i]);
                    }
                }
                else
                {
                    dview = trainMangeDataSet.TraProcessPointsDataTable.DefaultView;
                    //dview.RowFilter = "PID_NO='" + sPidNo + "'AND EXAMID_NO='" + sExamidNo + "'AND EXAM_DT = '" +
                    //				  sExamDt + "'";
                    dview.Sort = "seq_no asc";
                    DataTable dv = dview.ToTable();
                    for (int i = 0; i < dv.Rows.Count; i++)
                    {
                        dataset.TraProcessPointsDataTable.ImportRow(dv.Rows[i]);
                    }
                }

                //PrintFormatController pfc = new PrintFormatController(m_sDBConn, m_sFilePath, seqNo, sExamidNo, sPidNo, "RE", "", nPrintFormatOption);
                var pfc = new PrintFormatController(printFiles, ref dataset);
                if (pfc != null)
                {
                    reportFileName = pfc.GePrtKm2ExamResultHtml(sXslFile, "统计表", id);
                }
                return reportFileName;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, @"系统错误");
                return "";
            }
        }



        ///<summary>
        ///检测注册表是否注册控件  没有则注册
        ///</summary>
        ///
        public bool RegisterControls(string configName)
        {
            Dictionary<string, RegistryKey> dataReg = new Dictionary<string, RegistryKey>();
            string AppPath = Environment.CurrentDirectory;//System.Windows.Forms.Application.StartupPath;          
            Process proc = null;
            try
            {
                //检测注册表中是否注册
                dataReg = new Dictionary<string, RegistryKey>();
                dataReg.Add("SDK_2000.OCX", Registry.ClassesRoot.OpenSubKey(@"TypeLib\{C7C2AF82-CF1A-496E-823E-F2ED7F75F1D0}"));
                dataReg.Add("AvcaputureCtrl.ocx", Registry.ClassesRoot.OpenSubKey(@"Interface\{1C57617D-004C-46DA-B591-D8C078725C18}"));
                dataReg.Add("DoronUruActiveX4B.ocx", Registry.ClassesRoot.OpenSubKey(@"TypeLib\{45149C56-5D4C-4D85-8063-F58369A53BDA}"));
                dataReg.Add("biokey.ocx", Registry.ClassesRoot.OpenSubKey(@"TypeLib\{01640000-5D7D-11D3-9268-8EEA2836FD6D}"));
                dataReg.Add("SmartPainter.ocx", Registry.ClassesRoot.OpenSubKey(@"TypeLib\{88494371-9563-4065-AA92-86D6DFF422A2}"));//SmartPainter
                dataReg.Add("SmartPainterQ.ocx", Registry.ClassesRoot.OpenSubKey(@"TypeLib\{C122FDD0-61F7-439E-B4EC-DCD6374F0923}"));//SmartPainterQ.ocx
                dataReg.Add("TCMFPCtl.ocx", Registry.ClassesRoot.OpenSubKey(@"Interface\{33B5FA24-8FAC-4937-81CB-41330AFC3499}")); //TCMFPCtl
                dataReg.Add("Termb.ocx", Registry.ClassesRoot.OpenSubKey(@"Interface\{61A4FAE4-8E35-484D-AE27-1451675B5C34}"));//Termb.ocx                             
                proc = new Process();
                foreach (string key in dataReg.Keys)
                {
                    if (dataReg[key] == null)
                    {
                        if (!File.Exists(AppPath + @"/Refrence/" + key + ""))
                        {
                            System.Windows.MessageBox.Show("找不到注册文件，路径：@" + AppPath + "/Refrence/" + key + "");
                            return false;
                        }
                        if (key == "Termb.ocx" || key == "TCMFPCtl.ocx" || key == "SDK_2000.OCX")
                        {
                            proc = new Process();
                            proc.StartInfo.FileName = AppPath + @"/Refrence/" + key.Substring(0, key.IndexOf('.')) + ".bat";
                            proc.StartInfo.CreateNoWindow = true;
                            proc.StartInfo.UseShellExecute = false;
                            proc.Start();
                            proc.WaitForExit();
                            proc.Close();
                        }
                        else
                        {
                            proc.StartInfo.FileName = "Regsvr32.exe";
                            proc.StartInfo.Arguments = "/s " + AppPath + "/Refrence/" + key + "";
                            proc.Start();
                            proc.WaitForExit();
                            proc.Close();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(string.Format("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString()));
                return false;
            }
        }

        //显示预约历史信息
        public DataTable displayBookhistory(string sPidNo, string traname, DateTime dbookts, DateTime dbookes)
        {
            GetTraBook(sPidNo, traname, dbookts, dbookes);
            DataTable dthistory = TrainMangeDataSet.BookTrainingDataTable.Clone();
            if (TrainMangeDataSet.BookTrainingDataTable.Rows.Count > 0)
            {
                dthistory = TrainMangeDataSet.BookTrainingDataTable.Clone();
                DataRow[] drowhistory = TrainMangeDataSet.BookTrainingDataTable.Select(null, "BOOK_TIME DESC");
                foreach (DataRow dr in drowhistory)
                {
                    switch (dr["CHECKSTATUS"].ToString())
                    {
                        case "UK":
                            dr["CHECKSTATUS"] = "未签到";
                            break;
                        case "CK":
                            dr["CHECKSTATUS"] = "已签到";
                            break;
                        case "IK":
                            dr["CHECKSTATUS"] = "训练中";
                            break;
                        case "EK":
                            dr["CHECKSTATUS"] = "已结束";
                            break;
                        case "SK":
                            dr["CHECKSTATUS"] = "已结算";
                            break;
                        case "RK":
                            dr["CHECKSTATUS"] = "已取消";
                            break;
                    }
                    if (dr["BILL_MODE"].ToString() == "Time")
                        dr["RECHARGE_AMOUNT"] = dr["RECHARGE_AMOUNT"] + " 小时";
                    else if (dr["BILL_MODE"].ToString() == "Tries")
                        dr["RECHARGE_AMOUNT"] = dr["TRIES_AMOUNT"] + " 次";
                    else if (dr["BILL_MODE"].ToString() == "Mileage")
                        dr["RECHARGE_AMOUNT"] = dr["MILEAGE_AMOUNT"] + " 公里";
                    dthistory.Rows.Add(dr.ItemArray);
                }
            }
            else
                dthistory = null;
            return dthistory;
        }

        ///<summary>
        ///更新学员类型
        ///</summary>
        public bool updStuType(string pidno, int type)
        {
            bool result = true;
            try
            {
                DBAccessHelper.setStudentType(pidno, type);
            }
            catch (Exception ex)
            {
                result = false;
                System.Windows.MessageBox.Show("更新学员类型出错，错误信息：" + ex.Message.ToString());
            }
            return result;
        }
        List<TrainLicense> lstTraData = new List<TrainLicense>();
        public const int WM_DEVICECHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;
        public const int DBT_CONFIGCHANGED = 0x0018;
        public const int DBT_CUSTOMEVENT = 0x8006;
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;
        public const int DBT_DEVNODES_CHANGED = 0x0007;
        public const int DBT_QUERYCHANGECONFIG = 0x0017;
        public const int DBT_USERDEFINED = 0xFFFF;
        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            try
            {
                if (msg == WM_DEVICECHANGE)
                {
                    switch (wParam.ToInt32())
                    {
                        case WM_DEVICECHANGE:
                            break;
                        case DBT_DEVICEARRIVAL://U盘插入
                            var s = DriveInfo.GetDrives();
                            foreach (var drive in s)
                            {
                                if (drive.DriveType == DriveType.Removable)
                                {
                                    //读取U盘线程
                                    try
                                    {
                                        Thread thdReadDisk = new Thread(new ParameterizedThreadStart(GetDiskTraData));
                                        thdReadDisk.IsBackground = true;
                                        thdReadDisk.Start(drive.Name);
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Windows.MessageBox.Show("读取U盘数据错误，错误信息：" + ex.Message.ToString());
                                    }
                                    //break;
                                }
                            }
                            break;
                        case DBT_CONFIGCHANGECANCELED:
                            break;
                        case DBT_CONFIGCHANGED:
                            break;
                        case DBT_CUSTOMEVENT:
                            break;
                        case DBT_DEVICEQUERYREMOVE:
                            break;
                        case DBT_DEVICEQUERYREMOVEFAILED:
                            break;
                        case DBT_DEVICEREMOVECOMPLETE: //U盘卸载   
                            break;
                        case DBT_DEVICEREMOVEPENDING:
                            break;
                        case DBT_DEVICETYPESPECIFIC:
                            break;
                        case DBT_DEVNODES_CHANGED:
                            break;
                        case DBT_QUERYCHANGECONFIG:
                            break;
                        case DBT_USERDEFINED:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            return IntPtr.Zero;
        }
        private void GetDiskTraData(object diskName)
        {
            string devName = diskName as string;
            var result = readTraProcess(devName);
            if (result == null) return;

        }
        public List<TrainLicense> readTraProcess(string devname)
        {
            try
            {
                lstTraData.Clear();
                TrainingControl.ReadAndWrite readJStoSql = new ReadAndWrite();
                var readResult = readJStoSql.ReadTrainLicense(devname);
                if (readResult != null)
                {
                    for (int i = 0; i < readResult.Count; i++)
                    {
                        //更新状态和余额到主账户

                        var result = DBAccessHelper.TraineeCheckIn(readResult[i].TraBookSeqNo.ToString(), readResult[i].CheckStatus, readResult[i].PidNo, readJStoSql.StrWholeRead[1].ToString(), double.Parse(readResult[i].AccountBalance.ToString()));
                        if (result != 1)
                        {
                            System.Windows.MessageBox.Show("更新账户余额失败，U盘ＩＤ不匹配或者数据不正确,请重试");
                            return null;
                        }
                        if (readResult[i].TrainDetail == null)
                        {
                            continue;
                        }
                        DateTime startTime = readResult[i].TrainDetail[0].TrainStartTs;
                        DateTime endTime = readResult[i].TrainDetail[0].TrainEndTs;
                        string autoId = string.Empty;
                        string trainer = string.Empty;
                        double traTime = 0;
                        int trainTries = 0;
                        double trainMileage = 0;
                        foreach (var item in readResult[i].TrainDetail)
                        {
                            if (item.TrainEndTs < item.TrainStartTs)
                                item.TrainEndTs = item.TrainStartTs.AddHours(item.TrainTime);
                            autoId = item.AutoId;
                            trainer = item.Trainer == null ? "" : item.Trainer;
                            traTime += item.TrainTime;
                            trainTries += item.TrainTries;
                            trainMileage += item.TrainMileage;
                            if (item.TrainEndTs > endTime)
                                endTime = item.TrainEndTs;
                            if (startTime > item.TrainStartTs)
                                startTime = item.TrainStartTs;

                            var traProcSeqNo = DBAccessHelper.SaveTraProcessInfo(autoId, readResult[i].PidNo, readResult[i].Date, startTime, endTime, trainer, readResult[i].AutoType, traTime, readResult[i].TraBookSeqNo, trainTries, trainMileage);
                            if (traProcSeqNo == -1)
                            {
                                continue;
                            }
                            else
                            {                               
                                    foreach (var proc in item.TrainProcList)
                                    {
                                        DBAccessHelper.SaveTraProcessPoints(traProcSeqNo, proc.Code, DateTime.Parse(proc.Timestamp), proc.Mode, proc.Type);
                                    }
                                
                            }

                        }
                       
                        lstTraData.Add(readResult[i]);
                    }
                }
                else
                {
                   System.Windows.MessageBox.Show("读取的数据为NULL，请检查设备中是否有需要的数据文件。");
                    return null;
                }
                return lstTraData;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("保存训练过程信息到数据库错误：" + ex.Message.ToString());
                return null;
            }
        }
		
      
    }

    /// <summary>
    /// 教练员
    /// </summary>
    public class Trainer
    {

        public static string sTrainerId;
        public static string sTrainerName;
        public static string sTrainerDrv;
        public static string sTrainerPhone;
        public static string sTrainerPidNo;
        public static string sTrainerCtype;
        public static string sTrainerCarid;
        public static byte[] sTrainerImage;


        public string TrainerCtype
        {
            get { return sTrainerCtype; }
            set { sTrainerCtype = value; }
        }
        public string TrainerCarid
        {
            get { return sTrainerCarid; }
            set { sTrainerCarid = value; }
        }
        public byte[] TrainerImage
        {
            get { return sTrainerImage; }
            set { sTrainerImage = value; }
        }
        public string TrainerId
        {
            get { return sTrainerId; }
            set { sTrainerId = value; }
        }
        public string TrainerName
        {
            get { return sTrainerName; }
            set { sTrainerName = value; }
        }
        public string TrainerDrv
        {
            get { return sTrainerDrv; }
            set { sTrainerDrv = value; }
        }
        public string TrainerPhone
        {
            get { return sTrainerPhone; }
            set { sTrainerPhone = value; }
        }
        public string TrainerPidNo
        {
            get { return sTrainerPidNo; }
            set { sTrainerPidNo = value; }
        }

    }

    /// <summary>
    /// 教练车
    /// </summary>
    public class TrainAuto
    {
        public static string sCarType;
        public static string sCarStatus;
        public static string sCarTrainer;
        public static string sCarNo;
        public static string sPlateNo;
        public static string sCarName;
        public static string sCarBrand;
        public static string sCarLength;
        public static string sCarWidth;
        public static string sCarHeight;
        public static string sCarHostIp;
        public static string sCarVideoIp;

        public string CarType
        {
            get { return sCarType; }
            set { sCarType = value; }
        }
        public string CarStatus
        {
            get { return sCarStatus; }
            set { sCarStatus = value; }
        }
        public string CarTrainer
        {
            get { return sCarTrainer; }
            set { sCarTrainer = value; }
        }
        public string CarNo
        {
            get { return sCarNo; }
            set { sCarNo = value; }
        }
        public string PlateNo
        {
            get { return sPlateNo; }
            set { sPlateNo = value; }
        }
        public string CarName
        {
            get { return sCarName; }
            set { sCarName = value; }
        }
        public string CarBrand
        {
            get { return sCarBrand; }
            set { sCarBrand = value; }
        }
        public string CarLength
        {
            get { return sCarLength; }
            set { sCarLength = value; }
        }
        public string CarWidth
        {
            get { return sCarWidth; }
            set { sCarWidth = value; }
        }
        public string CarHeight
        {
            get { return sCarHeight; }
            set { sCarHeight = value; }
        }
        public string CarHostIp
        {
            get { return sCarHostIp; }
            set { sCarHostIp = value; }
        }
        public string CarVideoIp
        {
            get { return sCarVideoIp; }
            set { sCarVideoIp = value; }
        }
    }

    /// <summary>
    /// 驾校（学员所属驾校）
    /// </summary>
    public class DrvSchool
    {
        public static int sDrvId;
        public static string sDrvName;
        public static string sDrvAddress;
        public static string sDrvPhoneNo;
        public static string sDrvDsShortName;
        public static string sDrvContact;


        public int DrvId
        {
            get { return sDrvId; }
            set { sDrvId = value; }
        }
        public string drvname
        {
            get { return sDrvName; }
            set { sDrvName = value; }
        }
        public string drvaddress
        {
            get { return sDrvAddress; }
            set { sDrvAddress = value; }
        }
        public string drvphoneno
        {
            get { return sDrvPhoneNo; }
            set { sDrvPhoneNo = value; }
        }
        public string drvshortname
        {
            get { return sDrvDsShortName; }
            set { sDrvDsShortName = value; }
        }
        public string drvcontact
        {
            get { return sDrvContact; }
            set { sDrvContact = value; }
        }
    }
    /// <summary>
    /// 用户信息
    /// </summary>
    public class TraUser
    {
        public static string sUserName;
        public static string sUserPwd;
        public static string sUserId;
        public static string sUserPhone;
        public static string sUserType;
        public static byte[] sUserPhoto;

        public string UserName
        {
            get { return sUserName; }
            set { sUserName = value; }
        }
        public string UserPwd
        {
            get { return sUserPwd; }
            set { sUserPwd = value; }
        }
        public string UserId
        {
            get { return sUserId; }
            set { sUserId = value; }
        }
        public string UserPhone
        {
            get { return sUserPhone; }
            set { sUserPhone = value; }
        }
        public string UserType
        {
            get { return sUserType; }
            set { sUserType = value; }
        }
        public byte[] UserPhoto
        {
            get { return sUserPhoto; }
            set { sUserPhoto = value; }
        }
    }

    /// <summary>
    /// 预约信息
    /// </summary>
    public class TraBook
    {
        public static string sExamCd;
        public static string sPidNo;
        public static string sName;
        public static string sDrvId;
        public static string sPhoneNo;
        public static string dTrainDt;
        public static string sSession;
        public static string sBillMode;
        public static string sTraMode;
        public static string sTrainer;
        public static string sTraAmount;
        public static string sCarType;
        public static string sBalance;
        public static string sCarBrand;
        public static string sTraSubject;
        public static string sTraCarNo;
        public static string sTraCarPlateNo;
        public static int nTriesAmount;
        public static float dMileageAmount;
        public static double dThisBalance;

        public string ExamCd
        {
            get { return sExamCd; }
            set { sExamCd = value; }
        }
        public string PidNo
        {
            get { return sPidNo; }
            set { sPidNo = value; }
        }
        public string Name
        {
            get { return sName; }
            set { sName = value; }
        }
        public string DrvId
        {
            get { return sDrvId; }
            set { sDrvId = value; }
        }
        public string PhoneNo
        {
            get { return sPhoneNo; }
            set { sPhoneNo = value; }
        }
        public string TrainDt
        {
            get { return dTrainDt; }
            set { dTrainDt = value; }
        }
        public string Session
        {
            get { return sSession; }
            set { sSession = value; }
        }
        public string BillMode
        {
            get { return sBillMode; }
            set { sBillMode = value; }
        }
        public string TraMode
        {
            get { return sTraMode; }
            set { sTraMode = value; }
        }
        public string TrainerNames
        {
            get { return sTrainer; }
            set { sTrainer = value; }
        }

        public string TraAmount
        {
            get { return sTraAmount; }
            set { sTraAmount = value; }
        }
        public string CarType
        {
            get { return sCarType; }
            set { sCarType = value; }
        }
        public string Balance
        {
            get { return sBalance; }
            set { sBalance = value; }
        }
        public string CarBrand
        {
            get { return sCarBrand; }
            set { sCarBrand = value; }
        }
        public string TraSubject
        {
            get { return sTraSubject; }
            set { sTraSubject = value; }
        }
        public string TraCarNo
        {
            get { return sTraCarNo; }
            set { sTraCarNo = value; }
        }
        public string TraCarPlateNo
        {
            get { return sTraCarPlateNo; }
            set { sTraCarPlateNo = value; }
        }
        /// <summary>
        /// 计次数量
        /// </summary>
        public int TriesAmount
        {
            get { return nTriesAmount; }
            set { nTriesAmount = value; }
        }
        /// <summary>
        /// 计里程数量
        /// </summary>
        public float MileageAmount
        {
            get { return dMileageAmount; }
            set { dMileageAmount = value; }
        }
        /// <summary>
        /// 本次预约费用
        /// </summary>
        public double ThisBalance
        {
            get { return dThisBalance; }
            set { dThisBalance = value; }
        }
    }

}



