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
	/// drvInfo.xaml 的交互逻辑
	/// </summary>
	public partial class drvInfo : Page
	{
		public drvInfo(DsRsrc dsr)
		{
			InitializeComponent();
			dsrsrc = dsr;
			PopuDrvPage();
		}
		DsRsrc dsrsrc;		
		///
		//驾校管理界面代码
		///
		#region
		private void btnDelDrv_Click(object sender, RoutedEventArgs e)
		{
			if (dataGridDrv.SelectedItems.Count > 0 && System.Windows.MessageBox.Show("你确定删除此条驾校信息吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{

				for (int i = 0; i < dataGridDrv.SelectedItems.Count; i++)
				{
					var tempA = (DataRowView)dataGridDrv.SelectedItems[i];
					dsrsrc.delDrvSchool(tempA["DRIVING_SCHOOL_ID"].ToString());
				}
			}
			else
			{
				if (dataGridDrv.SelectedItems.Count > 0)
					return;
				else
					System.Windows.Forms.MessageBox.Show("未选中任何行，请重新选择！");
			}
			PopuDrvPage();
		}
		private void btnSaveDrv_Click(object sender, RoutedEventArgs e)
		{

			if (tBoxDrvName.Text.ToString().Trim() == "" || tBoxDrvAddress.Text.ToString().Trim() == "" || tBoxPhone.Text.ToString().Trim() == "" || tBoxDsShortName.Text.ToString().Trim() == "" || tBoxContact.Text.ToString().Trim() == "")
			{
				System.Windows.Forms.MessageBox.Show("请检查是否有信息为空.");
				return;
			}
			else
			{
				TrainingControl.DrvSchool drvSh = new TrainingControl.DrvSchool();
				if (dsrsrc.IsRegEx("^(-?[0-9]*[.]*[0-9]{0,3})$", this.tboxdrvid.Text.ToString().Trim()))
					drvSh.DrvId = Convert.ToInt32(this.tboxdrvid.Text.ToString().Trim());
				else
				{
					System.Windows.Forms.MessageBox.Show("驾校ID必须为数字，请重新输入");
					return;
				}
				drvSh.drvname = tBoxDrvName.Text.ToString().Trim();
				drvSh.drvaddress = tBoxDrvAddress.Text.ToString().Trim();
				drvSh.drvphoneno = tBoxPhone.Text.ToString().Trim();
				drvSh.drvshortname = tBoxDsShortName.Text.ToString().Trim();
				drvSh.drvcontact = tBoxContact.Text.ToString().Trim();
				DataRow[] drow = dsrsrc.trainMangeDataSet.DrvSchoolDataTable.Select("DRIVING_SCHOOL_ID = '" + Convert.ToInt32(this.tboxdrvid.Text.ToString().Trim()) + "'");
				if (drow.Length > 0)
				{

					if (System.Windows.MessageBox.Show("已经存在相同ID的驾校信息，确认更新ID为" + this.tboxdrvid.Text.ToString().Trim() + "的驾校信息吗？", "提示：", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
						dsrsrc.AddDrvSchool("Y");
					else
						return;

				}
				else
				{ dsrsrc.AddDrvSchool(""); }
			}
			PopuDrvPage();
		}

		private void btnAddDrv_Click(object sender, RoutedEventArgs e)
		{
			tboxdrvid.Text = "";
			tBoxDrvName.Text = "";
			tBoxDrvAddress.Text = "";
			tBoxPhone.Text = "";
			tBoxDsShortName.Text = "";
			tBoxContact.Text = "";
		}
		/// <summary>
		/// 刷新界面
		/// </summary>
		private void PopuDrvPage()
		{
			dsrsrc.getAllDrvSchool();
			if (dsrsrc.trainMangeDataSet.DrvSchoolDataTable.Rows.Count > 0)
			{
				dataGridDrv.ItemsSource = dsrsrc.trainMangeDataSet.DrvSchoolDataTable.DefaultView;
				var tempB = (DataRowView)dataGridDrv.Items[0];
				this.tBoxContact.Text = tempB["CONTACT"].ToString();
				this.tBoxDrvAddress.Text = tempB["ADDRESS"].ToString();
				this.tBoxDrvName.Text = tempB["DS_NAME"].ToString();
				this.tBoxDsShortName.Text = tempB["DS_SHORT_NAME"].ToString();
				this.tBoxPhone.Text = tempB["PHONE_NO"].ToString();
				this.tboxdrvid.Text = tempB["DRIVING_SCHOOL_ID"].ToString();

			}
			else
			{
				System.Windows.Forms.MessageBox.Show("查询到的驾校信息为空。请检查驾校配置信息");
			}
		}
		/// <summary>
		/// 选择某一行切换显示
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataGridDrv_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (dataGridDrv.Items.Count == 0)
				return;
			var tempC = (DataRowView)dataGridDrv.SelectedItems[0];
			this.tBoxContact.Text = tempC["CONTACT"].ToString();
			this.tBoxDrvAddress.Text = tempC["ADDRESS"].ToString();
			this.tBoxDrvName.Text = tempC["DS_NAME"].ToString();
			this.tBoxDsShortName.Text = tempC["DS_SHORT_NAME"].ToString();
			this.tBoxPhone.Text = tempC["PHONE_NO"].ToString();
			this.tboxdrvid.Text = tempC["DRIVING_SCHOOL_ID"].ToString();
		}
		#endregion
	}
}
