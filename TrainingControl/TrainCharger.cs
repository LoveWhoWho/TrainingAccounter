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
        public DelegateStop StopDelegate = null;
        
        private List<TrainLicense> _trnLicenseCollection;
        private TrainLicense _trnLicense;
        private float _trnMileage;
        private readonly ReadAndWrite _usbHelper = new ReadAndWrite();
        private ChargesInfo _chargesProcInfo;
        private ChargeControlInfo _controlInfo;
        private readonly ConcurrentQueue<TrainProc> _trainProcQueue = new ConcurrentQueue<TrainProc>();
        private ChargingControl _currentInfoControl;
        private string _mLicensePath = string.Empty;
        private readonly StreamWriter _sw;
        private string _usbSerialNumber = string.Empty;
        private string _usbDriveName = string.Empty;

        private int _indexOfLicense = -1;

        /// <summary>
        /// 检查是否所有许可都可用
        /// </summary>
        public bool IsAllLincenseOk 
        {
            get
            {
                if (_trnLicenseCollection != null)
                {
                    foreach (TrainLicense tl in _trnLicenseCollection)
                    {
                        LicenseState state=tl.CheckLicense();
                        if (state == LicenseState.Overdue || state == LicenseState.AutoTypeInvaild)
                        {
                            return false;
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 检查所有许可中是否含有有效许可
        /// </summary>
        public bool IsLincenseOk
        {
            get
            {
                if (_trnLicenseCollection != null)
                {
                    foreach (TrainLicense tl in _trnLicenseCollection)
                    {

                        if (tl.CheckLicense() == LicenseState.Normal)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        
        public string AutoId
        {
            private get;
            set;
        }
        public string AutoTypeCd { get; set; }
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

        /// <summary>
        /// 获取一个新的许可索引，如果当前可用返回当前索引
        /// </summary>
        /// <returns></returns>
        public int GetNewIndexOfLicense()
        {
            if (_trnLicense != null && _trnLicense.CheckLicense()== LicenseState.Normal)
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
                if (_trnLicenseCollection[i].CheckLicense() == LicenseState.Normal)
                {
                    _indexOfLicense = i;
                    return _indexOfLicense;
                }
            }
            //若未查询到可用许可，遍历所有许可，查看是否可用
            for (int j = 0; j < _trnLicenseCollection.Count; j++)
            {
                if (_trnLicenseCollection[j].CheckLicense() == LicenseState.Normal)
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
                _sw = new StreamWriter(DateTime.Now.ToString("yyyy-MM-dd") + ".log", true, Encoding.UTF8)
                {
                    AutoFlush = true
                };
                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "开始初始化……");
                SqlLiteHelper.CreateAllTable();
                _currentInfoControl = new ChargingControl();
                _chargesProcInfo = new ChargesInfo();
                AutoTypeCd = "C1";
                AutoId = "C1";

            }
            catch (Exception ex)
            {
                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "计费初始化失败：" + ex.Message);
                throw ex;
            }
        }



        /// <summary>
        /// 加载许可
        /// </summary>
        /// <param name="usbDriveName">指定U盘盘符，空则自动加载</param>
        /// <returns>成功:True,失败：False</returns>
        public bool LoadLincese(string usbDriveName)
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
                    if (tl.CheckLicense()== LicenseState.Normal)
                    {
                        _indexOfLicense = _trnLicenseCollection.IndexOf(tl);
                        break;
                    }
                }
            }
            if (_indexOfLicense >= 0)
                return true;
            return false;
            
        }

       
        public List<LicenseState> CheckAllLicenseState()
        {
            List<LicenseState> states = new List<LicenseState>();
            if (_trnLicenseCollection != null)
            {
                foreach (TrainLicense tl in _trnLicenseCollection)
                {
                    states.Add(tl.CheckLicense());
                }
            }
            return states;
        }
        /*
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
         */

		public bool StartCharge()
		{
			_sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "开始计费，重新检测许可信息……");

            if (_trnLicense.CheckLicense() == LicenseState.Normal)
            {
                _controlInfo = new ChargeControlInfo()
                {
                    PidNo = _trnLicense.PidNo,
                    Name = _trnLicense.Name,
                    Balance = _trnLicense.AccountBalance,
                    Time = 0,
                    Tries = 0
                };
                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "该许可信息有效……");
                SqlLiteHelper.SaveTranineesInfo(_trnLicense);
                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "已保存当前许可……");
                IsRunning = true;
                DateTime startTime = DateTime.Now;
                if (_trnLicense.TrainDetail == null)
                {
                    _trnLicense.TrainDetail = new List<TrainDetail>();
                }

                _trnLicense.TrainDetail.Add(new TrainDetail());
                int lastIndex = _trnLicense.TrainDetail.Count - 1;
                _trnLicense.TrainDetail[lastIndex].TrainProcList = new List<TrainProc>();
                _trnLicense.TrainDetail[lastIndex].TrainStartTs = startTime;
                _trnLicense.TrainDetail[lastIndex].AutoId = AutoId;

                _controlInfo.StartTime = _trnLicense.TrainDetail[lastIndex].TrainStartTs.ToString("HH:mm:ss");
                _currentInfoControl.Show();

                SaveChargProcInfo("开始训练");
                Thread updateThread = new Thread(UpdateChargingControle);
                updateThread.IsBackground = true;
                updateThread.Start();
                Thread timeThread = new Thread(ChargingTimeThread);
                timeThread.IsBackground = true;
                timeThread.Start(_trnLicense.ChargeMode == "Time");
                Thread triesThread = new Thread(ChargingTriesThread);
                triesThread.IsBackground = true;
                triesThread.Start(_trnLicense.ChargeMode == "Tries");
                return true;

            }
            else
            {
                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":" + "许可信息无效,失效类型：" + _trnLicense.CheckLicense());
                return false;
            }
		}

        public void UpdateChargingControle()
        {
            
            while(IsRunning)
            {
                _currentInfoControl.CallChangeShowInfo(_controlInfo);
                Thread.Sleep(500);
            }
        }

        public int SaveChargProcInfo(string operationType)
        {
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _chargesProcInfo.AutoId = Lincense.AutoId;
            _chargesProcInfo.CurrenBalance = Lincense.AccountBalance;
            _chargesProcInfo.CurrentTries = Lincense.TriesLmt;
            _chargesProcInfo.PidNo = Lincense.PidNo;
            _chargesProcInfo.RemainingTime = Lincense.TimeLmt;
            _chargesProcInfo.OperationTime = currentTime;
            _chargesProcInfo.OperationType = operationType;
            return SqlLiteHelper.SaveChargesProcInfo(_chargesProcInfo);
        }

        public void StopCharging()
        {
            IsRunning = false;
            try
            {
                //string endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //_trnLicense.TrainDetail.TrainEndTs = DateTime.Parse(endTime);
            HideChargeControl();
            SaveChargProcInfo("结束训练");
                _trnLicenseCollection[_indexOfLicense] = _trnLicense;
                _usbHelper.CreateLogFileAndWriteLog(_mLicensePath,JsonConvert.SerializeObject(_trnLicenseCollection), _usbSerialNumber);
            }
            catch(Exception ex)
            {
                _sw.WriteLine("写许可时遇到错误：" + ex.Message + ",堆栈：" + ex.StackTrace);
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

        private void ChargingTriesThread(object chargingTries)
        {
            bool isTries = (bool)chargingTries;
            _sw.WriteLine(DateTime.Now.ToLocalTime() + ":当前计费线程：次数，" + "计费前剩余余额：" + _trnLicense.AccountBalance + "，当前计费模式:" + _trnLicense.ChargeMode);
            while (IsRunning)
            {
                if (_trainProcQueue.Count > 0)
                {
                    _sw.WriteLine(DateTime.Now.ToLocalTime() + ":计费线程：" + "发现" + _trainProcQueue.Count + "条过程数据，开始处理……");
                    TrainProc tp;
                    if (!_trainProcQueue.TryDequeue(out tp) || tp == null)
                    {
                        throw new InvalidOperationException("队列处理失败");
                    }
                    _sw.WriteLine(DateTime.Now.ToLocalTime() + ":计费线程：" + "过程数据:" + tp.Code + "," + tp.Type);
                    SaveTrainingInfo(tp);
                    SqlLiteHelper.SaveTraningProInfo(_trnLicense.PidNo, tp);
                    writeToDisk();
                    if (tp.Code == "10000" && tp.Type == "S")
                    {
                        _sw.WriteLine(DateTime.Now.ToLocalTime() + ":计费线程：" + "过程数据为10000，状态为S,初始化状态信息……");
                        _trnLicense.TriesLmt = _trnLicense.TriesLmt--;
                        _trnLicense.TrainDetail[_trnLicense.TrainDetail.Count - 1].TrainTries += 1;
                        _controlInfo.Tries = _trnLicense.TrainDetail[_trnLicense.TrainDetail.Count - 1].TrainTries;
                        if (isTries)
                        {
                            _trnLicense.AccountBalance = _trnLicense.AccountBalance - _trnLicense.ChargingStandard;
                            _controlInfo.Balance = _trnLicense.AccountBalance;
                            SqlLiteHelper.SaveTranineesInfo(_trnLicense);
                        }
                        
                    }
                    if (tp.Code == "10000" && tp.Type == "E")
                    {
                        if (isTries)
                        {
                            if (_trnLicense.CheckLicense()== LicenseState.Invaild)
                            {
                                _sw.WriteLine(DateTime.Now.ToLocalTime() + ":计费线程：" + "训练次数为0，停止计费……");
                                IsRunning = false;
                                if (StopDelegate != null)
                                    StopDelegate();
                            }
                        }
                    }
                   
                   
                }
                Thread.Sleep(500);
            }
        }

        private void ChargingTimeThread(object chargingTime)
        {
            bool isTimesCharging = (bool)chargingTime;
            double totalTime = _trnLicense.TimeLmt;
            _sw.WriteLine(DateTime.Now.ToLocalTime() + ":当前计费线程：时间，" + "计费前剩余余额：" + _trnLicense.AccountBalance + "，当前计费模式:" + _trnLicense.ChargeMode);
            if (isTimesCharging)
            {
                _trnLicense.AccountBalance = _trnLicense.AccountBalance - _trnLicense.ChargingStandard;
                _controlInfo.Balance = _trnLicense.AccountBalance;
            }
            DateTime lastHandleTime = _trnLicense.TrainDetail[_trnLicense.TrainDetail.Count - 1].TrainStartTs;
            _trnLicense.TrainDetail[_trnLicense.TrainDetail.Count - 1].TrainEndTs = lastHandleTime.AddMinutes(1);
            _trnLicense.TrainDetail[_trnLicense.TrainDetail.Count - 1].TrainTime =
                (_trnLicense.TrainDetail[_trnLicense.TrainDetail.Count - 1].TrainEndTs
                - _trnLicense.TrainDetail[_trnLicense.TrainDetail.Count - 1].TrainStartTs).TotalMinutes;
            while (IsRunning)
            {

                DateTime now = DateTime.Now;
                TimeSpan ts = now - lastHandleTime;
                _controlInfo.Time = (now - _trnLicense.TrainDetail[_trnLicense.TrainDetail.Count - 1].TrainStartTs).TotalSeconds;

                if (isTimesCharging)
                {
                    if (ts.TotalMinutes >= _trnLicense.MinTimeUnit)
                    {
                        
                        lastHandleTime = _trnLicense.TrainDetail[_trnLicense.TrainDetail.Count - 1].TrainEndTs;
                        if (_trnLicense.CheckLicense() == LicenseState.Invaild)
                        {
                            IsRunning = false;
                            if (StopDelegate != null)
                                StopDelegate();
                            break;
                        }
                        _trnLicense.AccountBalance = _trnLicense.AccountBalance - _trnLicense.ChargingStandard * _trnLicense.MinTimeUnit;
                        _controlInfo.Balance = _trnLicense.AccountBalance;
                        _trnLicense.TrainDetail[_trnLicense.TrainDetail.Count - 1].TrainEndTs = lastHandleTime.AddMinutes(1);
                        _trnLicense.TrainDetail[_trnLicense.TrainDetail.Count - 1].TrainTime = 
                            (_trnLicense.TrainDetail[_trnLicense.TrainDetail.Count - 1].TrainEndTs 
                            - _trnLicense.TrainDetail[_trnLicense.TrainDetail.Count - 1].TrainStartTs).TotalMinutes;
                       
                        writeToDisk();
                        
                    }
                }
                //_currentInfoControl.CallChangeShowInfo(_controlInfo);
                #region
                //if (_trnLicense.TrainDetail != null)
                //{
                //    _trnLicense.TrainDetail.TrainTime = detailTime + diff;
                //    _trnLicense.TrainDetail.TrainMileage = detailMileage + _trnMileage;
                //    _trnLicense.TrainDetail.TrainTries = detailTries;
                //    _trnLicense.TrainDetail.AutoId = AutoId;
                //}
                //if (_currentInfoControl != null)
                //    _currentInfoControl.CallChangesShowInfo(_chargesDisplayInfo);
                #endregion


                Thread.Sleep(300);
            }

        }

        private void writeToDisk()
        {
            try
            {
            _trnLicenseCollection[_indexOfLicense] = _trnLicense;
            _usbHelper.CreateLogFileAndWriteLog(_mLicensePath,
                                                   JsonConvert.SerializeObject(_trnLicenseCollection), _usbSerialNumber);
        }
            catch(Exception ex)
            {
                IsRunning = false;
                if (StopDelegate != null)
                    StopDelegate();
            }
        }

        private void SaveTrainingInfo(TrainProc tpInfo)
        {
            if (_trnLicense != null && _trnLicense.TrainDetail != null)
                _trnLicense.TrainDetail[_trnLicense.TrainDetail.Count - 1].TrainProcList.Add(tpInfo);
        }

        public void Dispose()
        {
            _sw.Close();
            _sw.Dispose();
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
