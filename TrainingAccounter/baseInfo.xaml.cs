using System;
using System.Collections.Generic;
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
	/// baseInfo.xaml 的交互逻辑
	/// </summary>
	public partial class baseInfo : Page
	{
		public baseInfo(DsRsrc dsr)
		{
			InitializeComponent();
			dsrsrc = dsr;
			PopuTraManagerPage();
		}
		DsRsrc dsrsrc;
		string sChargingMode = "Time";
		int nStratWT;
		private void btnSaveSetting_Click(object sender, RoutedEventArgs e)
		{
			if (this.tboxMinTimeUnit.Text.ToString().Trim() == "" || tboxFmTime.Text.Trim() == "" || comBoxStartTime.SelectedValue == null || cboxChargeModes.SelectedValue == null)
			{
				System.Windows.Forms.MessageBox.Show("请检查是否有信息为空.");
				return;
			}
			else
			{
				string m_sChargeMode = "";
				if (cboxChargeModes.Text == "按次数计费")
					m_sChargeMode = "Tries";
				else if (cboxChargeModes.Text == "按时间计费")
					m_sChargeMode = "Time";
				else
					m_sChargeMode = "Mileage";
				dsrsrc.TraSettingValues.Clear();
				dsrsrc.TraSettingValues.Add("ChargingMode", m_sChargeMode + "|默认计费模式|");
				dsrsrc.TraSettingValues.Add("MinTimeUnit", this.tboxMinTimeUnit.Text + "|最小计时单位（分钟）|");
				dsrsrc.TraSettingValues.Add("Duration", this.tboxFmTime.Text + "|工作总时长（小时）|");
				dsrsrc.TraSettingValues.Add("StartTime", this.comBoxStartTime.SelectedValue.ToString() + "|工作开始时间|");
				//获取选中的车型
				string Cartype = string.Empty;
				if (this.ckboxC1.IsChecked == true)
					Cartype += "C1,";
				if (this.ckboxC2.IsChecked == true)
					Cartype += "C2,";
				if (this.ckboxC5.IsChecked == true)
					Cartype += "C5,";
				if (this.ckboxB1.IsChecked == true)
					Cartype += "B1,";
				if (this.ckboxB2.IsChecked == true)
					Cartype += "B2,";
				if (this.ckboxA1.IsChecked == true)
					Cartype += "A1,";
				if (this.ckboxA2.IsChecked == true)
					Cartype += "A2,";
				if (this.ckboxA3.IsChecked == true)
					Cartype += "A3,";
				if (Cartype.Length > 0)
					Cartype = Cartype.Substring(0, Cartype.Length - 1);
				dsrsrc.TraSettingValues.Add("CarType", Cartype + "|车型|");
				dsrsrc.AddTraSetting();
			}
			PopuTraManagerPage();
		}
		private void btnSaveChargingMode_Click(object sender, RoutedEventArgs e)
		{
			//获取选中的车型
			string Cartype = CboxCarTypes.SelectedItem.ToString();
			if (Cartype == "")
			{
				System.Windows.MessageBox.Show("请选择训练车型！");
				return;
			}
			if (tboxChargeByTime.Text.Trim() == "" || tboxChargeByTries.Text.Trim() == "" || tboxChargeByMileage.Text.Trim() == "")
			{
				System.Windows.MessageBox.Show("请填入计费标准！");
				return;
			}
			if (txtBoxTimes.Text.Trim() == "")
			{
				System.Windows.MessageBox.Show("请填入单次训练限制时间！");
				return;
			}
			double txtBoxVip = 0;
			double txtBoxVipPreferential = 0;
			if (double.TryParse(this.txtBoxVip.Text, out txtBoxVip))
			{
				if (txtBoxVip < 200)
				{
					MessageBox.Show("VIP默认金额不能为低于200元，请重新填写");
					return;
				}
			}
			else
			{
				MessageBox.Show("输入的数字格式不合法，请重新填写");
				return;
			}
			if (double.TryParse(this.txtBoxVipPreferential.Text, out txtBoxVipPreferential))
			{
				if (txtBoxVipPreferential > 1 || txtBoxVipPreferential < 0)
				{
					MessageBox.Show("VIP优惠比例要在0~1之间，请重新填写");
					return;
				}
			}
			else
			{
				MessageBox.Show("输入的数字格式不合法，请重新填写");
				return;
			}
			dsrsrc.TraSettingValues.Clear();
			string sChargSt = "";
			sChargSt = "Time:" + tboxChargeByTime.Text.Trim() + "*" + "Tries:" + tboxChargeByTries.Text.Trim() + "*" + "Mileage:" + tboxChargeByMileage.Text.Trim();
			dsrsrc.TraSettingValues.Add("ChargingStandard" + Cartype, sChargSt + "|" + Cartype + "计费标准|");
			dsrsrc.TraSettingValues.Add("SingleTime" + Cartype, this.txtBoxTimes.Text.Trim() + "|" + Cartype + "单次训练限制时间（分钟）|");
			dsrsrc.TraSettingValues.Add("VipStandard" + Cartype, this.txtBoxVip.Text.Trim() + "|" + Cartype + "自动转换VIP充值标准|");
			dsrsrc.TraSettingValues.Add("VipPreferential" + Cartype, this.txtBoxVipPreferential.Text.Trim() + "|" + Cartype + "VIP充值优惠比例|");
			dsrsrc.AddTraSetting();
		}
		private void tboxChargeByTime_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			TextAllowInputIntAndDecimal(sender, e);
		}
		private void tboxChargeByTries_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			TextAllowInputIntAndDecimal(sender, e);
		}
		private void tboxChargeByMileage_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			TextAllowInputIntAndDecimal(sender, e);
		}
		private void txtBoxTimes_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			TextAllowInputInt(sender, e);
		}
		private void CboxCarTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			tboxChargeByTime.Text = "";
			tboxChargeByTries.Text = "";
			tboxChargeByMileage.Text = "";
			txtBoxTimes.Text = "";
			for (int count = 0; count < dsrsrc.trainMangeDataSet.Tra_Setting.Rows.Count; count++)
			{
				string sRsTxt = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_TXT"].ToString();
				string sRsValue = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_VALUE"].ToString();
				string sCarType = "";
				if (sRsTxt.Contains("ChargingStandard"))
				{
					sCarType = sRsTxt.Substring(16, 2);
					if (sCarType == CboxCarTypes.SelectedItem.ToString())
					{
						tboxChargeByTime.Text = dsrsrc.GetChargingStandard(sChargingMode,sCarType).ToString();
						tboxChargeByTries.Text = dsrsrc.GetChargingStandard(sChargingMode, sCarType).ToString();
						tboxChargeByMileage.Text = dsrsrc.GetChargingStandard(sChargingMode,sCarType).ToString();
					}
				}
				else if (sRsTxt.Contains("SingleTime"))
				{
					sCarType = sRsTxt.Substring(10, 2);
					if (sCarType == CboxCarTypes.SelectedItem.ToString())
					{
						txtBoxTimes.Text = sRsValue;
					}
				}
			}
		}
		/// <summary>
		/// 刷新界面的方法
		/// </summary>
		/// 
		private void PopuTraManagerPage()
		{	
			dsrsrc.GetAllSetting();		
			this.CboxCarTypes.ItemsSource = dsrsrc.TrainLicenseType;
			this.CboxCarTypes.SelectedIndex = 0;
			this.comBoxStartTime.ItemsSource = dsrsrc.WorkTime;
			comBoxStartTime.SelectedIndex = 0;
			cboxChargeModes.ItemsSource = dsrsrc.BillingMode;	
			//信息设置部分
			//dataGridSetting.ItemsSource = dsrsrc.trainMangeDataSet.Tra_Setting.DefaultView;
			if (dsrsrc.trainMangeDataSet.Tra_Setting.Rows.Count > 0)
			{
				for (int count = 0; count < dsrsrc.trainMangeDataSet.Tra_Setting.Rows.Count; count++)
				{
					string sRsTxt = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_TXT"].ToString();
					string sRsValue = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_VALUE"].ToString();
					string sFlag = "";
					string sCarType = "";
					if (sRsTxt.Contains("VipStandard"))
					{
						sFlag = sRsTxt.Substring(0, 11);
						sCarType = sRsTxt.Substring(11, 2);
					}
					else if (sRsTxt.Contains("VipPreferential"))
					{
						sFlag = sRsTxt.Substring(0, 15);
						sCarType = sRsTxt.Substring(15, 2);
					}
					else if (sRsTxt.Contains("ChargingStandard"))
					{
						sFlag = sRsTxt.Substring(0, 16);
						sCarType = sRsTxt.Substring(16, 2);
					}
					else if (sRsTxt.Contains("SingleTime"))
					{
						sFlag = sRsTxt.Substring(0, 10);
						sCarType = sRsTxt.Substring(10, 2);
					}
					else
						sFlag = sRsTxt;
					switch (sFlag)
					{
						case "ChargingMode":
							sChargingMode = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_VALUE"].ToString();
							if (sChargingMode == "Tries")
							{
								cboxChargeModes.SelectedIndex = 0;							
							}
							else if (sChargingMode == "Time")
							{
								cboxChargeModes.SelectedIndex = 1;							
							}
							else
							{
								cboxChargeModes.SelectedIndex = 2;							
							}
							break;
						case "VipStandard":
							txtBoxVip.Text = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_VALUE"].ToString();
							break;
						case "VipPreferential":
							txtBoxVipPreferential.Text = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_VALUE"].ToString();
							break;
						case "MinTimeUnit":
							this.tboxMinTimeUnit.Text = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_VALUE"].ToString();
							break;
						case "Duration":
							this.tboxFmTime.Text = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_VALUE"].ToString();
							break;
						case "StartTime":
							this.comBoxStartTime.SelectedValue = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_VALUE"].ToString();
							break;
						case "CarType":
							string[] CarType = dsrsrc.trainMangeDataSet.Tra_Setting.Rows[count]["RS_VALUE"].ToString().Split(',');
							for (int e = 0; e < CarType.Length; e++)
							{
								switch (CarType[e].ToString())
								{
									case "C1":
										this.ckboxC1.IsChecked = true;
										break;
									case "C2":
										this.ckboxC2.IsChecked = true;
										break;
									case "C5":
										this.ckboxC5.IsChecked = true;
										break;
									case "B1":
										this.ckboxB1.IsChecked = true;
										break;
									case "B2":
										this.ckboxB2.IsChecked = true;
										break;
									case "A1":
										this.ckboxA1.IsChecked = true;
										break;
									case "A2":
										this.ckboxA1.IsChecked = true;
										break;
									case "A3":
										this.ckboxA1.IsChecked = true;
										break;
									default:
										break;
								}
							}
							break;
						default:
							break;
					}
				}
				if (this.tboxFmTime.Text != "" && this.comBoxStartTime.SelectedValue.ToString() != "")
				{
					int x = int.Parse(tboxFmTime.Text) * 2;
					nStratWT = int.Parse(comBoxStartTime.SelectedValue.ToString().Split(':')[0]);
				}
				else
					MessageBox.Show("请设置工作时长、工作开始时间！");
			}
			else
			{
				MessageBox.Show("没有查找到相关的基本配置信息,请启动后重新配置并重启程序");
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

		private void tboxMinTimeUnit_KeyDown(object sender, KeyEventArgs e)
		{
			TextAllowInputInt(sender, e);
		}

		private void tboxFmTime_KeyDown(object sender, KeyEventArgs e)
		{
			TextAllowInputInt(sender, e);
		}
	}
}
