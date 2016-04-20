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
	/// carInfo.xaml 的交互逻辑
	/// </summary>
	public partial class carInfo : Page
	{
		public carInfo(DsRsrc dsr)
		{
			InitializeComponent();
			dsrsrc = dsr;
			dsrsrc.GetAllTrainer();
			dsrsrc.getAllTraCar();
			this.cboxCarType.ItemsSource = dsrsrc.TrainLicenseType;
			PopuTraCarPage();
		}
		DsRsrc dsrsrc;
		///<summary>
		///训练车管理界面代码
		///</summary>

		#region
		private void btnAddTraCar_Click(object sender, RoutedEventArgs e)
		{
			this.tboxCarNo.Text = "";
			this.tboxPlate.Text = "";
			this.tboxName.Text = "";
			this.tboxBrand.Text = "";
		}

		private void btnDelTraCar_Click(object sender, RoutedEventArgs e)
		{
			if (dataGridTraCar.SelectedItems.Count > 0 && System.Windows.MessageBox.Show("你确定删除吗？", "提示", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
			{

				for (int i = 0; i < dataGridTraCar.SelectedItems.Count; i++)
				{
					var tempA = (DataRowView)dataGridTraCar.SelectedItems[i];
					dsrsrc.delTraCar(tempA["AUTO_ID"].ToString());
				}
			}
			else
			{
				if (dataGridTraCar.SelectedItems.Count > 0)
					return;
				else
					System.Windows.Forms.MessageBox.Show("未选中任何行，请重新选择！");
			}
			PopuTraCarPage();
		}
		private void dataGridTraCar_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (dataGridTraCar.SelectedItems.Count <= 0)
				return;
			var tempC = (DataRowView)dataGridTraCar.SelectedItems[0];
			this.tboxBrand.Text = tempC["BRAND"].ToString();
			this.tboxCarNo.Text = tempC["AUTO_ID"].ToString();
			this.tboxName.Text = tempC["AUTO_NAME"].ToString();
			this.tboxPlate.Text = tempC["PLATE_NO"].ToString();
			if (!string.IsNullOrEmpty(tempC["AUTO_STATUS_CD"].ToString()))
			{
				if (tempC["AUTO_STATUS_CD"].ToString() == "可用")
					this.cboxCarStatus.SelectedIndex = 0;
				else
					this.cboxCarStatus.SelectedIndex = 1;
			//	this.cboxCarStatus.SelectedValue = tempC["AUTO_STATUS_CD"].ToString();
			}
		
			this.cboxCarType.SelectedValue = tempC["AUTO_TYPE_CD"].ToString();
			string m_sTrainerName = tempC["TRAINER_NAME"].ToString();
			this.cboxTrainer.Text = m_sTrainerName;
		}

		private void cboxCarType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{			
				if (this.cboxCarType.SelectedValue == null)
					return;
				else
				{
					try
					{
						//获取改变后的值
						object obj = (object)e.AddedItems;
						string strCarType = ((object[])obj)[0].ToString();
						this.cboxTrainer.ItemsSource = dsrsrc.trainMangeDataSet.TrainerDataTable.Select("AllowableLicenseType = '" + strCarType + "'");
					}
					catch (Exception ex)
					{
						System.Windows.Forms.MessageBox.Show(ex.ToString());
						return;
					}
				}
			

		}
		private void PopuTraCarPage()
		{
			dsrsrc.getAllTraCar();
			dsrsrc.GetAllTrainer();
			dsrsrc.GetAllSetting();			
			this.cboxCarType.ItemsSource = dsrsrc.TrainLicenseType;			
			if (cboxCarType.SelectedValue == null)
			{
				cboxCarType.SelectedIndex = 0;
			}
			//获取教练员信息列表         
			if (dsrsrc.trainMangeDataSet.TrainerDataTable.Rows.Count > 0)
			{
				if (this.cboxCarType.SelectedValue == null)
					return;
				DataRow[] drvs = dsrsrc.trainMangeDataSet.TrainerDataTable.Select("AllowableLicenseType = '" + this.cboxCarType.SelectedValue.ToString() + "'");
				if (drvs.Length > 0)
				{
					DataTable trainerSource = dsrsrc.trainMangeDataSet.TrainerDataTable.Clone();
					foreach (DataRow dr in drvs)
					{
						trainerSource.Rows.Add(dr.ItemArray);
					}
					this.cboxTrainer.ItemsSource = trainerSource.DefaultView;
				}
				else
					this.cboxTrainer.ItemsSource = null;
			}

			if (dsrsrc.trainMangeDataSet.TraCarDataTable.Rows.Count > 0)
			{
				dataGridTraCar.ItemsSource = dsrsrc.trainMangeDataSet.TraCarDataTable.DefaultView;
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("查询到的车辆信息为空。请检查车辆配置信息");
			}
		}
		private void btnSaveTraCar_Click(object sender, RoutedEventArgs e)
		{
			if (this.tboxBrand.Text.ToString().Trim() == "" || this.tboxCarNo.Text.ToString().Trim() == "" || this.tboxName.Text.ToString().Trim() == "")
			{
				System.Windows.Forms.MessageBox.Show("车辆品牌、编号、名称不能为空,请检查是否有信息为空.");
				return;
			}
			else
			{
				TrainingControl.TrainAuto trainauto = new TrainingControl.TrainAuto();
				trainauto.CarBrand = this.tboxBrand.Text.ToString().Trim();
				trainauto.CarName = this.tboxName.Text.ToString().Trim();
				trainauto.CarNo = this.tboxCarNo.Text.ToString().Trim();
				trainauto.CarStatus = this.cboxCarStatus.SelectedValue != null ? this.cboxCarStatus.SelectedValue.ToString().Trim() : "";
				trainauto.CarTrainer = this.cboxTrainer.SelectedValue != null ? this.cboxTrainer.SelectedValue.ToString().Trim() : "";
				trainauto.CarType = this.cboxCarType.SelectedValue != null ? this.cboxCarType.SelectedValue.ToString().Trim() : "";
				trainauto.PlateNo = this.tboxPlate.Text.ToString().Trim();
				DataRow[] drow = dsrsrc.trainMangeDataSet.TraCarDataTable.Select("AUTO_ID = '" + this.tboxCarNo.Text.ToString().Trim() + "'");
				if (drow.Length > 0)
				{
					if (System.Windows.MessageBox.Show("已经有相同ID的教练车存在,确认更新ID为" + this.tboxCarNo.Text.ToString() + "的教练车信息吗？", "提示：", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
						dsrsrc.AddTraCar("Y");
                        dsrsrc.MainBarText = "更新车号为：" + this.tboxCarNo.Text.ToString() + "的考车信息到本地数据库成功";
                    }
					else
						return;
				}
				else
				{
					dsrsrc.AddTraCar("");
                    dsrsrc.MainBarText = "新增车号为：" + this.tboxCarNo.Text.ToString() + "的考车信息到本地数据库成功";
				}

			}
			PopuTraCarPage();
		}
		#endregion
	}
}
