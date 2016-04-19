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
	/// bookMange.xaml 的交互逻辑
	/// </summary>
	public partial class bookMange : Page
	{
		public bookMange(DsRsrc dsr)
		{
			InitializeComponent();
			dsrsrc = dsr;
			dsrsrc.GetAllSetting();
			if (dsrsrc.trainMangeDataSet.Tra_Setting.Rows.Count > 0)
			{
				DataRow[] drv=dsrsrc.trainMangeDataSet.Tra_Setting.Select("RS_TXT='StartTime'");
				if(drv.Length>0)
					nStratWT=int.Parse(drv[0]["RS_VALUE"].ToString().Split(':')[0]);
				DataRow[] drvs=dsrsrc.trainMangeDataSet.Tra_Setting.Select("RS_TXT='Duration'");
				if(drvs.Length>0)
					workLong = int.Parse(drvs[0]["RS_VALUE"].ToString());
				DataRow[] drvm = dsrsrc.trainMangeDataSet.Tra_Setting.Select("RS_TXT='ChargingMode'");
				if (drvm.Length > 0)
					sChargingMode = drvm[0]["RS_VALUE"].ToString();
			}
			InitialControl();
			tboxpidno.SetBinding(TextBox.TextProperty, new Binding("PidNo") { Source = dsrsrc });
		}
		DsRsrc dsrsrc;
		string sChargingMode = string.Empty;
		int nStratWT;
		int workLong;
		Double dTimeAmount;//预约时间总量
		Boolean isAdd = true;
		List<string> ltcar = new List<string>();
		Dictionary<string, string> bookdata = new Dictionary<string, string>();
		private void btnbook_Click(object sender, RoutedEventArgs e)
		{
			//文本框           
			TrainingControl.TraBook TraBookInfo = new TrainingControl.TraBook();
			var examCd = cboxSubject.SelectedItem != null ? (cboxSubject.SelectedItem as ComboBoxItem).Content.ToString() : "";
			if (examCd == "科目二")
				TraBookInfo.ExamCd = "RE";
			else if (examCd == "科目三")
				TraBookInfo.ExamCd = "SE";
			else
				TraBookInfo.ExamCd = "";
			if (tboxpidno.Text.Trim() == "" || tboxVehicleModel.Text.Trim() == "")
			{
				System.Windows.MessageBox.Show("请先查询一个学员后再预约！");
				return;
			}
			if (dataGridHistory.Items.Count > 0)
			{
				foreach (var item in dataGridHistory.Items)
				{
					var itemRow = item as DataRowView;
					if (itemRow.Row["CHECKSTATUS"].ToString() == "已结算" || itemRow.Row["CHECKSTATUS"].ToString() == "已取消")
						continue;
					else
					{
						MessageBox.Show("存在未结束的预约，无法进行新的预约，请先结算或者取消未结束的预约");
						return;
					}
				}
			}
			TraBookInfo.Name = tblockName.Text;
			TraBookInfo.PidNo = this.tboxpidno.Text.Trim();
			TraBookInfo.PhoneNo = "";
			Boolean ischecked = false;
			double dBalance = 0;
			double dChargingStandard = 0;
			dsrsrc.getTraStudinfo(TraBookInfo.PidNo);
			if (dsrsrc.trainMangeDataSet.TraineeDataTable.Rows.Count > 0)
			{
				dBalance = double.Parse(dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["BALANCE"].ToString());//账户余额
				dChargingStandard = dsrsrc.GetChargingStandard(sChargingMode,dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["LICENSE_TYPE_CD"].ToString());
				if (dBalance <= 0)
				{
					TraBookInfo.Balance = "0";
					MessageBox.Show("余额不足，无法预约");
					return;
				}
				else
				{
					TraBookInfo.Balance = Math.Round(dBalance, 2).ToString();//预约的余额
				}
			}
			else
			{
				MessageBox.Show("获取余额信息失败，请重试");
				return;
			}
			if (tboxBillModeBook.Text == "按时间计费")
			{
				//遍历所有checkbox判断共选择几项，位于哪一行
				bookdata.Clear();
				UIElementCollection Childrens = canvas1.Children;
				foreach (UIElement ui in Childrens)
				{
					string bookdt = string.Empty;
					string cession = string.Empty;
					var cb = (System.Windows.Controls.CheckBox)ui;
					if (cb.IsChecked == true && cb.Opacity == 1)
					{
						ischecked = true;
						int m = int.Parse(cb.Name.Substring(9, cb.Name.Length - 9));
						int nMod = m % 2;
						int nQuo = (m - 1) / 2;

						if (cb.Name.Substring(8, 1) == "1")
						{
							bookdt = DateTime.Now.ToString("yyyy-MM-dd");
						}
						else if (cb.Name.Substring(8, 1) == "2")
						{
							bookdt = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
						}
						else if (cb.Name.Substring(8, 1) == "3")
						{
							bookdt = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");
						}
						else if (cb.Name.Substring(8, 1) == "4")
						{
							bookdt = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd");
						}
						else if (cb.Name.Substring(8, 1) == "5")
						{
							bookdt = DateTime.Now.AddDays(4).ToString("yyyy-MM-dd");
						}
						else if (cb.Name.Substring(8, 1) == "6")
						{
							bookdt = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd");
						}
						else if (cb.Name.Substring(8, 1) == "7")
						{
							bookdt = DateTime.Now.AddDays(6).ToString("yyyy-MM-dd");
						}
						if (nMod == 1)
						{
							cession = nStratWT + nQuo + "-0;";
						}
						else
						{
							cession = nStratWT + nQuo + "-1;";
						}
						if (!bookdata.Keys.Contains(bookdt))
						{
							bookdata.Add(bookdt, cession);
						}
						else
						{
							cession = bookdata[bookdt] + cession;
							bookdata[bookdt] = cession;
						}
					}
				}
				//if (!ischecked )   //限制预约必须选择
				//{
				//	System.Windows.MessageBox.Show("没有选中任何时间段，请先进行选择。");
				//	return;
				//}
			}
			//预约总次数或者时间 \预约包含的日期    
			Double traAmount = 0;
			string m_dDatePicker = string.Empty;
			string m_cession = string.Empty;
			if (this.lboxBooktype.Items.Count <= 0)
			{
				MessageBox.Show("列表中没有可用的训练车，请先添加当前车型的训练车辆信息");
				return;
			}
			if (this.lboxBooktype.SelectedItems.Count > 0)
			{
				if (this.lboxBooktype.SelectedItems[0].ToString().Trim().Split('-')[0] != "")
					TraBookInfo.TraCarNo = this.lboxBooktype.SelectedValue.ToString().Trim().Split('-')[0];
				else
				{
					MessageBox.Show("请先选择要预约的训练车，然后再进行预约");
					return;
				}

				TraBookInfo.TraCarPlateNo = this.lboxBooktype.SelectedValue.ToString().Trim().Split('-')[1];
			}

			TraBookInfo.TraSubject = this.cboxSubject.SelectedValue.ToString(); //训练科目   
			TraBookInfo.CarType = this.tboxVehicleModel.Text.Trim();
			TraBookInfo.TrainerNames = this.lboxBooktype.SelectedValue != null ? this.lboxBooktype.SelectedValue.ToString().Trim().Split('-')[2] : "";
			TraBookInfo.CarBrand = this.cboxBookBrand.SelectedValue != null ? (cboxBookBrand.SelectedItem as System.Data.DataRowView).Row["BRAND"].ToString() : "";
			TraBookInfo.TriesAmount = 0;
			TraBookInfo.MileageAmount = 0;
			double dSingleTime = 0;
			//账户余额和预约所需金额差   
			if (dsrsrc.trainMangeDataSet.Tra_Setting.Rows.Count > 0)
			{
				for (int count = 0; count < dsrsrc.trainMangeDataSet.Tra_Setting.Rows.Count; count++)
				{
					string sRsTxt = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_TXT"].ToString();
					string sRsValue = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_VALUE"].ToString();
					string sCarType = "";
					if (sRsTxt.Contains("SingleTime"))
					{
						sCarType = sRsTxt.Substring(10, 2);
						if (sCarType == TraBookInfo.CarType)
						{
							dSingleTime = Math.Round(double.Parse(sRsValue) / 60, 2);
							continue;
						}
					}
				}
				if (dSingleTime == 0 && tboxBillModeBook.Text != "按时间计费")
				{
					System.Windows.MessageBox.Show("请先设置 " + TraBookInfo.CarType + " 车型的单次训练限制时间！");
					return;
				}
			}
			else
			{
				MessageBox.Show("没有找到训练需要的基础配置信息，请检查配置");
				return;
			}
			if (tboxBillModeBook.Text == "按时间计费")
			{
				if (bookdata.Count > 1)
				{
					System.Windows.MessageBox.Show("只能预约同一天的时段！");
					return;
				}
				foreach (string temp in bookdata.Keys)
				{
					traAmount += (bookdata[temp].Split(';').Length - 1) * 0.5;
					//m_dDatePicker += temp + "|";
					m_dDatePicker = temp;
					m_cession += temp + ":" + bookdata[temp] + "|";
				}
				TraBookInfo.TraAmount = traAmount.ToString();
				TraBookInfo.TrainDt = m_dDatePicker == "" ? DateTime.Now.ToString("yyyy-MM-dd") : m_dDatePicker;
				TraBookInfo.Session = m_cession;
				TraBookInfo.BillMode = "Time";
				//  cunAmount = Math.Round(dBalance - (dChargingStandard * traAmount), 2);//剩余金额
				TraBookInfo.ThisBalance = Math.Round(dChargingStandard * traAmount, 2);//本次预约费用
			}
			else if (tboxBillModeBook.Text == "按次数计费")
			{
				TraBookInfo.TrainDt = DateTime.Now.ToString("yyyy-MM-dd");
				TraBookInfo.Session = "";
				TraBookInfo.BillMode = "Tries";
				//if (tboxTraCnt.Text.Trim() == "")
				//{
				//	System.Windows.MessageBox.Show("请填入训练次数！");
				//	tboxTraCnt.Focus();
				//	return;
				//}
				if (Convert.ToInt32(tboxTraCnt.Text.Trim()) <= 0)
				{
					System.Windows.MessageBox.Show("输入的次数不合法！");
					tboxTraCnt.Text = "";
					tboxTraCnt.Focus();
					return;
				}
				TraBookInfo.TriesAmount = Convert.ToInt32(tboxTraCnt.Text.Trim());
				//TraBookInfo.TraAmount = (dSingleTime * TraBookInfo.TriesAmount).ToString();
				TraBookInfo.TraAmount = dSingleTime.ToString();
				// cunAmount = Math.Round(dBalance - (dChargingStandard * TraBookInfo.TriesAmount), 2);//剩余金额
				TraBookInfo.ThisBalance = Math.Round(dChargingStandard * TraBookInfo.TriesAmount, 2);//本次预约费用
			}
			else if (tboxBillModeBook.Text == "按里程计费")
			{
				TraBookInfo.TraAmount = dSingleTime.ToString();
				TraBookInfo.TrainDt = DateTime.Now.ToString("yyyy-MM-dd");
				TraBookInfo.Session = "";
				TraBookInfo.BillMode = "Mileage";
				//if (tboxTraMil.Text.Trim() == "")
				//{
				//	System.Windows.MessageBox.Show("请填入训练里程！");
				//	tboxTraMil.Focus();
				//	return;
				//}
				if (Convert.ToSingle(tboxTraMil.Text.Trim()) <= 0)
				{
					System.Windows.MessageBox.Show("输入的里程数不合法！");
					tboxTraMil.Text = "";
					tboxTraMil.Focus();
					return;
				}
				TraBookInfo.MileageAmount = Convert.ToSingle(tboxTraMil.Text.Trim());
				TraBookInfo.TraAmount = (dSingleTime * TraBookInfo.MileageAmount).ToString();
				// cunAmount = Math.Round(dBalance - (dChargingStandard * TraBookInfo.MileageAmount), 2);//剩余金额
				TraBookInfo.ThisBalance = Math.Round(dChargingStandard * TraBookInfo.MileageAmount, 2);//本次预约费用
			}
			dsrsrc.AddTraBookInfo("");
			if (tboxBillModeBook.Text == "按时间计费")
				dsrsrc.InsRechargeRecord(TraBookInfo.ThisBalance, TraBookInfo.PidNo, "", "预约 " + TraBookInfo.TraAmount + " 小时");
			else if (tboxBillModeBook.Text == "按次数计费")
				dsrsrc.InsRechargeRecord(TraBookInfo.ThisBalance, TraBookInfo.PidNo, "", "预约 " + TraBookInfo.TriesAmount + " 次");
			else if (tboxBillModeBook.Text == "按里程计费")
				dsrsrc.InsRechargeRecord(TraBookInfo.ThisBalance, TraBookInfo.PidNo, "", "预约 " + TraBookInfo.MileageAmount + " 公里");
			//重新刷新界面
			PopuMainForm(this.tboxpidno.Text.Trim());
			string finalStr = string.Empty;
			if (m_cession != "")
			{
				List<string> lstStr = new List<string>();
				foreach (string str in m_cession.Split('|'))
				{
					if (str != "")
					{
						string[] strSon = str.Split(':');
						string time1 = string.Empty;
						string temp = string.Empty;
						if (strSon[1].Split(';').Length > 2)
						{
							string[] strTime = strSon[1].Split(';');
							for (int i = 0; i < strTime.Length; i++)
							{
								if (temp == "")
								{
									temp = strTime[i];
									time1 += strTime[i].Split('-')[0] + "时";
								}
								else
								{
									if (temp != strTime[i])
									{
										if (strTime[i] != "")
										{
											time1 += strTime[i].Split('-')[0] + "时";
										}
									}
									temp = strTime[i];
								}
							}
						}
						else
						{
							string[] strTime = strSon[1].Split(';');
							for (int i = 0; i < strTime.Length; i++)
							{
								if (strTime[i] != "")
								{
									time1 += strTime[i].Split('-')[0] + "时";
								}
							}
						}
						lstStr.Add(Convert.ToDateTime(strSon[0]).ToString("yyyy年MM月dd日") + time1);
					}
				}
				foreach (string temp in lstStr)
				{
					finalStr += temp + ";";
				}
			}
		//	if (cboxBillModeBook.SelectedValue.ToString() == "按时间计费")
			//	tBlockBookStatus.Text = string.Format("学员:" + this.tblockName.Text + "    完成一次预约，预约时间为:" + finalStr + "预约总时长:" + traAmount + "小时,车型:" + this.tboxVehicleModel.Text.Trim() + ",车号:" + TraBookInfo.TraCarNo + "");
		//	else if (cboxBillModeBook.SelectedValue.ToString() == "按次数计费")
				//tBlockBookStatus.Text = string.Format("学员:" + this.tblockName.Text + "    完成一次预约，预约次数为:" + TraBookInfo.TriesAmount + "次,车型:" + this.tboxVehicleModel.Text.Trim() + ",车号:" + TraBookInfo.TraCarNo + "");
		//	else if (cboxBillModeBook.SelectedValue.ToString() == "按里程计费")
			//	tBlockBookStatus.Text = string.Format("学员:" + this.tblockName.Text + "    完成一次预约，预约里程为:" + TraBookInfo.TraAmount + "公里,车型:" + this.tboxVehicleModel.Text.Trim() + ",车号:" + TraBookInfo.TraCarNo + "");

		}	
		private void tboxTraMil_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			DisplayPayAmount();
		}
		private void tboxTraCnt_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			DisplayPayAmount();
		}
		public void DisplayPayAmount()
		{
			if (dsrsrc.trainMangeDataSet.TraineeDataTable.Rows.Count == 0)
				return;
			double dChargingStandard = dsrsrc.GetChargingStandard(dsrsrc.sBillingMode, dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["LICENSE_TYPE_CD"].ToString());

			if (sChargingMode == "Tries")
			{
				if (tboxTraCnt.Text.Trim() == "")
				{
					this.tboxPayamount.Text = "";
					return;
				}
				this.tboxPayamount.Text = (double.Parse(tboxTraCnt.Text.Trim()) * dChargingStandard).ToString();
			}
			else if (sChargingMode == "Mileage")
			{
				if (tboxTraMil.Text.Trim() == "")
				{
					this.tboxPayamount.Text = "";
					return;
				}
				this.tboxPayamount.Text = (double.Parse(tboxTraMil.Text.Trim()) * dChargingStandard).ToString();
			}
			else if (sChargingMode == "Time")
			{
				if (tboxTraTime.Text.Trim() == "")
				{
					this.tboxPayamount.Text = "";
					return;
				}
				this.tboxPayamount.Text = (double.Parse(tboxTraTime.Text.Trim()) * dChargingStandard).ToString();
			}
			tboxTraTime.Text = dTimeAmount.ToString();
		}
		private void btnManualSearch_Click(object sender, RoutedEventArgs e)
		{
			if (this.tboxpidno.Text.ToString() == "")
			{
				System.Windows.MessageBox.Show("身份证号码不能为空,请重新填写");
				return;
			}
		 PopuMainForm(this.tboxpidno.Text.ToString().Trim());

		}
		private void tboxTraMil_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			TextAllowInputIntAndDecimal(sender, e);
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
		private void tboxTraCnt_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			TextAllowInputInt(sender, e);
		}
		private void lboxBooktype_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			popTrabook(true);
			tboxPayamount.Text = "";
		}
		private void popTrabook(bool isclear)
		{
			if (tboxBillModeBook.Text == "按时间计费")
			{

				if (lboxBooktype.SelectedItem != null)
				{
					string[] lboxItem = lboxBooktype.SelectedItem.ToString().Split('-');
					//显示历史预约记录            
					dsrsrc.GetTraBook("", "", DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")), DateTime.Parse(DateTime.Now.AddDays(8).ToString("yyyy-MM-dd")));
					//根据预约记录刷新界面  
					if (dsrsrc.trainMangeDataSet.BookTrainingDataTable.Select("AUTO_ID='" + lboxItem[0].Trim().ToString() + "'").Length > 0)
					{
						try
						{
							//清除上次选择
							UIElementCollection Childrens = canvas1.Children;
							foreach (UIElement ui in Childrens)
							{
								var cb = (System.Windows.Controls.CheckBox)ui;
								if (isclear)
								{
									if (cb.IsChecked == true)
									{
										isAdd = false;
										cb.IsChecked = false;
										isAdd = true;
										cb.IsEnabled = true;
										cb.Opacity = 1;
									}
								}
								else
								{
									if (cb.IsChecked == true && cb.Opacity == 1)
									{
										isAdd = false;
										cb.IsChecked = false;
										isAdd = true;
										cb.IsEnabled = true;
										cb.Opacity = 1;
									}
								}
							}
						}
						catch (Exception EX)
						{
							throw EX;
						}

						foreach (DataRow dr in dsrsrc.trainMangeDataSet.BookTrainingDataTable.Select("AUTO_ID='" + lboxItem[0].Trim().ToString() + "'"))
						{
							string[] booksession = dr["TRAINSESSION"].ToString().Split('|');
							for (int i = 0; i < booksession.Length - 1; i++)
							{
								string[] bookdetial = booksession[i].Split(':');
								string bookdate = bookdetial[0];
								string[] booktime = bookdetial[1].Split(';');
								if (bookdate == DateTime.Now.ToString("yyyy-MM-dd"))
								{
									updCbox(1, booktime, isclear);
								}
								else if (bookdate == DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"))
								{
									updCbox(2, booktime, isclear);
								}
								else if (bookdate == DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"))
								{
									updCbox(3, booktime, isclear);
								}
								else if (bookdate == DateTime.Now.AddDays(3).ToString("yyyy-MM-dd"))
								{
									updCbox(4, booktime, isclear);
								}
								else if (bookdate == DateTime.Now.AddDays(4).ToString("yyyy-MM-dd"))
								{
									updCbox(5, booktime, isclear);
								}
								else if (bookdate == DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"))
								{
									updCbox(6, booktime, isclear);
								}
								else
								{
									updCbox(7, booktime, isclear);
								}
							}
						}
					}
					else
					{
						//清除上次选择
						UIElementCollection Childrens = canvas1.Children;
						foreach (UIElement ui in Childrens)
						{
							var cb = (System.Windows.Controls.CheckBox)ui;
							if (cb.IsChecked == true)
							{
								isAdd = false;
								cb.IsChecked = false;
								isAdd = true;
								cb.IsEnabled = true;
								cb.Opacity = 1;
							}
						}
					}
				}
			}
		}
		private void updCbox(int rownum, string[] time, bool isclear)
		{
			isAdd = false;
			string schkBoxName = "";
			foreach (var temp in time)
			{
				if (temp != "")
				{
					string[] str = temp.Split('-');

					if (str[1] == "0")
					{
						schkBoxName = "CheckBox" + rownum + ((int.Parse(str[0]) - nStratWT) * 2 + 1).ToString();
					}
					else
					{
						schkBoxName = "CheckBox" + rownum + ((int.Parse(str[0]) - nStratWT) * 2 + 2).ToString();
					}
					CheckBox cbx = canvas1.FindName(schkBoxName) as CheckBox;
					cbx.IsEnabled = false;
					cbx.IsChecked = true;
					cbx.Opacity = 0.6;
				}
			}
			isAdd = false;
		}
		private void cboxBookBrand_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				ltcar.Clear();
				if (dsrsrc.trainMangeDataSet.TraCarDataTable.Rows.Count > 0)
				{
					lboxBooktype.ItemsSource = null;
					DataRow[] trainerDb = null;
					if (cboxBookBrand.SelectedItem == null)
					{
						trainerDb = dsrsrc.trainMangeDataSet.TraCarDataTable.Select("AUTO_TYPE_CD='" + tboxVehicleModel.Text.ToString() + "' ");
					}
					else
					{
						trainerDb = dsrsrc.trainMangeDataSet.TraCarDataTable.Select("AUTO_TYPE_CD='" + tboxVehicleModel.Text.ToString() + "' and BRAND='" + (cboxBookBrand.SelectedItem as System.Data.DataRowView).Row["BRAND"].ToString() + "'");
					}
					if (trainerDb.Length > 0)
					{
						foreach (DataRow dr in trainerDb)
						{
							var drTrainer = dsrsrc.trainMangeDataSet.TrainerDataTable.Select("TRAINER_ID = '" + dr["TRAINER_ID"] + "'");
							if (drTrainer.Length > 0)
							{
								ltcar.Add(dr["AUTO_ID"].ToString() + " - " + dr["PLATE_NO"].ToString() + " - " + drTrainer[0]["TRAINER_NAME"].ToString());
							}
							else
							{
								ltcar.Add(dr["AUTO_ID"].ToString() + " - " + dr["PLATE_NO"].ToString() + " - ");
							}
						}
						lboxBooktype.ItemsSource = ltcar;
						isAdd = false;
						lboxBooktype.SelectedIndex = 0;
						isAdd = true;
					}

				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("刷新界面出错，错误信息：" + ex.Message.ToString());
				return;
			}

		}
		private void btnSign_Click(object sender, RoutedEventArgs e)
		{
			if (dataGridHistory.SelectedItems.Count == 1)
			{
				var tempA = (DataRowView)dataGridHistory.SelectedItems[0];
				if (tempA["CHECKSTATUS"].ToString() == "未签到")
				{
					bool bPrint = true;
					//打印小票
					//   bPrint = print.PrintDirect("OrderDocument.xaml", tempA);//直接打印
					//bPrint = print.PrintDlg("OrderDocument.xaml", tempA);//选择打印机打印
					if (bPrint)
					{
						try
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
								WriteTrainLicense(tempA["PID_NO"].ToString(), int.Parse(tempA["SEQ_NO"].ToString()));
								//PopuMainForm(tempA["PID_NO"].ToString());
							}
							else
							{
								MessageBox.Show("没有可用的移动设备。");
								return;

							}
						}
						catch (Exception ex)
						{
							MessageBox.Show("写入许可失败，请重试,原因：" + ex.Message.ToString());
							return;
						}
					}

				}
				else if (tempA["CHECKSTATUS"].ToString() == "已签到")
				{
					System.Windows.MessageBox.Show("本次预约已签到，不能进行签到");
					return;
				}
				else if (tempA["CHECKSTATUS"].ToString() == "已结算")
				{
					System.Windows.MessageBox.Show("本次预约已经结算，不能进行签到");
					return;
				}
				else if (tempA["CHECKSTATUS"].ToString() == "训练中")
				{
					System.Windows.MessageBox.Show("本次预约正在训练中，不能进行签到");
					return;
				}
				else if (tempA["CHECKSTATUS"].ToString() == "已结束")
				{
					System.Windows.MessageBox.Show("本次预约已完成训练，不能进行签到");
					return;
				}
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("未选中任何行或者选中了多行，请重新选择！");
			}
		}
		private void btnBookCancel_Click(object sender, RoutedEventArgs e)
		{
			if (dataGridHistory.SelectedItems.Count == 1)
			{
				var tempA = (DataRowView)dataGridHistory.SelectedItems[0];
				if (tempA["CHECKSTATUS"].ToString() == "未签到")
				{
					if (System.Windows.MessageBox.Show("是否取消该预约？", "取消预约", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						double dSeqNo = Convert.ToDouble(tempA["SEQ_NO"].ToString());
						dsrsrc.GetTraBookBySeqNo(dSeqNo);
						double dBalanceCancel = 0;
						dBalanceCancel = Convert.ToDouble(dsrsrc.trainMangeDataSet.BookTrainingDataTable.Rows[0]["THIS_BALANCE"]);
						dsrsrc.TraBookCancel(tempA["SEQ_NO"].ToString());
						dsrsrc.RechargeAccount("", dBalanceCancel, tempA["PID_NO"].ToString(), "", "取消预约返还");
						//PopuMainForm(tempA["PID_NO"].ToString());
					//	tBlockBookStatus.Text = string.Format("学员:" + tempA["TRAINE_NAME"] + "    取消预约！");
					}
				}
				else if (tempA["CHECKSTATUS"].ToString() == "已签到")
				{
					System.Windows.MessageBox.Show("本次预约已经签到，不能取消预约！");
					return;
				}
				else if (tempA["CHECKSTATUS"].ToString() == "已结算")
				{
					System.Windows.MessageBox.Show("本次预约已经结算，不能取消预约！");
					return;
				}
				else if (tempA["CHECKSTATUS"].ToString() == "训练中")
				{
					System.Windows.MessageBox.Show("本次预约正在训练中，不能取消预约！");
					return;
				}
				else if (tempA["CHECKSTATUS"].ToString() == "已结束")
				{
					System.Windows.MessageBox.Show("本次预约已完成训练，不能取消预约！");
					return;
				}
				else if (tempA["CHECKSTATUS"].ToString() == "已取消")
				{
					System.Windows.MessageBox.Show("本次预约已取消！");
					return;
				}
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("未选中任何行或者选中了多行，请重新选择！");
			}
		}
		/// <summary>
		/// 写入许可的方法
		/// </summary>
		/// <param name="sPidNo"></param>
		/// <param name="iSeqNo"></param>
		public void WriteTrainLicense(string sPidNo, int iSeqNo)
		{
			List<TrainLicense> m_listTraLic = new List<TrainLicense>();
			Dictionary<string, string> listBookSeq = new Dictionary<string, string>();
			listBookSeq.Add(sPidNo, iSeqNo.ToString());
			string sJsonTraLic = "";
			dsrsrc.trainMangeDataSet.TraTrainLicenseDataTable.Clear();
			DBAccessHelper.getTrainLicense(iSeqNo, sPidNo, dsrsrc.trainMangeDataSet);
			TrainLicense traLic = null;//= new TrainLicense();
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
			sJsonTraLic = JsonConvert.SerializeObject(m_listTraLic);
			WriteLicenseWindow writeLicWin = new WriteLicenseWindow(sJsonTraLic, listBookSeq);
			writeLicWin.ShowDialog();
		}
		//初始化界面
		private void InitialControl()
		{			
			try
			{
				if (!Directory.Exists(DBAccessProc.Common.TempFilePath))
					Directory.CreateDirectory(DBAccessProc.Common.TempFilePath);
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show("创建路径出错,错误信息：" + ex.Message);
			}
			////预约界面
			if (sChargingMode == "Time")
				this.tboxBillModeBook.Text = "按时间计费";
			else if (sChargingMode == "Tries")
				this.tboxBillModeBook.Text = "按次数计费";
			else
				this.tboxBillModeBook.Text = "按里程计费";
			//预约选择框  
			this.label81.Content = DateTime.Now.ToString("MM-dd-dddd");
			this.label82.Content = DateTime.Now.AddDays(1).ToString("MM-dd-dddd");
			this.label83.Content = DateTime.Now.AddDays(2).ToString("MM-dd-dddd");
			this.label84.Content = DateTime.Now.AddDays(3).ToString("MM-dd-dddd");
			this.label85.Content = DateTime.Now.AddDays(4).ToString("MM-dd-dddd");
			this.label86.Content = DateTime.Now.AddDays(5).ToString("MM-dd-dddd");
			this.label87.Content = DateTime.Now.AddDays(6).ToString("MM-dd-dddd");
			if (workLong >0)
			{
				int x = workLong*2;
				CreateLabel(x, 1, nStratWT);
				CreateCheckBox(x, 7);
			}			
		}
		private void CreateLabel(int x, int y, int n)
		{
			canvas2.Children.Clear();
			//四个方向的边距都是m
			int m = 1;
			if (x <= 30)
				m = 1;
			else if (x > 30)
				m = 0;
			double width = (this.canvas2.Width - (x + 1) * m) / x;
			double height = (this.canvas2.Height - (y + 1) * m) / y;

			int idx = 0;
			for (int i = 0; i < x; i++)
			{
				for (int j = 0; j < y; j++)
				{
					Label lbl = new Label() {
						Width = width,
						Height = height
					};

					lbl.FontSize = 12;
					if (idx < 1)
					{
						if (n < 10)
							lbl.Content = "0" + n;
						else if (n > 24)
						{
							if ((n - 24) < 10)
								lbl.Content = "0" + (n - 24);
							else
								lbl.Content = (n - 24);
						}
						else
							lbl.Content = n;
						idx++;
					}
					else
					{
						if (n < 10)
							lbl.Content = "0" + n;
						else if (n > 24)
						{
							if ((n - 24) < 10)
								lbl.Content = "0" + (n - 24);
							else
								lbl.Content = (n - 24);
						}
						else
							lbl.Content = n;
						idx = 0;
						n++;
					}

					Canvas.SetTop(lbl, j * height + 3);
					Canvas.SetLeft(lbl, i * width + 5);
					//这两句很关键。按钮在Canvas中的定位与它自己的Left以及Top不是一个概念
					canvas2.Children.Add(lbl);
				}
			}
		}
		private void CreateCheckBox(int x, int y)
		{
			canvas1.Children.Clear();
			int m = 1;
			if (x <= 30)
				m = 1;
			else if (x > 30)
				m = 0;
			//四个方向的边距都是m
			double width = (this.canvas1.Width - (x + 1) * m) / x;
			double height = (this.canvas1.Height - (y + 1) * m) / y;

			for (int i = 0; i < x; i++)
			{
				for (int j = 0; j < y; j++)
				{
					CheckBox chkBox = new CheckBox() {
						Width = width,
						Height = height
					};
					chkBox.Name = "CheckBox" + (j + 1).ToString() + (i + 1).ToString();
					//canvas1.UnregisterName(chkBox.Name);

					canvas1.RegisterName(chkBox.Name, chkBox);//注册名字

					Canvas.SetTop(chkBox, j * height + 3);
					Canvas.SetLeft(chkBox, i * width + 5);
					chkBox.Checked += checkBox1_Checked;
					chkBox.Unchecked += checkBox1_Unchecked;
					//这两句很关键。按钮在Canvas中的定位与它自己的Left以及Top不是一个概念
					canvas1.Children.Add(chkBox);
				}
			}
		}
		private void checkBox1_Checked(object sender, RoutedEventArgs e)
		{
			if (isAdd)
			{
				if (dsrsrc.trainMangeDataSet.TraineeDataTable.Rows.Count == 0)
					return;
				double dChargingStandard = dsrsrc.GetChargingStandard(dsrsrc.sBillingMode,dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["LICENSE_TYPE_CD"].ToString());
				dTimeAmount += 0.5;
				if (sChargingMode == "Time")
				{
					this.tboxPayamount.Text = (dTimeAmount * dChargingStandard).ToString();
				}
				tboxTraTime.Text = dTimeAmount.ToString();
			}
		}

		private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
		{
			if (isAdd)
			{
				if (dsrsrc.trainMangeDataSet.TraineeDataTable.Rows.Count == 0)
					return;
				double dChargingStandard = dsrsrc.GetChargingStandard(dsrsrc.sBillingMode,dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["LICENSE_TYPE_CD"].ToString());
				dTimeAmount -= 0.5;
				if (sChargingMode == "Time")
				{
					this.tboxPayamount.Text = (dTimeAmount * dChargingStandard).ToString();
					tboxTraTime.Text = dTimeAmount.ToString();
				}
			}
		}
		//查询学员信息&&刷新主界面
		private void PopuMainForm(string sPidNo)
		{
			dsrsrc.getAllTraCar();
			try
			{				
				dsrsrc.getTraStudinfo(sPidNo);
				if (dsrsrc.trainMangeDataSet.TraineeDataTable.Rows.Count > 0)
				{
					dTimeAmount = 0;
					isAdd = false;
					lboxBooktype.SelectedIndex = 0;
					popTrabook(true);

					isAdd = true;
					tboxTraCnt.Text = "";
					tboxTraMil.Text = "";
					tboxTraTime.Text = "";
					tboxpidno.Text = dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["PID_NO"].ToString();
					tblockName.Text = dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["TRAINE_NAME"].ToString();
					tboxVehicleModel.Text = dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["LICENSE_TYPE_CD"].ToString();
					tboxBanlance.Text = dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["BALANCE"].ToString();
					DisplayPayAmount();
					//车辆品牌显示
					if (tboxVehicleModel.Text == "")
					{
						MessageBox.Show("学员车型信息不能为空。");
						return;
					}
					//车辆教练列表				
					if (dsrsrc.trainMangeDataSet.TraCarDataTable.Rows.Count > 0)
					{
						this.cboxBookBrand.DisplayMemberPath = "BRAND";
						DataTable dt2 = dsrsrc.trainMangeDataSet.TraCarDataTable.Clone();
						DataRow[] dr2 = dsrsrc.trainMangeDataSet.TraCarDataTable.Select("AUTO_TYPE_CD='" + this.tboxVehicleModel.Text.ToString().Trim() + "'");
						foreach (DataRow ss in dr2)
						{
							dt2.Rows.Add(ss.ItemArray);
						}
						dt2 = dt2.DefaultView.ToTable(true, "BRAND");
						this.cboxBookBrand.ItemsSource = dt2.DefaultView;
						this.cboxBookBrand.SelectedIndex = 0;
					}
					//历史记录显示  
					var bookHistory = dsrsrc.displayBookhistory(sPidNo, "", DateTime.Parse(Convert.ToDateTime("1900-01-01").ToString("yyyy-MM-dd")), DateTime.Parse(Convert.ToDateTime("1900-01-01").ToString("yyyy-MM-dd")));
					if (bookHistory != null)
						dataGridHistory.ItemsSource = bookHistory.DefaultView;
					else
						dataGridHistory.ItemsSource = null;
				}
				else
				{
					if (sPidNo != "")
					{
					//	tBlockBookStatus.Text = string.Format("没有学员的注册信息,可以进行注册。");
					}
				}

			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show("刷新主界面出错,错误信息：" + ex.Message);
				return;
			}
		}

		private void tboxBillModeBook_SelectionChanged(object sender, TextChangedEventArgs e)
		{
			if (tboxBillModeBook.Text == "按时间计费")
			{				
				tboxTraCnt.IsEnabled = false;
				tboxTraMil.IsEnabled = false;
				tboxTraCnt.Text = "";
				tboxTraMil.Text = "";
				tboxTraTime.IsEnabled = false;
				tboxTraTime.Text = "";
				//清除上次选择
				UIElementCollection Childrens = canvas1.Children;
				foreach (UIElement ui in Childrens)
				{
					if (ui.GetType().Name == "CheckBox")
					{
						var cb = (System.Windows.Controls.CheckBox)ui;
						cb.IsEnabled = true;
						cb.IsChecked = false;
						cb.Opacity = 1.0;
					}
				}
			}
			else
			{
				UIElementCollection Childrens = canvas1.Children;
				foreach (UIElement ui in Childrens)
				{

					if (ui.GetType().Name == "CheckBox")
					{
						var cb = (System.Windows.Controls.CheckBox)ui;
						cb.IsEnabled = false;
						cb.IsChecked = false;
						cb.Opacity = 0.7;
					}
				}
				if (tboxBillModeBook.Text == "按次数计费")
				{	
					tboxTraCnt.IsEnabled = true;
					tboxTraMil.IsEnabled = false;
					tboxTraCnt.Text = "";
					tboxTraMil.Text = "";			
					tboxTraCnt.Focus();
					tboxTraTime.Text = "";
					tboxTraTime.IsEnabled = false;
				}
				else if (tboxBillModeBook.Text == "按里程计费")
				{				
					tboxTraCnt.IsEnabled = false;
					tboxTraMil.IsEnabled = true;
					tboxTraCnt.Text = "";
					tboxTraMil.Text = "";		
					tboxTraMil.Focus();
					tboxTraTime.Text = "";
					tboxTraTime.IsEnabled = false;

				}
			}
		}

	}
}
