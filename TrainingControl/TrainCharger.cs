using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace TrainingControl
{

    public class TrainCharger
    {
        public delegate void DelegateStop();
        public delegate void DelegateStopNewExam();
        public delegate void DelegateClearPostBackData();
        public delegate void DelegateResetUiView();
        public DelegateStop StopDelegate = null;
        public DelegateStopNewExam StopNewExamDelegate = null;
        public DelegateClearPostBackData ClearPostBackDataDelegate = null;
        public DelegateResetUiView ResetUiViewDelegate = null;

        private List<TrainLicense> _trnLicenseCollection;
        private TrainLicense _trnLicense;
        private float _trnMileage;
        private LicenseState _lincenseState;
        private readonly ReadAndWrite _usbHelper = new ReadAndWrite();
        private ChargesInfo _chargesDisplayInfo;
       
        private readonly ConcurrentQueue<TrainProc> _trainProcQueue = new ConcurrentQueue<TrainProc>();
        private ChargingControl _currentInfoControl;
        private string _mLicensePath = string.Empty;
        private readonly StreamWriter _sw;
        private string _usbSerialNumber = string.Empty;
        private string _usbDriveName = string.Empty;

        private int _indexOfLicense = -1;

        private bool IsChargerControlHide = false;

        double cacheTime = 0;
        double detailTime = 0;
        double detailMileage = 0;
        DateTime startTime = DateTime.Now;
        int detailTries = 0;

        public string AutoId
        {
            private get;
            set;
        }
        public TrainLicense Lincense
        {
            get
            {
                return _trnLicense;
            }
        }
        public bool IsRunning
        {
            get; private set;
        }
        public bool IsCharging
        {
            get;
            set;
        }
        private bool IsChargingThread
        { get; set; }

        public float SetCurrentMileage
        {
            set
            {
                _trnMileage = value / 1000f;
            }
        }

        public int IndexOfLicense
        {
            get
            {
                return _indexOfLicense;
            }

            set
            {
                if (value < _trnLicenseCollection.Count && value >= 0)
                {
                    _indexOfLicense = value;
                    _trnLicense = _trnLicenseCollection[value];
                }

            }
        }

        public int GetNewIndexOfLicense(string autoTypeCd)
        {
            if (_trnLicense != null && _trnLicense.CheckLicense(autoTypeCd)== LicenseState.Normal)
            {
                return _indexOfLicense;
            }
            int currentIndex = -1;
            if (_trnLicenseCollection == null)
                return currentIndex;
            if (_indexOfLicense == _trnLicenseCollection.Count - 1)
            {
                currentIndex = 0;
            }
            else
                currentIndex = _indexOfLicense + 1;
            for (int i = currentIndex; i < _trnLicenseCollection.Count; i++)
            {
                if (_trnLicenseCollection[i].CheckLicense(autoTypeCd) == LicenseState.Normal)
                {
                    _indexOfLicense = i;
                    return _indexOfLicense;
                }
            }
            //若未查询到可用许可，遍历所有许可，查看是否可用
            for (int j = 0; j < _trnLicenseCollection.Count; j++)
            {
                if (_trnLicenseCollection[j].CheckLicense(autoTypeCd) == LicenseState.Normal)
                {
                    _indexOfLicense = j;
                    return _indexOfLicense;
                }
            }
            return -1;
        }



        /// <summary>
        /// Gets the TRN license collection.
        /// </summary>
        /// <value>
        /// The TRN license collection.
        /// </value>
        public List<TrainLicense> TrnLicenseCollection
        {
            get { return _trnLicenseCollection; }
            //set { _trnLicenseCollection = value; }
        }

        public TrainCharger()
        {
            try
            {
                _sw = new StreamWriter("test.log", true, Encoding.UTF8)
                    {
                        AutoFlush = true
                    };
                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "开始初始化……");
                SqlLiteHelper.CreateAllTable();
            }
            catch(Exception ex)
            {
                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "计费初始化失败：" + ex.Message);
                throw ex;
            }
        }


        /// <summary>
        /// 根据盘符加载许可
        /// </summary>
        /// <param name="usbDriveName">U盘名称，若为空则加载默认U盘</param>
        /// <returns>成功返回True，否则为False</returns>
        public int LoadLincese(string usbDriveName, string autoTypeCd)
        {   
            _indexOfLicense = -1;
            _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "开始加载许可信息，参数为：" + usbDriveName);
            _trnLicenseCollection = string.IsNullOrEmpty(usbDriveName) ? _usbHelper.ReadTrainLicense() : _usbHelper.ReadTrainLicense(usbDriveName);
            if (_trnLicenseCollection != null)
            {
                _mLicensePath = string.IsNullOrEmpty(usbDriveName) ? _usbHelper.GetLicensePath() : usbDriveName.Replace("\\","");
                List<string> number = _usbHelper.matchDriveLetterWithSerial(_mLicensePath);
                _usbSerialNumber = number[0];
                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "许可信息加载成功，并且已保存！");
                foreach (TrainLicense tl in _trnLicenseCollection)
                {
                    if (tl.CheckLicense(autoTypeCd)== LicenseState.Normal)
                    {
                        _indexOfLicense = _trnLicenseCollection.IndexOf(tl);
                        break;
                    }
                }
            }
            return _indexOfLicense;
        }

        public List<LicenseState> GetAllLicenseState(string usbDriveName, string autoTypeCd)
        {
            List<LicenseState> state = new List<LicenseState>();
            _trnLicenseCollection = string.IsNullOrEmpty(usbDriveName) ? _usbHelper.ReadTrainLicense() : _usbHelper.ReadTrainLicense(usbDriveName);
            if (_trnLicenseCollection != null)
            {
                foreach (TrainLicense tl in _trnLicenseCollection)
                {
                   
                    state.Add(tl.CheckLicense(autoTypeCd));
                }
            }
            return state;
        }

        public bool StartTrain(string autoType)
        {
            _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "………………………………开始训练…………………………");
            _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "开启训练线程，检测许可信息……");
            _lincenseState = _trnLicense.CheckLicense(autoType);
            if (_lincenseState == LicenseState.Normal)
            {
                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":学员" + _trnLicense.PidNo + "的许可信息有效……");
                ClearTrainProQueue();
                IsRunning = true;
                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "开启过程数据检查线程……");
                Thread checkTrainProcThread = new Thread(CheckTrainProcThread);
                checkTrainProcThread.Start();
                return true;
            }
            else
            {
                IsRunning = false;
                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "许可信息无效，许可失效类型：" + _lincenseState);
                return false;
            }
          
        }

        public bool StartCharge(string autoType)
        {
            _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "准备开始计费，开始检测许可信息……");
            _lincenseState = _trnLicense.CheckLicense(autoType);
            SqlLiteHelper.SaveTranineesInfo(_trnLicense);
            if (_lincenseState== LicenseState.Normal)
            {
                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "该许可信息有效……");
                if (_currentInfoControl == null)
                {
                    _currentInfoControl = new ChargingControl();
                    _currentInfoControl.Show();
                }
                else
                {
                    _currentInfoControl.Show();
                }
                IsChargerControlHide = false;
                _chargesDisplayInfo = new ChargesInfo
                {
                    PidNo = _trnLicense.PidNo,
                    Mode = _trnLicense.ChargeMode,
                    SeqNo = Guid.NewGuid().ToString("N").ToUpper(),
                    CurrentMileage = _trnLicense.MileageLmt,
                    CurrentMinutes = _trnLicense.TimeLmt,
                    SurplusTimes = _trnLicense.TriesLmt
                };
                _currentInfoControl.CallShowTrainerName(_trnLicense.Name);
                _currentInfoControl.CallShowTrainerPhoto(_trnLicense.Photo);
                _currentInfoControl.CallChangesShowInfo(_chargesDisplayInfo);
                IsChargingThread = true;
                Thread chargingThread = new Thread(ChargingThread);
                chargingThread.Start();
                return true;
            }
            else
            {
                IsChargingThread = false;
                IsCharging = false;
                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "许可信息无效,失效类型：" + _lincenseState);
                return false;
            }
        }


        public void StopCharging()
        {
            IsChargingThread = false;
            IsCharging = false;
            SqlLiteHelper.SaveTranineesInfo(_trnLicense);
            SqlLiteHelper.SaveChargesInfo(_chargesDisplayInfo);
            SqlLiteHelper.SaveTraningInfo(_trnLicense);
            writeToDisk();

            if (StopNewExamDelegate != null)
            {
                StopNewExamDelegate();
            }
        }

        /// <summary>
        /// Stops the train.
        /// </summary>
        /// <param name="isSave">if set to <c>true</c> [is save Train Info].</param>
        public void StopTrain(bool isSave)
        {
            if (!IsChargerControlHide)
            {
                IsRunning = false;
                IsChargingThread = false;
                IsCharging = false;
                HideChargeControl();
                if (ResetUiViewDelegate != null)
                {
                    ResetUiViewDelegate();
                }
                if(isSave)
                {
                    SqlLiteHelper.SaveTranineesInfo(_trnLicense);
                    SqlLiteHelper.SaveChargesInfo(_chargesDisplayInfo);
                    SqlLiteHelper.SaveTraningInfo(_trnLicense);
                    writeToDisk();
                }
                IsChargerControlHide = true;
            }
        }


        public void AddProcInfo(string code, string mode, string timeStamp, string type)
        {
            TrainProc tp = new TrainProc
            {
                Code = code,
                Mode = mode,
                Timestamp = timeStamp,
                Type = type
            };
            _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "加入队列数据:" + tp.Code + "," + tp.Type);
            _trainProcQueue.Enqueue(tp);
        }

        private void ClearTrainProQueue()
        {
            TrainProc tp;
            while (_trainProcQueue.TryDequeue(out tp))
            {
            }
        }

        public void HideChargeControl()
        {
            if (_currentInfoControl != null)
            {
                _currentInfoControl.Dispatcher.Invoke(new Action(_currentInfoControl.Hide));
            }
        }

        private void clearPostBackData()
        {
            if (ClearPostBackDataDelegate != null)
                ClearPostBackDataDelegate.Invoke();
        }

        private void CheckTrainProcThread()
        {
            bool canGo = false;
            bool isEnd = false;
            while (IsRunning)
            {
                if (_trainProcQueue.Count > 0)
                {
                    _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "发现" + _trainProcQueue.Count + "条过程数据，开始处理……");
                    TrainProc tp;
                    if (!_trainProcQueue.TryDequeue(out tp) || tp == null)
                    {
                        throw new InvalidOperationException("队列处理失败");
                    }
                    _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "过程数据:" + tp.Code + "," + tp.Type);
                    if (tp.Code == "10000" && tp.Type == "S")
                    {
                        cacheTime = 0;
                        detailTime = 0;
                        detailMileage = 0;
                        detailTries = 0;
                        startTime = DateTime.Now;
                        canGo = true;
                        isEnd = false;
                        if (_trnLicense.TrainDetail == null)
                        {
                            _trnLicense.TrainDetail = new TrainDetail
                            {
                                TrainStartTs = startTime
                            };
                        }
                        else
                        {
                            detailMileage = _trnLicense.TrainDetail.TrainMileage;
                            detailTime = _trnLicense.TrainDetail.TrainTime;
                            detailTries = _trnLicense.TrainDetail.TrainTries;
                        }
                        if (_trnLicense.TrainDetail.TrainProcList == null)
                        {
                            _trnLicense.TrainDetail.TrainProcList = new List<TrainProc>();
                        }
                        SaveTrainingInfo(tp);
                        SqlLiteHelper.SaveChargesInfo(_chargesDisplayInfo);
                        SqlLiteHelper.SaveTraningInfo(_trnLicense);
                        writeToDisk();
                        _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "过程数据为10000，状态为S,初始化状态信息……");
                        
                        IsCharging = true;
                        _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "状态信息初始化完毕，开启训练线程……");
                        continue;
                    }
                    if (!canGo)
                    {
                        if (tp.Code == "10000" && tp.Type == "T")
                        {
                            StopCharging();
                            _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "训练未开始，关闭训练！");
                        }
                        else
                            _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "训练未开始，当前数据无效，已丢弃！");
                        continue;
                    }
                    if (tp.Code == "10000" && tp.Type == "T")
                    {
                        if (!IsRunning)
                            continue;
                        if (_trnLicense.TrainDetail != null)
                        {
                            _trnLicense.TrainDetail.TrainEndTs = DateTime.Now;
                        }
                        _lincenseState = LicenseState.Invaild;
                        _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + @"过程数据为10000，状态为T,准备保存状态信息……");
                        SaveTrainingInfo(tp);
                        StopTrain(true);
                        _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "本次状态信息保存完毕，关闭训练界面，返回验证界面……");
                        continue;
                    }
                    if (tp.Code == "10000" && tp.Type == "E")
                    {
                        if (!isEnd)
                        {
                            isEnd = true;
                            SaveTrainingInfo(tp);
                            if (_trnLicense.ChargeMode == "Tries")
                            {
                                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + @"过程数据为10000，状态为E,计费模式为[次数]，检查许可信息……");
                                if (_trnLicense.TriesLmt <= 0)
                                {
                                    _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "训练次数为0，停止计费……");
                                    StopCharging();
                                    if (StopDelegate != null)
                                    {
                                        StopDelegate();
                                    }
                                    continue;
                                }
                            }
                            StopCharging();
                            _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "本次训练过程信息保存完毕，本次训练结束……");
                        }
                        continue;
                    }
                    SaveTrainingInfo(tp);
                    SqlLiteHelper.SaveChargesInfo(_chargesDisplayInfo);
                    SqlLiteHelper.SaveTraningInfo(_trnLicense);
                    writeToDisk();
                }
                Thread.Sleep(100);
            }
        }

        private void ChargingThread()
        {
            while (IsChargingThread)
            {
                Thread.Sleep(300);
                double spanTime = 2;
                if (IsCharging && _lincenseState == LicenseState.Normal)
                {
                   
                    DateTime lastHandleTime = DateTime.Now;
                    double totalMileage = _trnLicense.MileageLmt;
                    double totalTime = _trnLicense.TimeLmt;
                    _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "计费前里程剩余：" + totalMileage + "，次数剩余：" + _trnLicense.TriesLmt + "，时间剩余：" + totalTime + "，当前计费模式:" + _trnLicense.ChargeMode);
                    SqlLiteHelper.SaveChargesInfo(_chargesDisplayInfo);
                    _chargesDisplayInfo.StartTime = startTime.ToString("yyyy-MM-dd HH:mm:ss");
                    _trnLicense.TriesLmt = _trnLicense.TriesLmt - 1;
                    detailTries += 1;
                    if (_trnLicense.ChargeMode == "Tries")
                    {
                        if (_trnLicense.TriesLmt < 0)
                        {
                            _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "训练次数非法，准备终止训练……");
                            StopCharging();
                            if (StopDelegate != null)
                            {
                                StopDelegate();
                            }
                            continue;
                        }
                    }
                    else
                    {
                        SqlLiteHelper.SaveChargesInfo(_chargesDisplayInfo);
                        writeToDisk();
                    }
                    while (IsCharging && _lincenseState == LicenseState.Normal)
                    {
                        DateTime now = DateTime.Now;
                        TimeSpan ts = now - lastHandleTime;
                        double diff = Math.Round((now - startTime).TotalHours, 4);
                        cacheTime = totalTime - diff;
                        if (_trnLicense.ChargeMode == "Time")
                        {
                            _trnLicense.TimeLmt = cacheTime;
                        }
                        _trnLicense.MileageLmt = totalMileage - Math.Round(_trnMileage, 4);
                        _trnLicense.TriesLmt = _trnLicense.TriesLmt;
                        if (_trnLicense.TrainDetail != null)
                        {
                            _trnLicense.TrainDetail.TrainTime = detailTime + diff;
                            _trnLicense.TrainDetail.TrainMileage = detailMileage + _trnMileage;
                            _trnLicense.TrainDetail.TrainTries = detailTries;
                            _trnLicense.TrainDetail.AutoId = AutoId;
                        }
                        _chargesDisplayInfo.SurplusTimes = _trnLicense.TriesLmt;
                        _chargesDisplayInfo.CurrentMinutes = cacheTime;
                        _chargesDisplayInfo.CurrentMileage = _trnLicense.MileageLmt;
                        string chargeMode = _trnLicense.ChargeMode;
                        if (chargeMode != null)
                        {
                            switch (chargeMode)
                            {
                                case "Time":
                                    {
                                        if (_trnLicense.TimeLmt > 0.0)
                                        {
                                            if (ts.TotalMinutes >= spanTime)
                                            {
                                                lastHandleTime = now;
                                                SqlLiteHelper.SaveChargesInfo(_chargesDisplayInfo);
                                                writeToDisk();
                                                clearPostBackData();
                                            }
                                        }
                                        else
                                        {
                                            StopCharging();
                                            if (StopDelegate != null)
                                            {
                                                StopDelegate();
                                            }
                                        }
                                        break;
                                    }
                                case "Mileage":
                                    {
                                        if (cacheTime <= 0.0 || _trnLicense.MileageLmt > 0.0)
                                        {
                                            if (ts.TotalMinutes >= spanTime)
                                            {
                                                lastHandleTime = now;
                                                SqlLiteHelper.SaveChargesInfo(_chargesDisplayInfo);
                                                writeToDisk();
                                                clearPostBackData();
                                            }
                                        }
                                        else
                                        {
                                            StopCharging();
                                            if (StopDelegate != null)
                                            {
                                                StopDelegate();
                                            }
                                        }
                                        break;
                                    }
                                case "Tries":
                                    {
                                        if (_trnLicense.TriesLmt >= 0)
                                        {
                                            if (cacheTime <= 0.0)
                                            {
                                                StopCharging();
                                                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "时间耗尽，训练已停止！");
                                                if (_trnLicense.TriesLmt <= 0)
                                                {
                                                    if (StopDelegate != null)
                                                    {
                                                        StopDelegate();
                                                    }
                                                }
                                                continue;
                                            }
                                            if (ts.TotalMinutes >= spanTime)
                                            {
                                                lastHandleTime = now;
                                                SqlLiteHelper.SaveChargesInfo(_chargesDisplayInfo);
                                                writeToDisk();
                                                clearPostBackData();
                                            }
                                        }
                                        else
                                        {
                                            StopCharging();
                                            if (StopDelegate != null)
                                            {
                                                StopDelegate();
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        if (_trnLicense.TimeLmt > 0.0)
                                        {
                                            if (ts.TotalMinutes >= 5.0)
                                            {
                                                lastHandleTime = now;
                                                SqlLiteHelper.SaveChargesInfo(_chargesDisplayInfo);
                                                writeToDisk();
                                            }
                                        }
                                        else
                                        {
                                            StopCharging();
                                            if (StopDelegate != null)
                                            {
                                                StopDelegate();
                                            }
                                        }
                                        break;
                                    }
                            }
                        }
                        if (_currentInfoControl != null)
                            _currentInfoControl.CallChangesShowInfo(_chargesDisplayInfo);
                        Thread.Sleep(800);
                    }
                }
            }
            
        }

        private void writeToDisk()
        {
            _trnLicenseCollection[_indexOfLicense] = _trnLicense;
            _usbHelper.CreateLogFileAndWriteLog(_mLicensePath,
                                                   JsonConvert.SerializeObject(_trnLicenseCollection), _usbSerialNumber);
        }

        private void SaveTrainingInfo(TrainProc tpInfo)
        {
            if (_trnLicense != null && _trnLicense.TrainDetail != null)
                _trnLicense.TrainDetail.TrainProcList.Add(tpInfo);
        }

        //private void axZKFPEngX_OnFeatureInfo(object sender, AxZKFPEngXControl.IZKFPEngXEvents_OnFeatureInfoEvent e)
        //{
        //    if (e.aQuality != 0)
        //    {
        //        if (e.aQuality == 1)
        //        {
        //            //ProcessUtility.ShowErrorMessage("指纹特征点不够！请重按。");
        //        }
        //        else
        //        { 
        //            //ProcessUtility.ShowErrorMessage("其它原因导致不能取到指纹特征！请重按。");
        //        }
        //    }
        //}

        //public void InitZKFPEng()
        //{
        //    if (m_AxZKFPEngX != null)
        //        m_AxZKFPEngX = new AxZKFPEngXControl.AxZKFPEngX();
        //    switch (m_AxZKFPEngX.InitEngine())
        //    {
        //        case 0:
        //            this.m_AxZKFPEngX.OnFeatureInfo += new AxZKFPEngXControl.IZKFPEngXEvents_OnFeatureInfoEventHandler(this.axZKFPEngX_OnFeatureInfo);
        //            this.m_AxZKFPEngX.OnCapture += new AxZKFPEngXControl.IZKFPEngXEvents_OnCaptureEventHandler(this.axZKFPEngX_OnCapture);
        //            break;
        //        case 1:
        //            throw new ApplicationException("指纹识别驱动程序加载失败！");
        //        case 2:
        //            throw new ApplicationException("没有连接指纹仪！");
        //        case 3:
        //            throw new ApplicationException("指定的指纹仪不存在！");
        //    }
        //}

        //private void axZKFPEngX_OnCapture(object sender, AxZKFPEngXControl.IZKFPEngXEvents_OnCaptureEvent e)
        //{
        //    try
        //    {
        //        bool bRegFeatureChanged = true;
        //        object objRegTemp1 = null;
        //        object objRegTemp2 = null;
        //        bool bRegTemp1Match = false;
        //        bool bRegTemp2Match = false;
        //        if ("" != string.Empty)
        //        {
        //            objRegTemp1 = m_AxZKFPEngX.DecodeTemplate1(fingerprint1);
        //            bRegTemp1Match = m_AxZKFPEngX.VerFinger(ref objRegTemp1, e.aTemplate, true, ref bRegFeatureChanged);
        //        }
        //        if (bRegTemp1Match)
        //        {
                    
        //        }
        //        else
        //        {
                    
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        

    }
}
