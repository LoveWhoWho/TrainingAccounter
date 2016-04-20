using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TrainingControl
{

    public class ChargesInfo
    {
        public string SeqNo { get; set; }
        public string PidNo { get; set; }
        public string AutoId { get; set; }
        public string StartTime {get; set; }
        public string EndTime { get; set; }
        public string Mode { get; set; }
        public double CurrentMileage { get; set; }
        public double CurrentMinutes { get; set; }
        public int CurrentTimes { get; set; }
        public int SurplusTimes
        {
            get;
            set;
        }
        public ChargesInfo()
        {
            SeqNo = "";
            PidNo = "";
            AutoId = "";
            StartTime = "";
            EndTime = "";
            Mode ="TIME";
            CurrentMileage = 0;
            CurrentMinutes = 0;
            CurrentTimes = 0;
            SurplusTimes = 0;
        }
    }

    /// <summary>
    /// 计费方式
    /// </summary>
    public enum ChargeMode
    {
        /// <summary>
        /// 计时间
        /// </summary>
        TIME,
        /// <summary>
        /// 计次数
        /// </summary>
        TRIES,
        /// <summary>
        /// 计里程
        /// </summary>
        MILEAGE
    }

    /// <summary>
    /// 训练时段
    /// </summary>
    public enum TrainSession
    {
        /// <summary>
        /// 上午
        /// </summary>
        AM,
        /// <summary>
        /// 下午
        /// </summary>
        PM
    }

    /// <summary>
    /// 许可状态
    /// </summary>
    public enum LicenseState { Normal, Invaild, Overdue, AutoTypeInvaild };

    public class Training
    {

    }

    /// <summary>
    /// 训练预约
    /// </summary>
    public class TrainBooking
    {
        DateTime m_trnDate;
        string m_sName;
        string m_sPidNo;
        TrainSession m_trnSession;
        string m_sBookCarNo;
        string m_sBookLicenseNm;
        string m_sTrainer;
        string m_sAutoType;

        /// <summary>
        /// 训练日期
        /// </summary>
        public DateTime Date
        {
            get { return m_trnDate; }
            set { m_trnDate = value; }
        }
        /// <summary>
        /// 学员姓名
        /// </summary>
        public string Name
        {
            get { return m_sName; }
            set { m_sName = value; }
        }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string PidNo
        {
            get { return m_sPidNo; }
            set { m_sPidNo = value; }
        }
        /// <summary>
        /// 训练时段
        /// </summary>
        public TrainSession Session
        {
            get { return m_trnSession; }
            set { m_trnSession = value; }
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
        /// 预约车型
        /// </summary>
        public string AutoType
        {
            get { return m_sAutoType; }
            set { m_sAutoType = value; }
        }

        /// <summary>
        /// 预约车号
        /// </summary>
        public string BookCarNo
        {
            get { return m_sBookCarNo; }
            set { m_sBookCarNo = value; }
        }

        /// <summary>
        /// 预约车号牌
        /// </summary>
        public string BookLicenseNm
        {
            get { return m_sBookLicenseNm; }
            set { m_sBookLicenseNm = value; }
        }

    }
       
    /// <summary>
    /// 训练许可
    /// </summary>
    public class TrainLicense
    {
        string m_trnDate;
        string m_sName;
        string m_sPidNo;
        string m_trnSession;
        string m_sAutoId;
        string m_sTrainer;
        string m_sAutoType;
        string m_sChargeMode;
        string m_sFingerprint;
        string m_sPhoto;
        double m_nTimeLmt;
        int m_nTriesLmt;
        int m_traBookSeqNo;
        double m_fMileageLmt;
        TrainDetail m_sTrainDetail;

		///<summary>
		///学员类型
		/// </summary>
		public int StudentType { get; set; }
		public int MinTimeUnit { get; set; }
		public double ChargingStandard { get; set; }
		public double AccountBalance { get; set; }
        /// <summary>
        /// 预约序号
        /// </summary>
        public int TraBookSeqNo
        {
            get { return m_traBookSeqNo; }
            set { m_traBookSeqNo = value; }
        }
        /// <summary>
        /// 训练日期
        /// </summary>
        /// 
        public string Date
        {
            get { return m_trnDate; }
            set { m_trnDate = value; }
        }
        /// <summary>
        /// 学员姓名
        /// </summary>
        public string Name
        {
            get { return m_sName; }
            set { m_sName = value; }
        }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string PidNo
        {
            get { return m_sPidNo; }
            set { m_sPidNo = value; }
        }
        /// <summary>
        /// 训练时段
        /// </summary>
        public string Session
        {
            get { return m_trnSession; }
            set { m_trnSession = value; }
        }
        /// <summary>
        /// 预约车号
        /// </summary>
        public string AutoId
        {
            get { return m_sAutoId; }
            set { m_sAutoId = value; }
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
        /// 预约车型
        /// </summary>
        public string AutoType
        {
            get { return m_sAutoType; }
            set { m_sAutoType = value; }
        }
        /// <summary>
        /// 计费模式
        /// </summary>
        public string ChargeMode
        {
            get { return m_sChargeMode; }
            set { m_sChargeMode = value; }
        }     
        /// <summary>
        /// 照片
        /// </summary>
        public string Photo
        {
            get { return m_sPhoto; }
            set { m_sPhoto = value; }
        }
        /// <summary>
        /// 指纹
        /// </summary>
        public string Fingerprint
        {
            get { return m_sFingerprint; }
            set { m_sFingerprint = value; }
        }
        /// <summary>
        /// 时间限制
        /// </summary>
        public double TimeLmt
        {
            get { return m_nTimeLmt; }
            set { m_nTimeLmt = value; }
        }
        /// <summary>
        /// 次数限制
        /// </summary>
        public int TriesLmt
        {
            get { return m_nTriesLmt; }
            set { m_nTriesLmt = value; }
        }
        /// <summary>
        /// 里程限制
        /// </summary>
        public double MileageLmt
        {
            get { return m_fMileageLmt; }
            set { m_fMileageLmt = value; }
        }
        /// <summary>
        /// 训练明细
        /// </summary>
        public TrainDetail TrainDetail
        {
            get { return m_sTrainDetail; }
            set { m_sTrainDetail = value; }
        }

        public LicenseState CheckLicense(string autoTypeCd)
        {
            LicenseState lincenseState = LicenseState.Invaild;
            string[] time = m_trnDate.Split('|');
            if (!time.Any(s => s.Equals(DateTime.Now.ToString("yyyy-MM-dd"))))
            {
                lincenseState = LicenseState.Overdue;
                return lincenseState;
            }
            if (!m_sAutoType.Equals(autoTypeCd.Trim()))
            {
                lincenseState = LicenseState.AutoTypeInvaild;
                return lincenseState;
            }
            if (!string.IsNullOrEmpty(m_sChargeMode))
            {
                switch (m_sChargeMode)
                {
                    case "Time":
                        {
                            lincenseState = (m_nTimeLmt > 0.0) ? LicenseState.Normal : LicenseState.Invaild;
                            break;
                        }
                    case "Mileage":
                        {
                            lincenseState = (m_fMileageLmt > 0.0) ? LicenseState.Normal : LicenseState.Invaild;
                            break;
                        }
                    case "Tries":
                        {
                            lincenseState = (m_nTriesLmt > 0) ? LicenseState.Normal : LicenseState.Invaild;
                            break;
                        }
                    default:
                        lincenseState = LicenseState.Invaild;
                        break;
                }
            }
            else
            {
                throw new MissingFieldException("许可模式不能为空！");
            }
            return lincenseState;
        }
    }


    /// <summary>
    /// 训练过程信息
    /// </summary>
    public class TrainProc
    {
       private string m_sCode;
       private string m_sMode;
       private string m_sTimestamp;
       private string m_sType;
       
        /// <summary>
        /// 代码(项目代码/扣分代码)
        /// </summary>
        public string Code
        {
            get { return m_sCode; }
            set { m_sCode = value; }
        }
        /// <summary>
        /// 训练模式(E-模拟考试、P-模考练习Practice、R-自由训练Roam)
        /// </summary>
        public string Mode
        {
            get { return m_sMode; }
            set { m_sMode = value; }
        }
        /// <summary>
        /// 时间戳（时分秒）
        /// </summary>
        public string Timestamp
        {
            get { return m_sTimestamp; }
            set { m_sTimestamp = value; }
        }
        /// <summary>
        /// 类型(开始S/结束E/扣分V)
        /// </summary>
        public string Type
        {
            get { return m_sType; }
            set { m_sType = value; }
        }
    }

    /// <summary>
    /// 训练明细
    /// </summary>
    public class TrainDetail
    {
        string m_sAutoId;
        string m_sTrainer;
        double  m_nTrainTime;
        int m_nTrainTries;
        double m_fTrainMileage;       
        DateTime m_dTrainEndTs;
        DateTime m_dTrainStartTs;
        List<TrainProc> m_alTrainProcList;

       
        /// <summary>
        /// 训练开始时间
        /// </summary>
        public DateTime TrainStartTs
        {
            get { return m_dTrainStartTs; }
            set { m_dTrainStartTs = value; }
        }
        /// <summary>
        /// 训练结束时间
        /// </summary>
        public DateTime TrainEndTs
        {
            get { return m_dTrainEndTs; }
            set { m_dTrainEndTs = value; }
        }
        /// <summary>
        /// 车号
        /// </summary>
        public string AutoId
        {
            get { return m_sAutoId; }
            set { m_sAutoId = value; }
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
        /// 训练时长
        /// </summary>
        public double TrainTime
        {
            get { return m_nTrainTime; }
            set { m_nTrainTime = Math.Round(value,3); }
        }
        /// <summary>
        /// 训练次数
        /// </summary>
        public int TrainTries
        {
            get { return m_nTrainTries; }
            set { m_nTrainTries = value; }
        }
        /// <summary>
        /// 训练里程
        /// </summary>
        public double TrainMileage
        {
            get { return m_fTrainMileage; }
            set { m_fTrainMileage = Math.Round(value,3); }
        }
        /// <summary>
        /// 训练过程信息
        /// </summary>
        public List<TrainProc> TrainProcList
        {
            get { return m_alTrainProcList; }
            set { m_alTrainProcList = value; }
        }
    }

    
}
