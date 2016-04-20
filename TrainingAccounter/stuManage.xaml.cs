using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	/// stuManage.xaml 的交互逻辑
	/// </summary>
	public partial class stuManage : Page
	{
		public stuManage(DsRsrc dsrsrcPara)
		{
			InitializeComponent();
			dsrsrc = dsrsrcPara;
			txbPidNo.SetBinding(TextBox.TextProperty, new Binding("PidNo") { Source = dsrsrc });
			dsrsrc.getAllDrvSchool();			
			cboxSerDrv.ItemsSource = dsrsrc.trainMangeDataSet.DrvSchoolDataTable.DefaultView;		
		}		
		DsRsrc dsrsrc = null;
		DateTime m_StarTime;
		DateTime m_EndTime;
		string sChargingMode = "Tries";     
		private void btnAddTrainee_Click(object sender, RoutedEventArgs e)
		{
			AddTrainee addTra = new AddTrainee(dsrsrc);	
			addTra.ShowDialog();
		}
		public void serchTrainee()
		{
			try
			{
				dsrsrc.SearchTrainee(txbPidNo.Text.Trim(), "", "", "", cboxSerDrv.SelectedValue != null ? int.Parse(cboxSerDrv.SelectedValue.ToString()) : -1, DateTime.Parse(dtFrom.SelectedDate.Value.ToString()), DateTime.Parse(dtTo.SelectedDate.Value.ToString()), "1");
				if (dsrsrc.trainMangeDataSet.TraineeDataTable.Rows.Count > 0)
				{
					for (int i = 0; i < dsrsrc.trainMangeDataSet.TraineeDataTable.Rows.Count; i++)
					{
						if (dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[i]["STUDENT_TYPE"].ToString() == "0")
							dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[i]["STUDENT_TYPE_NAME"] = "普通用户";
						else if (dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[i]["STUDENT_TYPE"].ToString() == "1")
						{
							dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[i]["STUDENT_TYPE_NAME"] = "VIP用户";
						}
					}
					dgSearchTrainee.ItemsSource = dsrsrc.trainMangeDataSet.TraineeDataTable.DefaultView;
				}
				else
				{
					MessageBox.Show("没有查询到学员注册信息，请点击‘添加新学员’按钮进行添加.");
				}
                dsrsrc.MainBarText = "共查询到：" + dsrsrc.trainMangeDataSet.TraineeDataTable.Rows.Count + "条数据";

			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show("获取学员信息出错，错误信息：" + ex.Message.ToString());
			}
		}
		private void btnSearchTrainee_Click(object sender, RoutedEventArgs e)
		{
			serchTrainee();
		}
		//充值事件
		private void btnDeposit_Click(object sender, RoutedEventArgs e)
		{
			Recharge reg = new Recharge(dsrsrc);
            reg._sChargingMode = dsrsrc.sBillingMode;
			 if (dgSearchTrainee.SelectedItems.Count > 0)
			{
				var temp = (DataRowView)dgSearchTrainee.SelectedItems[0];
				reg.tboxSeqNo.Text = temp["SEQ_NO"].ToString();
				reg.txbUserName.Text = temp["TRAINE_NAME"].ToString();
				reg.txbPidNo.Text = temp["PID_NO"].ToString();
				reg.tboxBlce.Text = temp["BALANCE"].ToString();
				reg._sCarType = temp["LICENSE_TYPE_CD"].ToString();
				dsrsrc.getTraStudinfoBySeqNo(double.Parse(temp["SEQ_NO"].ToString()));
				if (dsrsrc.trainMangeDataSet.TraineeDataTable.Rows.Count > 0)
				{
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
			serchTrainee();
		
		}
		private void dgSearchTrainee_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Point aP = e.GetPosition(dgSearchTrainee);
			IInputElement obj = dgSearchTrainee.InputHitTest(aP);
			DependencyObject target = obj as DependencyObject;
			while (target != null)
			{
				if (target is DataGridRow)
				{
					DataGridRow aDGR = target as DataGridRow;
					DataRowView theDRV = aDGR.Item as DataRowView;
					this.txbPidNo.Text=theDRV["PID_NO"].ToString();
					break;
				}
				target = VisualTreeHelper.GetParent(target);
			}
		}
		private void dtFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			m_StarTime = this.dtFrom.SelectedDate.Value;
		}

		private void dtTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			m_EndTime = dtTo.SelectedDate.Value;
		}
	}
}
