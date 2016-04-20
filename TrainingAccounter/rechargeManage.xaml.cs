using System;
using System.Collections.Generic;
using System.Data;
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
	/// rechargeManage.xaml 的交互逻辑
	/// </summary>
	public partial class rechargeManage : Page
	{
		public rechargeManage(DsRsrc dsr)
		{
			InitializeComponent();
			dsrsrc = dsr;
			dsrsrc.getAllDrvSchool();
			cbxDevName.ItemsSource=dsrsrc.trainMangeDataSet.DrvSchoolDataTable.DefaultView;
			tbxSerPidNo.SetBinding(TextBox.TextProperty, new Binding("PidNo") { Source = dsrsrc });
		}
		DsRsrc dsrsrc;
		DateTime m_StarTime;
		DateTime m_EndTime;
		//充值事件
		private void btnDeposit_Click(object sender, RoutedEventArgs e)
		{
			Recharge reg = new Recharge(dsrsrc);
            reg._sChargingMode = dsrsrc.sBillingMode;
			reg.ChangeTextEvent += new ChangeTextHandler(reg_ChangeTextEvent);			   
			if (dataGridFinace.SelectedItems.Count > 0)
			{
				var temp = (DataRowView)dataGridFinace.SelectedItems[0];
				reg.tboxSeqNo.Text = temp["SEQ_NO"].ToString();
				reg.txbUserName.Text = temp["NAME"].ToString();
				reg.txbPidNo.Text = temp["PID_NO"].ToString();
				reg.tboxBlce.Text = temp["BALANCE"].ToString();
				dsrsrc.getTraStudinfoBySeqNo(double.Parse(temp["SEQ_NO"].ToString()));
				if (dsrsrc.trainMangeDataSet.TraineeDataTable.Rows.Count > 0)
				{
					reg._sCarType = dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["LICENSE_TYPE_CD"].ToString();
					int stutype = 0;
					if (int.TryParse(dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["STUDENT_TYPE"].ToString(), out stutype))
					{
						if (stutype == 1)
							reg.rdbVip.IsChecked = true;
						else
							reg.rdbCustom.IsChecked = true;
					}
					else
						reg.rdbCustom.IsChecked = true;
				}
				else
					MessageBox.Show("充值失败，找不到此学员信息。");
			}			
			reg.ShowDialog();
		}
		//查看充值记录事件
		private void btnDepositSer_Click(object sender, RoutedEventArgs e)
		{
			if (dataGridFinace.SelectedItems.Count > 0)
			{
				var temp = (DataRowView)dataGridFinace.SelectedItems[0];
				serRecharge sreg = new serRecharge(temp["PID_NO"].ToString(), datePickerSerStr.SelectedDate.Value, datePickerSerEnd.SelectedDate.Value);
				sreg.ShowDialog();
			}
		}
		private void btnSearch_Click(object sender, RoutedEventArgs e)
		{
			getSerRecord(tbxSerPidNo.Text.Trim(),"", cbxDevName.SelectedValue != null ? int.Parse(cbxDevName.SelectedValue.ToString()) : -1, m_StarTime, m_EndTime);			
			
		}
		private void dataGridFinace_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (dataGridFinace.SelectedItems.Count == 1)
			{
				var temp = (DataRowView)dataGridFinace.SelectedItems[0];
				tbxSerPidNo.Text = temp["PID_NO"].ToString();				
			}
		}
		void reg_ChangeTextEvent(string text)
		{
			getSerRecord(tbxSerPidNo.Text.Trim(), "", cbxDevName.SelectedValue != null ? int.Parse(cbxDevName.SelectedValue.ToString()) : -1, m_StarTime, m_EndTime);		
		}
		private void getSerRecord(string pidNo, string serName, int drvno, DateTime timeStr, DateTime timeEnd)
		{  //查询金额		
			dsrsrc.trainMangeDataSet.TraFinanceDataTable.Clear();
			DBAccessProc.DBAccessHelper.getTraFinance(pidNo, serName, drvno, timeStr, timeEnd, dsrsrc.trainMangeDataSet);
			if (dsrsrc.trainMangeDataSet.TraFinanceDataTable.Rows.Count > 0)
			{
				Double dRecharge = 0;
				Double dConsumption = 0;
				Double dBalance = 0;
				double dBookBalance = 0;
				dataGridFinace.ItemsSource = dsrsrc.trainMangeDataSet.TraFinanceDataTable.DefaultView;
				for (int i = 0; i < dataGridFinace.Items.Count; i++)
				{
					var tempRow = (DataRowView)dataGridFinace.Items[i];
					dRecharge += tempRow["TOTALRECHARGE"].ToString() != "" ? Double.Parse(tempRow["TOTALRECHARGE"].ToString()) : 0;
					dConsumption += tempRow["TOTALACCOUNT"].ToString() != "" ? Double.Parse(tempRow["TOTALACCOUNT"].ToString()) : 0;
					dBalance += tempRow["BALANCE"].ToString() != "" ? Double.Parse(tempRow["BALANCE"].ToString()) : 0;
					dBookBalance += tempRow["TOTALBOOKACCOUNT"].ToString() != "" ? Double.Parse(tempRow["TOTALBOOKACCOUNT"].ToString()) : 0;
				}			
			}
            dsrsrc.MainBarText = "共查询到：" + dsrsrc.trainMangeDataSet.TraFinanceDataTable.Rows.Count + "条数据";

		}
		private void datePickerSerStr_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			m_StarTime = this.datePickerSerStr.SelectedDate.Value;
		}

		private void datePickerSerEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			m_EndTime = this.datePickerSerEnd.SelectedDate.Value;
		}	
	}
}
