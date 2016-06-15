using DBAccessProc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrainingControl;

namespace TrainingAccounter
{
	/// <summary>
	/// BulkBooking.xaml 的交互逻辑
	/// </summary>
	public partial class BulkBooking : Page
	{
		DsRsrc dsrsrc;
		public BulkBooking(DsRsrc dsr)
		{
			dsrsrc = dsr;	
			InitializeComponent();
			if (dsrsrc.trainMangeDataSet.DrvSchoolDataTable.Rows.Count > 0)
				cboxSerDrv.ItemsSource = dsrsrc.trainMangeDataSet.DrvSchoolDataTable.DefaultView;
			if (dsrsrc.TrainLicenseType!=null)
			{ 
              this.cboxSerCarType.ItemsSource = dsrsrc.TrainLicenseType;
              this.cboxSerCarType.SelectedIndex = 0;
			}       
            cboxSubject.SelectedIndex = 0;    
			if (dsrsrc.sBillingMode == "Tries")
                cboxBillModeBook.Text= "按次数计费";
			else if(dsrsrc.sBillingMode=="Time")
                cboxBillModeBook.Text = "按时间计费";
            else if (dsrsrc.sBillingMode == "StudyTime")
            {
                cboxBillModeBook.Text = "按学时计费";
            }
            else
            {
                cboxBillModeBook.Text = "按里程计费";            
            }
			sChargingMode = dsrsrc.sBillingMode;
		 
			cboxList.IsChecked = true;
		
        }
		string sChargingMode ="";
        string sRunningMode = DBAccessProc.Common.RunningMode;
		private void btnSearchTrainee_Click(object sender, RoutedEventArgs e)
		{
			PopuBulkBooking();
		}
		private void PopuBulkBooking()
		{
			dsrsrc.SearchTrainee(tboxSerPidNo.Text.Trim(), tboxSerName.Text.Trim(), "", cboxSerCarType.SelectedValue == null ? "" : cboxSerCarType.SelectedValue.ToString(), cboxSerDrv.SelectedValue == null ? -1 : int.Parse(cboxSerDrv.SelectedValue.ToString()), DateTime.Parse(dtFrom.SelectedDate.ToString()), DateTime.Parse(dtTo.SelectedDate.ToString()), "0");
			//	DataRow[] booksDb = dsrsrc.trainMangeDataSet.TraineeDataTable.Select();			

			dgSearchTrainee.ItemsSource = dsrsrc.trainMangeDataSet.TraineeDataTable.DefaultView;
			if (dsrsrc.displayBookhistory("", "", DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"))) != null)
			{
				dgBookRecord.ItemsSource = dsrsrc.displayBookhistory("", "", DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"))).DefaultView;
			}
			else
				dgBookRecord.ItemsSource = null;

		}

		private void tboxTraCnt_KeyDown(object sender, KeyEventArgs e)
		{
			TextAllowInputInt(sender, e);
		}

		private void tboxTraMil_KeyDown(object sender, KeyEventArgs e)
		{
			TextAllowInputIntAndDecimal(sender, e);
		}

		private void btnBook_Click(object sender, RoutedEventArgs e)
		{
			List<string> listPidNo = new List<string>();
			if (dsrsrc.displayBookhistory("", "", DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"))) != null)
			{
				if (dsrsrc.trainMangeDataSet.BookTrainingDataTable.Rows.Count > 0)
				{
					foreach (DataRow item in dsrsrc.trainMangeDataSet.BookTrainingDataTable.Rows)
					{
						if (item["CHECKSTATUS"].ToString() == "UK" || item["CHECKSTATUS"].ToString() == "CK")
						{
							if (!listPidNo.Contains(item["PID_NO"].ToString()))
								listPidNo.Add(item["PID_NO"].ToString());
						}
					}
				}
			}

			if (dgBookRecord.Items.Count > 0)
			{
				foreach (DataRowView item in dgBookRecord.Items)
				{
					if (item["CHECKSTATUS"].ToString() == "未签到" || item["CHECKSTATUS"].ToString() == "已签到")
					{
						if (!listPidNo.Contains(item["PID_NO"].ToString()))
							listPidNo.Add(item["PID_NO"].ToString());
					}
				}
			}
			TrainingControl.TraBook TraBookInfo = new TrainingControl.TraBook();
			var examCd = cboxSubject.SelectedItem != null ? (cboxSubject.SelectedItem as ComboBoxItem).Content.ToString() : "";
			if (examCd == "科目二")
				TraBookInfo.ExamCd = "RE";
			else if (examCd == "科目三")
				TraBookInfo.ExamCd = "SE";

			double dSingleTime = 0;
			Double traAmount = 0;
			double dChargingStandard = dsrsrc.GetChargingStandard(sChargingMode, cboxSerCarType.SelectedValue.ToString());
			double dBalance = 0;//账户余额
			TraBookInfo.TriesAmount = 0;
			TraBookInfo.MileageAmount = 0;
			Double cunAmount = 0;//剩余金额
			for (int count = 0; count < dsrsrc.trainMangeDataSet.Tra_Setting.Rows.Count; count++)
			{
				string sRsTxt = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_TXT"].ToString();
				string sRsValue = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_VALUE"].ToString();
				string sCarType = "";
				if (sRsTxt.Contains("SingleTime"))
				{
					sCarType = sRsTxt.Substring(10, 2);
					if (sCarType == cboxSerCarType.SelectedValue.ToString())
					{
						dSingleTime = Math.Round(double.Parse(sRsValue) / 60, 2);
						continue;
					}
				}
			}
			if (cboxBillModeBook.Text == "按时间计费")
			{
				double timeLenth = 0;
				sChargingMode = "Time";
				if (tboxTraTime.Text.Trim() == "")
				{ tboxTraTime.Text = "0"; }
				else
				{
					//	System.Windows.MessageBox.Show("请填入训练时间！");
					//	tboxTraTime.Focus();
					//	return;
					//}
					//else			
					if (!double.TryParse(tboxTraTime.Text.Trim(), out timeLenth))
					{
						System.Windows.MessageBox.Show("输入的时间数不合法！");
						tboxTraTime.Text = "";
						tboxTraTime.Focus();
						return;
					}
				}
				traAmount = timeLenth;
				TraBookInfo.TraAmount = timeLenth.ToString();
				TraBookInfo.ThisBalance = Math.Round(dChargingStandard * traAmount, 2);//本次预约费用
			}
			else if (cboxBillModeBook.Text == "按次数计费")
			{
				sChargingMode = "Tries";
				if (tboxTraCnt.Text.Trim() == "")
				{
					System.Windows.MessageBox.Show("请填入训练次数！");
					tboxTraCnt.Focus();
					return;
				}
				else if (Convert.ToInt32(tboxTraCnt.Text.Trim()) <= 0)
				{
					System.Windows.MessageBox.Show("输入的次数不合法！");
					tboxTraCnt.Text = "";
					tboxTraCnt.Focus();
					return;
				}
				TraBookInfo.TriesAmount = Convert.ToInt32(tboxTraCnt.Text.Trim());
				TraBookInfo.TraAmount = dSingleTime.ToString();
				TraBookInfo.ThisBalance = Math.Round(dChargingStandard * TraBookInfo.TriesAmount, 2);//本次预约费用
			}
            else if (cboxBillModeBook.Text == "按里程计费")
            {
                sChargingMode = "Mileage";
                TraBookInfo.TraAmount = dSingleTime.ToString();
                if (tboxTraMil.Text.Trim() == "")
                {
                    System.Windows.MessageBox.Show("请填入训练里程！");
                    tboxTraMil.Focus();
                    return;
                }
                else if (Convert.ToSingle(tboxTraMil.Text.Trim()) <= 0)
                {
                    System.Windows.MessageBox.Show("输入的里程数不合法！");
                    tboxTraMil.Text = "";
                    tboxTraMil.Focus();
                    return;
                }
                TraBookInfo.MileageAmount = Convert.ToSingle(tboxTraMil.Text.Trim());
                TraBookInfo.TraAmount = (dSingleTime * TraBookInfo.MileageAmount).ToString();
                TraBookInfo.ThisBalance = Math.Round(dChargingStandard * TraBookInfo.MileageAmount, 2);//本次预约费用
            }
            else
            {
                double timeLenth = 0;
                sChargingMode = "StudyTime";
                if (tboxTraStudyTime.Text.Trim() == "")
                { tboxTraStudyTime.Text = "0"; }
                else
                {
                    //	System.Windows.MessageBox.Show("请填入训练时间！");
                    //	tboxTraTime.Focus();
                    //	return;
                    //}
                    //else			
                    if (!double.TryParse(tboxTraStudyTime.Text.Trim(), out timeLenth))
                    {
                        System.Windows.MessageBox.Show("输入的时间数不合法！");
                        tboxTraStudyTime.Text = "";
                        tboxTraStudyTime.Focus();
                        return;
                    }
                }
                traAmount = timeLenth;
                TraBookInfo.TraAmount = timeLenth.ToString();
                TraBookInfo.ThisBalance = Math.Round(dChargingStandard * traAmount, 2);//本次预约费用
            }

			TraBookInfo.BillMode = sChargingMode;
			TraBookInfo.TrainDt = DateTime.Now.ToString("yyyy-MM-dd");
			TraBookInfo.Session = "";
			TraBookInfo.TraCarNo = CboxAutoId.Text.ToString();
			TraBookInfo.TraCarPlateNo = "";
			TraBookInfo.TrainerNames = CboxTrainer.Text.ToString();
			TraBookInfo.CarBrand = "";
			TraBookInfo.PhoneNo = "";
			if (dgSearchTrainee.SelectedItems.Count == 0)
			{
				MessageBox.Show("请先选中学员再点击预约！");
				return;
			}
			else
			{
				int count = dgSearchTrainee.SelectedItems.Count;
				DataRowView[] drv = new DataRowView[count];
				for (int i = 0; i < count; i++)
				{
					drv[i] = dgSearchTrainee.SelectedItems[i] as DataRowView;
					TraBookInfo.PidNo = drv[i].Row["PID_NO"].ToString();
					TraBookInfo.Name = drv[i].Row["TRAINE_NAME"].ToString();
					TraBookInfo.CarType = drv[i].Row["LICENSE_TYPE_CD"].ToString();
					TraBookInfo.Balance = drv[i].Row["BALANCE"].ToString();
					TraBookInfo.ThisBalance = double.Parse(drv[i].Row["BALANCE"].ToString());
					if (listPidNo.Contains(TraBookInfo.PidNo))
					{
						MessageBox.Show("已经包含该学员的预约信息，无法预约姓名为：" + TraBookInfo.Name + "的学员");
						continue;
					}
					//cunAmount = 0;
					//Math.Round(Double.Parse(TraBookInfo.Balance) - TraBookInfo.ThisBalance, 2);//剩余金额                  
					//if (cunAmount >= 0)
					//	TraBookInfo.Balance = Math.Round(cunAmount, 2).ToString();//余额
					//else
					//{
					if (TraBookInfo.Balance == "0")
					{
						System.Windows.MessageBox.Show("学员：" + TraBookInfo.Name + " 余额不足！无法预约");
						continue;
					}
					dsrsrc.AddTraBookInfo("");
					if (cboxBillModeBook.Text== "按时间计费")
						dsrsrc.InsRechargeRecord(TraBookInfo.ThisBalance, TraBookInfo.PidNo, "", "预约 " + TraBookInfo.TraAmount + " 小时");
					else if (cboxBillModeBook.Text== "按次数计费")
						dsrsrc.InsRechargeRecord(TraBookInfo.ThisBalance, TraBookInfo.PidNo, "", "预约 " + TraBookInfo.TriesAmount + " 次");
					else if (cboxBillModeBook.Text == "按里程计费")
						dsrsrc.InsRechargeRecord(TraBookInfo.ThisBalance, TraBookInfo.PidNo, "", "预约 " + TraBookInfo.MileageAmount + " 公里");
                    else
                        dsrsrc.InsRechargeRecord(TraBookInfo.ThisBalance, TraBookInfo.PidNo, "", "预约 " + TraBookInfo.TraAmount + " 个学时");
				}


			}
			PopuBulkBooking();
		}

		private void btnCancelBook_Click(object sender, RoutedEventArgs e)
		{

			if (dgBookRecord.SelectedItems.Count >= 1)
			{

				if (System.Windows.MessageBox.Show("是否取消该预约？", "取消预约", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					for (int i = 0; i < dgBookRecord.SelectedItems.Count; i++)
					{
						var tempA = (DataRowView)dgBookRecord.SelectedItems[i];
						if (tempA["CHECKSTATUS"].ToString() == "未签到")
						{
							double dSeqNo = Convert.ToDouble(tempA["SEQ_NO"].ToString());
							dsrsrc.GetTraBookBySeqNo(dSeqNo);
							double dBalanceCancel = 0;
							dBalanceCancel = Convert.ToDouble(dsrsrc.trainMangeDataSet.BookTrainingDataTable.Rows[0]["THIS_BALANCE"]);
							dsrsrc.TraBookCancel(tempA["SEQ_NO"].ToString());
							dsrsrc.RechargeAccount("", dBalanceCancel, tempA["PID_NO"].ToString(), "", "取消预约返还");

						}
						else if (tempA["CHECKSTATUS"].ToString() == "已签到")
						{
							System.Windows.MessageBox.Show("本次预约已经签到，不能取消预约！");
							continue;
						}
						else if (tempA["CHECKSTATUS"].ToString() == "已结算")
						{
							System.Windows.MessageBox.Show("本次预约已经结算，不能取消预约！");
							continue;
						}
						else if (tempA["CHECKSTATUS"].ToString() == "训练中")
						{
							System.Windows.MessageBox.Show("本次预约正在训练中，不能取消预约！");
							continue;
						}
						else if (tempA["CHECKSTATUS"].ToString() == "已结束")
						{
							System.Windows.MessageBox.Show("本次预约已完成训练，不能取消预约！");
							continue;
						}
						else if (tempA["CHECKSTATUS"].ToString() == "已取消")
						{
							System.Windows.MessageBox.Show("本次预约已取消！");
							continue;
						}
					}
				}			
				PopuBulkBooking();
			}
			else
			{
				MessageBox.Show("没有选中任何行。");
				return;
			}
		}

		/// <summary>
		/// 屏蔽非法按键，只能输入整数
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void TextAllowInputInt(object sender, System.Windows.Input.KeyEventArgs e)
		{
			System.Windows.Controls.TextBox txt = sender as System.Windows.Controls.TextBox;
			//屏蔽非法按键，只能输入整数
			if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
			{
				e.Handled = false;
			}
			else if (((e.Key >= Key.D0 && e.Key <= Key.D9)) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
			{
				e.Handled = false;
			}
			else
			{
				e.Handled = true;
			}
		}
		/// <summary>
		/// //屏蔽非法按键，只能输入整数小数
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void TextAllowInputIntAndDecimal(object sender, System.Windows.Input.KeyEventArgs e)
		{
			System.Windows.Controls.TextBox txt = sender as System.Windows.Controls.TextBox;
			//屏蔽非法按键，只能输入数字
			if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Decimal)
			{
				if (txt.Text.Contains(".") && e.Key == Key.Decimal)
				{
					e.Handled = true;
					return;
				}
				e.Handled = false;
			}
			else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.OemPeriod) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
			{
				if (txt.Text.Contains(".") && e.Key == Key.OemPeriod)
				{
					e.Handled = true;
					return;
				}
				e.Handled = false;
			}
			else
			{
				e.Handled = true;
			}
		}

		private void btnExportTraLic_Click(object sender, RoutedEventArgs e)
		{
			Dictionary<string, string> listSeqNo = new Dictionary<string, string>();
			int count = dgBookRecord.SelectedItems.Count;
			if (count <= 0)
				return;
			DataRowView[] drv = new DataRowView[count];
			for (int i = 0; i < count; i++)
			{
				drv[i] = dgBookRecord.SelectedItems[i] as DataRowView;
				listSeqNo.Add(drv[i].Row["PID_NO"].ToString(), drv[i].Row["SEQ_NO"].ToString());
			}
			if (listSeqNo.Count > 0)
			{
				var s = DriveInfo.GetDrives();
				List<string> uName = new List<string>();
				foreach (var drive in s)
				{
					if (drive.DriveType == DriveType.Removable)
					{
						uName.Add(drive.Name);
					}
				}
				if (uName.Count > 0)
				{
					WriteTrainLicense(listSeqNo);
				}
				else
				{
					MessageBox.Show("没有可用的移动设备。");
					return;
				}
			}
			PopuBulkBooking();
		}

		/// <summary>
		/// 写入许可的方法
		/// </summary>
		/// <param name="sPidNo"></param>
		/// <param name="iSeqNo"></param>
		public void WriteTrainLicense(Dictionary<string, string> listSeq)
		{
			List<TrainLicense> m_listTraLic = new List<TrainLicense>();
			string sJsonTraLic = "";
			TrainLicense traLic = null;
			foreach (var item in listSeq.Keys)
			{
				dsrsrc.trainMangeDataSet.TraTrainLicenseDataTable.Clear();
				DBAccessHelper.getTrainLicense(int.Parse(listSeq[item]), "", dsrsrc.trainMangeDataSet);
				foreach (DataRow dr in dsrsrc.trainMangeDataSet.TraTrainLicenseDataTable.Rows)
				{
					traLic = new TrainLicense();
					traLic.TraBookSeqNo = int.Parse(dr["BookSeqNo"].ToString());
					traLic.TimeLmt = Double.Parse(dr["TimeLmt"].ToString());
					traLic.TriesLmt = int.Parse(dr["TriesLmt"].ToString());
					traLic.MileageLmt = Double.Parse(dr["MileageLmt"].ToString());
					traLic.AutoId = dr["AutoId"].ToString().Trim();
					traLic.AutoType = dr["AutoType"].ToString();
					traLic.ChargeMode = dr["ChargeMode"].ToString();

					traLic.Date = dr["Date"].ToString();
					traLic.Fingerprint = "";// dr["Fingerprint"].ToString();
					traLic.Name = dr["Name"].ToString();
					traLic.Photo = "";// dr["Photo"].ToString();
					traLic.PidNo = dr["PidNo"].ToString();

					traLic.Session = dr["Session"].ToString();
					traLic.Trainer = dr["Trainer"].ToString();
					traLic.TrainDetail = null;

					traLic.StudentType = int.Parse(dr["StudentType"].ToString() == "" ? "0" : dr["StudentType"].ToString());
					traLic.MinTimeUnit = int.Parse(dr["MinTimeUnit"].ToString());
					traLic.ChargingStandard = double.Parse(dr["ChargingStandard"].ToString());
					traLic.AccountBalance = double.Parse(dr["AccountBalance"].ToString());

					m_listTraLic.Add(traLic);
				}
			}
			sJsonTraLic = JsonConvert.SerializeObject(m_listTraLic);
			WriteLicenseWindow writeLicWin = new WriteLicenseWindow(sJsonTraLic, listSeq);
			writeLicWin.ShowDialog();
		}

		private void cboxSerCarType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cboxSerCarType.SelectedValue != null)
			{
				//教练员显示
				if (dsrsrc.trainMangeDataSet.TrainerDataTable.Rows.Count > 0)
				{
					this.CboxTrainer.DisplayMemberPath = "TRAINER_NAME";
					DataTable dt2 = dsrsrc.trainMangeDataSet.TrainerDataTable.Clone();
					DataRow[] dr2 = dsrsrc.trainMangeDataSet.TrainerDataTable.Select("AllowableLicenseType='" + this.cboxSerCarType.SelectedValue.ToString().Trim() + "'");
					if (dr2.Length > 0)
					{
						foreach (DataRow ss in dr2)
						{
							dt2.Rows.Add(ss.ItemArray);
						}
						dt2 = dt2.DefaultView.ToTable(true, "TRAINER_NAME");
						this.CboxTrainer.ItemsSource = dt2.DefaultView;
						this.CboxTrainer.SelectedIndex = 0;
					}

				}
				//车号显示    
				if (dsrsrc.trainMangeDataSet.TraCarDataTable.Rows.Count > 0)
				{
					this.CboxAutoId.DisplayMemberPath = "AUTO_ID";
					DataTable dt2 = dsrsrc.trainMangeDataSet.TraCarDataTable.Clone();
					DataRow[] dr2 = dsrsrc.trainMangeDataSet.TraCarDataTable.Select("AUTO_TYPE_CD='" + this.cboxSerCarType.SelectedValue.ToString().Trim() + "'");
					if (dr2.Length > 0)
					{
						foreach (DataRow ss in dr2)
						{
							dt2.Rows.Add(ss.ItemArray);
						}
						dt2 = dt2.DefaultView.ToTable(true, "AUTO_ID");
						this.CboxAutoId.ItemsSource = dt2.DefaultView;
						this.CboxAutoId.SelectedIndex = 0;
					}
				}
			}



		}

		private void cboxList_Checked(object sender, RoutedEventArgs e)
		{
			dsrsrc.IsdisplayCk = true;
			if (dsrsrc.displayBookhistory("", "", DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"))) != null)
				dgBookRecord.ItemsSource = dsrsrc.displayBookhistory("", "", DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"))).DefaultView;
		}

		private void cboxList_Unchecked(object sender, RoutedEventArgs e)
		{
			dsrsrc.IsdisplayCk = false;
			if (dsrsrc.displayBookhistory("", "", DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"))) != null)
				dgBookRecord.ItemsSource = dsrsrc.displayBookhistory("", "", DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"))).DefaultView;
		}

      
        private void cboxBillModeBook_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cboxBillModeBook.Text == "按时间计费")
            {
                sChargingMode = "Time";
                tboxTraCnt.IsEnabled = false;
                tboxTraMil.IsEnabled = false;
                tboxTraCnt.Text = "";
                tboxTraMil.Text = "";
                tboxTraTime.IsEnabled = true;
                tboxTraTime.Text = "";
                tboxTraTime.Focus();
                tboxTraStudyTime.IsEnabled = false;
            }
            else if (cboxBillModeBook.Text == "按次数计费")
            {
                sChargingMode = "Tries";
                tboxTraCnt.IsEnabled = true;
                tboxTraMil.IsEnabled = false;
                tboxTraCnt.Text = "";
                tboxTraMil.Text = "";
                tboxTraCnt.Focus();
                tboxTraTime.Text = "";
                tboxTraTime.IsEnabled = false;
                tboxTraStudyTime.IsEnabled = false;
            }
            else if (cboxBillModeBook.Text == "按里程计费")
            {
                sChargingMode = "Mileage";
                tboxTraCnt.IsEnabled = false;
                tboxTraMil.IsEnabled = true;
                tboxTraCnt.Text = "";
                tboxTraMil.Text = "";
                tboxTraMil.Focus();
                tboxTraTime.Text = "";
                tboxTraTime.IsEnabled = false;
                tboxTraStudyTime.IsEnabled = false;
            }
            else
            {
                sChargingMode = "StudyTime";
                tboxTraStudyTime.IsEnabled = true;
                tboxTraCnt.IsEnabled = false;
                tboxTraMil.IsEnabled = false;
                tboxTraCnt.Text = "";
                tboxTraMil.Text = "";
                tboxTraStudyTime.Focus();
                tboxTraTime.Text = "";
                tboxTraTime.IsEnabled = false;
            }
        }

        private void tboxTraStudyTime_KeyDown(object sender, KeyEventArgs e)
        {
            TextAllowInputIntAndDecimal(sender, e);
        }

      
	}
}
