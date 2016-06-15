using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TrainingControl
{
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
        //public int Student_Type { get; set; }
        public int MinTimeUnit { get; set; }
        public double ChargingStandard { get; set; }
        public double AccountBalance { get; set; }
        string m_sPhoto;
        double m_nTimeLmt;
        int m_nTriesLmt;
        int m_traBookSeqNo;
        double m_fMileageLmt;
        List<TrainDetail> m_sTrainDetail;

		///<summary>
		///学员类型
		/// </summary>
		public int StudentType { get; set; }	
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
        public List<TrainDetail> TrainDetail
        {
            get { return m_sTrainDetail; }
            set { m_sTrainDetail = value; }
        }

        public LicenseState CheckLicense()
        {
            LicenseState lincenseState = LicenseState.Invaild;
            string[] time = m_trnDate.Split('|');
            if (StudentType == 0)
            {
                if (!time.Any(s => s.Equals(DateTime.Now.ToString("yyyy-MM-dd"))))
                {
                    lincenseState = LicenseState.Overdue;
                    return lincenseState;
                }
            }
            if (!m_sAutoType.Equals(AutoType))
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
                            lincenseState = (AccountBalance >= MinTimeUnit * ChargingStandard) ? LicenseState.Normal : LicenseState.Invaild;
                            break;
                        }
                    case "Mileage":
                        {
                            lincenseState = (m_fMileageLmt > 0.0) ? LicenseState.Normal : LicenseState.Invaild;
                            break;
                        }
                    case "Tries":
                        {
                            lincenseState = (AccountBalance >= ChargingStandard) ? LicenseState.Normal : LicenseState.Invaild;
                            break;
                        }
                    default:
                        lincenseState = LicenseState.Invaild;
                        break;
                }
            }
            else
            {
                throw new MissingFieldException("许可无效！");
            }
            return lincenseState;
        }

        public TrainingDetail GetTrainningDetail()
        {
            TrainingDetail detail = new TrainingDetail();
            detail.Name = Name;
            detail.PidNo = PidNo;
            detail.Balance = AccountBalance.ToString();
            detail.State = CheckLicense() == LicenseState.Normal ? "正常" : "余额不足";
            if (TrainDetail != null)
            {
                detail.StartTime = TrainDetail[TrainDetail.Count - 1].TrainStartTs.ToString();
                detail.EndTime = TrainDetail[TrainDetail.Count - 1].TrainEndTs.ToString();
                detail.TrainingTime = Math.Round((TrainDetail[TrainDetail.Count - 1].TrainEndTs - TrainDetail[TrainDetail.Count - 1].TrainStartTs).TotalMinutes, 1).ToString();
                List<TrainProc> proc = TrainDetail[TrainDetail.Count - 1].TrainProcList;
                var tries = from item in proc
                            where item.Code == "10000" && item.Type == "S"
                            select item;
                var itemCount = from item in proc
                                where item.Code != "10000" && item.Type == "S"
                                select item;
                var deduckPoints = from item in proc
                                   where item.Type == "V"
                                   select item.Code;
                detail.TrainingTries = tries.Count();
                detail.TrainingItemCount = itemCount.Count();
                detail.Deduck = deduckPoints.ToList();
            }
            return detail;
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

    public class ChargesInfo
    {
        /// <summary>
        /// Gets or sets the 序号.
        /// </summary>
        /// <value>
        /// The seq no.
        /// </value>
        public string SeqNo { get; set; }
        /// <summary>
        /// 获取或设置身份证号
        /// </summary>
        /// <value>
        /// The pid no.
        /// </value>
        public string PidNo { get; set; }
        /// <summary>
        /// 获取或设置车号
        /// </summary>
        /// <value>
        /// The automatic identifier.
        /// </value>
        public string AutoId { get; set; }

        /// <summary>
        /// 计费模式
        /// </summary>
        /// <value>
        /// The charge mode.
        /// </value>
        public string ChargeMode { get; set; }

        /// <summary>
        /// 获取或设置训练类型
        /// </summary>
        /// <value>
        /// The type of the operation.
        /// </value>
        public string OperationType { get; set; }
        /// <summary>
        /// 获取或设置动作时间
        /// </summary>
        /// <value>
        /// The operation time.
        /// </value>
        public string OperationTime { get; set; }
        /// <summary>
        /// 当前余额
        /// </summary>
        /// <value>
        /// The curren balance.
        /// </value>
        public double CurrenBalance { get; set; }
        /// <summary>
        /// 当前次数
        /// </summary>
        /// <value>
        /// The current tries.
        /// </value>
        public int CurrentTries { get; set; }
        /// <summary>
        /// 剩余时间
        /// </summary>
        /// <value>
        /// The remaining time.
        /// </value>
        public double RemainingTime { get; set; }
        public ChargesInfo()
        {


        }
    }

    public class ChargeControlInfo
    {
        private string pidNo;

        public string PidNo
        {
            get { return pidNo; }
            set { pidNo = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string chargeMode;

        public string ChargeMode
        {
            get { return chargeMode; }
            set { chargeMode = value; }
        }
        private double balance;

        public double Balance
        {
            get { return balance; }
            set { balance = value; }
        }
        private double time;

        public double Time
        {
            get { return time; }
            set { time = value; }
        }
        private int tries;

        public int Tries
        {
            get { return tries; }
            set { tries = Math.Abs(value); }
        }
        private string startTime;

        public string StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }
        
    }

    /// <summary>
    /// 训练详细(显示用)
    /// </summary>
    public class TrainingDetail
    {
        public string Name;
        public string PidNo;
        public string State;
        public string Balance;
        public string TrainingTime;
        public string StartTime;
        public string EndTime;
        public int TrainingTries;
        public int TrainingItemCount;
        public List<string> Deduck;

        public TrainingDetail()
        {
            Name = "";
            PidNo = "";
            State = "";
            Balance = "余额不足";
            TrainingTime = "0";
            StartTime = "";
            EndTime = "";
            TrainingTries = 0;
            TrainingItemCount = 0;
            Deduck = new List<string>();
        }

    }
}
