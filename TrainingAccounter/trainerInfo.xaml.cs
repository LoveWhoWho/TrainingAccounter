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
	/// trainerInfo.xaml 的交互逻辑
	/// </summary>
	public partial class trainerInfo : Page
	{
		public trainerInfo(DsRsrc dsr)
		{
			InitializeComponent();
			dsrsrc = dsr;
			PopuTrainerPage();
		}
		DsRsrc dsrsrc;
		///
		///
		///
		//教练员管理界面代码
		#region
		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			this.tboxTrainerNo.Text = "";
			this.tBoxName.Text = "";
			this.tBoxPidNo.Text = "";
			this.tBoxPhoneNo.Text = "";
			imageTrainer.Source = null;
		}

		private void btnDel_Click(object sender, RoutedEventArgs e)
		{
			if (dataGridTrainer.SelectedItems.Count > 0 && System.Windows.MessageBox.Show("你确定删除吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{

				for (int i = 0; i < dataGridTrainer.SelectedItems.Count; i++)
				{
					var tempA = (DataRowView)dataGridTrainer.SelectedItems[i];
					dsrsrc.delTrainer(tempA["TRAINER_ID"].ToString());
				}
			}
			else
			{
				if (dataGridTrainer.SelectedItems.Count > 0)
					return;
				else
					System.Windows.Forms.MessageBox.Show("未选中任何行，请重新选择！");
			}
			PopuTrainerPage();
		}

		private void btnSaveTrainer_Click(object sender, RoutedEventArgs e)
		{
			if (this.tBoxName.Text.ToString().Trim() == "" || this.tBoxPidNo.Text.ToString().Trim() == "" || this.tBoxPhoneNo.Text.ToString().Trim() == "")
			{
				System.Windows.Forms.MessageBox.Show("请检查是否有信息为空.");
				return;
			}
			else
			{
				TrainingControl.Trainer TrainerCs = new TrainingControl.Trainer();
				TrainerCs.TrainerCtype = this.cboxCarTypeInTrainer.SelectedValue.ToString().Trim();
				TrainerCs.TrainerDrv = this.cboxDrivingSchool.SelectedValue != null ? this.cboxDrivingSchool.SelectedValue.ToString().Trim() : "0";
				TrainerCs.TrainerId = this.tboxTrainerNo.Text.ToString().Trim();
				//照片格式转换
				BitmapImage bmp = imageTrainer.Source as BitmapImage;
				if (bmp != null)
				{
					TrainerCs.TrainerImage = dsrsrc.PhotoSave(bmp);
				}
				TrainerCs.TrainerName = this.tBoxName.Text.ToString().Trim();
				TrainerCs.TrainerPhone = this.tBoxPhoneNo.Text.ToString().Trim();
				TrainerCs.TrainerPidNo = this.tBoxPidNo.Text.ToString().Trim();
				DataRow[] drow = dsrsrc.trainMangeDataSet.TrainerDataTable.Select("TRAINER_ID = '" + this.tboxTrainerNo.Text.ToString().Trim() + "'");
				if (drow.Length > 0)
				{
					if (System.Windows.MessageBox.Show("已经有相同编号的教练员存在,确认更新ID为" + this.tboxTrainerNo.Text.ToString().Trim() + "的教练员信息？", "提示：", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
						dsrsrc.AddTrainer("Y");
					else
						return;

				}
				else
				{
					dsrsrc.AddTrainer("");
				}

			}
			PopuTrainerPage();
		}
		private void cboxCarTypeInTrainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//if (isTrigger)
			//{
			//    if (this.cboxCarTypeInTrainer.SelectedValue == null)
			//        return;
			//    else
			//    {
			//        //  this.cboxPlateNo.ItemsSource = dsrsrc.trainMangeDataSet.TraCarDataTable.Select("AUTO_TYPE_CD = '" + this.cboxCarType.SelectedValue.ToString() + "'");
			//        PopuTrainerPage();
			//    }
			//}
		}
		private void dataGridTrainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (dataGridTrainer.SelectedItems.Count == 0)
				return;
			var tempC = (DataRowView)dataGridTrainer.SelectedItems[0];
			this.tboxTrainerNo.Text = tempC["TRAINER_ID"].ToString();
			this.tBoxName.Text = tempC["TRAINER_NAME"].ToString();
			this.tBoxPidNo.Text = tempC["PidNo"].ToString();
			this.tBoxPhoneNo.Text = tempC["PHONE_NO"].ToString();
			if (tempC["PHOTO"] != null && tempC["PHOTO"].ToString() != "")
			{
				imageTrainer.Source = dsrsrc.ByteArrayToBitmapImage((byte[])tempC["PHOTO"]);
			}
			else
				imageTrainer.Source = null;
			this.cboxCarTypeInTrainer.SelectedValue = tempC["AllowableLicenseType"].ToString();
			this.cboxDrivingSchool.SelectedValue = tempC["DrvSchoolId"].ToString();
		}
		private void PopuTrainerPage()
		{

			//车型下拉列表
			this.cboxCarTypeInTrainer.ItemsSource = dsrsrc.TrainLicenseType;
			if (cboxCarTypeInTrainer.SelectedValue == null)
			{
				//isTrigger = false;
				cboxCarTypeInTrainer.SelectedIndex = 0;
				//isTrigger = true;
			}
			//驾校下拉列表赋值 
			if (dsrsrc.trainMangeDataSet.DrvSchoolDataTable.Rows.Count > 0)
			{
				this.cboxDrivingSchool.ItemsSource = dsrsrc.trainMangeDataSet.DrvSchoolDataTable.DefaultView;
			}
			dsrsrc.trainMangeDataSet.TrainerDataTable.Clear();
			dsrsrc.GetAllTrainer();
			if (dsrsrc.trainMangeDataSet.TrainerDataTable.Rows.Count > 0)
			{
				dataGridTrainer.ItemsSource = dsrsrc.trainMangeDataSet.TrainerDataTable.DefaultView;

			}
			else
			{
				System.Windows.Forms.MessageBox.Show("查询到的教练员信息为空。请检查教练员配置信息");

			}


		}
		#endregion

		private void btnPhotoTrainer_Click(object sender, RoutedEventArgs e)
		{
			var photoSource = dsrsrc.getPhotoFromCamra();
			if (photoSource!=null)
			dsrsrc.ShowImageFromBuffer(photoSource, this.imageTrainer);
		}
	}
}
