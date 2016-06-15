using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccessProc;
using DBAccessProc.Schema;
using System.Data;
using System.Windows;
using System.IO;

namespace TrainingControl
{
    ///
    /// 存储读卡信息
    ///
    public class saveCardInfo
    {
        static string sName;
        static string sPidNo;
        static string sFingerprint;
        static byte[] sPhoto;
        public string sCardName
        {
            get { return sName; }
            set {value =sName;}
        }
        public string sCardPid
        {
            get { return sPidNo; }
            set { value = sPidNo; }
        }
        public string sCardFingerprint
        {
            get { return sFingerprint; }
            set { value = sFingerprint; }
        }
        public  byte[] sCardPhoto
        {
            get { return sPhoto; }
            set { value = sPhoto; }
        }
    }
    /// 
    /// <summary>
    /// 学员信息
    /// </summary>
    public class Trainee
    {
		DsRsrc dsrsrc = new DsRsrc();
        TrainManagementDataSet m_trainMangeDataSet = new TrainManagementDataSet();
        //控件绑定
        private static string[] m_sArrSex = { "", "男", "女" };
        /// <summary>
        /// 性别,绑定显示
        /// </summary>
        public string[] SexArr
        {
            get { return m_sArrSex; }
            set { m_sArrSex = value; }
        }
        public TrainManagementDataSet TrainMangeDataSet
        {
            get { return m_trainMangeDataSet; }
        }
    /// <summary>
    /// 学员基本信息
    /// </summary>
        
        static string m_sName;
        static string m_sPidNo;
        static string m_sTrainer;
        static string m_sTrainerNo;
        static string m_sDrvSchool;
        static int m_sDrvSchoolId;
        static string m_sFingerprint;
        static string m_sPhoneNo;
        static byte[] m_sPhoto;
        static DateTime m_sBirthDate;
        //static DateTime m_sRegDate;
        static string m_sRegDate;
        static string m_sHomeAddress;
        static string m_sAutoType;
        static string m_sSex;
        static double m_nBalance;
        static string m_sFillPerson;
        static string m_sTrainingMode;
        static DateTime m_sBeginDt;
        static DateTime m_sEndDt;
        #region
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string PidNo
        {
            get { return m_sPidNo; }
            set { m_sPidNo = value; }
        }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return m_sName; }
            set { m_sName = value; }
        }
        /// <summary>
        /// 账户余额
        /// </summary>
        public double Balance
        {
            get { return m_nBalance; }
            set { m_nBalance = value; }
        }
        /// <summary>
        /// 账户充值
        /// </summary>
        /// <param name="amount">充值金额</param>
        /// <returns>账户余额</returns>
        public double RechargeAccount(double amount)
        {
            return Balance;
        }
        /// <summary>
        /// 教练员
        /// </summary>
        public string Trainer
        {
            get { return m_sTrainer; }
            set { m_sTrainer = value; }
        }
        /// <summary>
        /// 教练员编号
        /// </summary>
        public string TrainerNo
        {
            get { return m_sTrainerNo; }
            set { m_sTrainerNo = value; }
        }
        /// <summary>
        /// 驾校名称
        /// </summary>
        public string DrvSchool
        {
            get { return m_sDrvSchool; }
            set { m_sDrvSchool = value; }
        }
        /// <summary>
        /// 驾校ID
        /// </summary>
        public int DrvSchoolId
        {
            get { return m_sDrvSchoolId; }
            set { m_sDrvSchoolId = value; }
        }
        /// <summary>
        /// 学员指纹
        /// </summary>
        public string Fingerprint
        {
            get { return m_sFingerprint; }
            set { m_sFingerprint = value; }
        }
        /// <summary>
        /// 学员电话号码
        /// </summary>
        public string PhoneNo
        {
            get { return m_sPhoneNo; }
            set { m_sPhoneNo = value; }
        }
        /// <summary>
        /// 学员照片
        /// </summary>
        public byte[] Photo
        {
            get { return m_sPhoto; }
            set { m_sPhoto = value; }
        }
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime BirthDate
        {
            get { return m_sBirthDate; }
            set { m_sBirthDate = value; }
        }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginDt
        {
            get { return m_sBeginDt; }
            set { m_sBeginDt = value; }
        }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDt
        {
            get { return m_sEndDt; }
            set { m_sEndDt = value; }
        }
        /// <summary>
        /// 注册日期
        /// </summary>
        //public DateTime RegDate
        //{
        //    get { return m_sRegDate; }
        //    set { m_sRegDate = value; }
        //}
        public string RegDate
        {
            get { return m_sRegDate; }
            set { m_sRegDate = value; }
        }
        /// <summary>
        /// 家庭住址
        /// </summary>
        public string  HomeAddress
        {
            get { return m_sHomeAddress; }
            set { m_sHomeAddress = value; }
        }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex
        {
            get { return m_sSex; }
            set { m_sSex = value; }
        }
        /// <summary>
        /// 经办人
        /// </summary>
        public string FillPerson
        {
            get { return m_sFillPerson; }
            set { m_sFillPerson = value; }
        }
        /// <summary>
        /// 车型
        /// </summary>
        public string AutoType
        {
            get { return m_sAutoType; }
            set { m_sAutoType = value; }
        }
        /// <summary>
        /// 训练模式
        /// </summary>
        public string TrainingMode
        {
            get { return m_sTrainingMode; }
            set { m_sTrainingMode = value; }
        }
        #endregion
        
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
        //public void SearchTrainee()
        //{
        //    dsrsrc.trainMangeDataSet.TraineeDataTable.Clear();
        //    DBAccessHelper.GetTraStudInfo(Trainee.m_sPidNo, Trainee.m_sName, Trainee.m_sTrainerNo, Trainee.m_sAutoType, Trainee.m_sDrvSchoolId, Trainee.m_sBeginDt, Trainee.m_sEndDt, dsrsrc.trainMangeDataSet);
        //}
        /// <summary>
        /// 通过身份证查询学员信息
        /// </summary>
        /// <param name="sPidNo"></param>
        public void SearchTraineeByPidNo()
        { 
            m_trainMangeDataSet.TraineeDataTable.Clear();
            DBAccessHelper.GetTraStudInfoByPidNo(Trainee.m_sPidNo, m_trainMangeDataSet);
            if (m_trainMangeDataSet.TraineeDataTable.Rows.Count > 0)
            {
                DataRow dataRow = m_trainMangeDataSet.TraineeDataTable.Rows[0];
                Trainee.m_sName = dataRow["TRAINE_NAME"].ToString();
                if (dataRow["PHOTO"].ToString() != "" && dataRow["PHOTO"] != null)
                    Trainee.m_sPhoto = (byte[])dataRow["PHOTO"];
                else
                    Trainee.m_sPhoto = null;
                Trainee.m_sSex = dataRow["SEX"].ToString();
                Trainee.m_sBirthDate = Convert.ToDateTime(dataRow["BIRTH_DT"].ToString());
                Trainee.m_sFingerprint = dataRow["FINGERPRINT1"].ToString();
                Trainee.m_sDrvSchoolId = Convert.ToInt32(dataRow["DRIVING_SCHOOL_ID"].ToString());
                Trainee.m_sPhoneNo = dataRow["PHONE_NO"].ToString();
                Trainee.m_sRegDate = dataRow["FILLIN_TIME"].ToString();
                Trainee.m_sTrainerNo = dataRow["TRAINER_ID"].ToString();
                Trainee.m_sAutoType = dataRow["LICENSE_TYPE_CD"].ToString();
                Trainee.m_sTrainingMode = dataRow["TRAINING_MODE"].ToString();
                Trainee.m_nBalance = Convert.ToDouble(dataRow["BALANCE"].ToString());
                Trainee.m_sHomeAddress = dataRow["HOMEADDRESS"].ToString();
            }
        }     
        public static void SetComboBoxValue(System.Windows.Forms.ComboBox con, string val)
        {
            bool bfound = false;
            string strItem = "";
            for (int idx = 0; idx < con.Items.Count; ++idx)
            {
                strItem = con.Items[idx].ToString();
                int ifound = strItem.IndexOf(val);
                if (ifound == 0)
                {
                    con.SelectedItem = con.Items[idx];
                    bfound = true;
                    break;
                }
            }
            if (!bfound)
                throw new ApplicationException("Can't find value:" + val + " for dropdown list:" + con.Name);

        }
        /// <summary>
        /// 清空学员相关属性值
        /// </summary>
        public void ClearTraineeAttribute()
        {
            Trainee.m_sPidNo = "";
            Trainee.m_sName = "";
            Trainee.m_sPhoto = null;
            Trainee.m_sSex = "";
            Trainee.m_sBirthDate = DateTime.Now;
            Trainee.m_sFingerprint = "";
            Trainee.m_sDrvSchoolId = 0;
            Trainee.m_sPhoneNo = "";
            Trainee.m_sRegDate = DateTime.Now.ToString("yyyy-MM-dd");
            Trainee.m_sTrainerNo = "";
            Trainee.m_sAutoType = "";
            Trainee.m_sTrainingMode = "";
            Trainee.m_nBalance = Convert.ToDouble("0");
            Trainee.m_sHomeAddress = "";
            Trainee.m_sFillPerson = "";
        }

        
    }
}
