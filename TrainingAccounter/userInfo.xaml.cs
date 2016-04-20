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
	/// userInfo.xaml 的交互逻辑
	/// </summary>
	public partial class userInfo : Page
	{
		public userInfo(DsRsrc dsr)
		{
			InitializeComponent();
			dsrsrc = dsr;
			PopuTraManagerPage();
		}
		DsRsrc dsrsrc;
		///
		///
		///
		//用户管理界面部分代码
		private void btnAddUser_Click(object sender, RoutedEventArgs e)
		{
			this.tboxPhone.Text = "";
			this.tboxPwd.Text = "";
			this.tboxUserId.Text = "";
			this.tboxUserName.Text = "";
			this.imageUserManger.Source = null;
		}

		private void btnDelUser_Click(object sender, RoutedEventArgs e)
		{
			if (dataGridTraManager.SelectedItems.Count > 0 && System.Windows.MessageBox.Show("你确定删除吗？", "提示", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
			{

				for (int i = 0; i < dataGridTraManager.SelectedItems.Count; i++)
				{
					var tempA = (DataRowView)dataGridTraManager.SelectedItems[i];
					dsrsrc.delTraManager(tempA["TRA_MANAGER_ID"].ToString());
				}
			}
			else
			{
				if (dataGridTraManager.SelectedItems.Count > 0)
					return;
				else
					System.Windows.Forms.MessageBox.Show("未选中任何行，请重新选择！");
			}
			PopuTraManagerPage();
		}

		private void btnSaveUser_Click(object sender, RoutedEventArgs e)
		{
			if (this.tboxPwd.Text.ToString().Trim() == "" || this.tboxUserId.Text.ToString().Trim() == "" || this.tboxUserName.Text.ToString().Trim() == "" || this.cboxUserType.SelectedValue.ToString().Trim() == "")
			{
				System.Windows.Forms.MessageBox.Show("请检查是否有信息为空.");
				return;
			}
			else
			{
				TrainingControl.TraUser trvuser = new TrainingControl.TraUser();
				trvuser.UserId = this.tboxUserId.Text.ToString().Trim();
				trvuser.UserName = this.tboxUserName.Text.ToString().Trim();
				trvuser.UserPhone = this.tboxPhone.Text.ToString().Trim();
				BitmapImage bmp = imageUserManger.Source as BitmapImage;
				if (bmp != null)
				{
					trvuser.UserPhoto = dsrsrc.PhotoSave(bmp);
				}
				trvuser.UserPwd = this.tboxPwd.Text.ToString().Trim();
				trvuser.UserType = this.cboxUserType.SelectedValue.ToString();
				DataRow[] drow = dsrsrc.trainMangeDataSet.TraManagerDataTable.Select("TRA_MANAGER_ID = '" + this.tboxUserId.Text.ToString().Trim() + "'");
				if (drow.Length > 0)
				{
					if (System.Windows.MessageBox.Show("已经有相同ID的用户存在，确认更新用户ID为" + this.tboxUserId.Text.ToString().Trim() + "的用户信息吗？", "提示：", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Information) == System.Windows.MessageBoxResult.Yes)
						dsrsrc.AddTraManager("Y");
					else
						return;

				}
				else
				{
					dsrsrc.AddTraManager("");
				}
			}
			PopuTraManagerPage();
		}

		/// <summary>
		/// 刷新界面的方法
		/// </summary>
		/// 
		private void PopuTraManagerPage()
		{
			dsrsrc.GetAllUser();		
			if (dsrsrc.trainMangeDataSet.TraManagerDataTable.Rows.Count > 0)
			{
				//管理员部分 
				dataGridTraManager.ItemsSource = dsrsrc.trainMangeDataSet.TraManagerDataTable.DefaultView;
				var tempB = (DataRowView)dataGridTraManager.Items[0];

				this.tboxUserId.Text = tempB["TRA_MANAGER_ID"].ToString();
				this.tboxPwd.Text = "******";
				this.tboxUserId.Text = tempB["TRA_MANAGER_ID"].ToString();
				this.tboxUserName.Text = tempB["TRA_MANAGER_NAME"].ToString();
				this.tboxPhone.Text = tempB["PHONE_NO"].ToString();
				if (!string.IsNullOrEmpty(tempB["AuthManagement"].ToString()))
					this.cboxUserType.SelectedValue = tempB["AuthManagement"].ToString();
				else { this.cboxUserType.SelectedIndex = 0; }
				if (tempB["PHOTO"] != null && tempB["PHOTO"].ToString() != "")
				{
					this.imageUserManger.Source = dsrsrc.ByteArrayToBitmapImage((byte[])tempB["PHOTO"]);
				}
				else
					this.imageUserManger.Source = null;
			}
			else
			{
				MessageBox.Show("没有查找到相关的用户配置信息,请启动后重新配置并重启程序");
			}	
		}
		private void dataGridTraManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//管理员部分		
			if (dataGridTraManager.SelectedItems.Count <= 0)
				return;
			var tempB = (DataRowView)dataGridTraManager.SelectedItems[0];
			this.tboxUserId.Text = tempB["TRA_MANAGER_ID"].ToString();
			this.tboxPwd.Text = "******";
			this.tboxUserId.Text = tempB["TRA_MANAGER_ID"].ToString();
			this.tboxUserName.Text = tempB["TRA_MANAGER_NAME"].ToString();
			this.tboxPhone.Text = tempB["PHONE_NO"].ToString();
			if (!string.IsNullOrEmpty(tempB["AuthManagement"].ToString()))
			{
				if (tempB["AuthManagement"].ToString()=="操作员")
					this.cboxUserType.SelectedIndex = 0;
				else
					this.cboxUserType.SelectedIndex = 1;
				//this.cboxUserType.SelectedValue = tempB["AuthManagement"].ToString();
			}				
			else
			{
				this.cboxUserType.SelectedIndex = 0;
			}
			if (tempB["PHOTO"] != null && tempB["PHOTO"].ToString() != "")
			{
				this.imageUserManger.Source = dsrsrc.ByteArrayToBitmapImage((byte[])tempB["PHOTO"]);
			}
			else
				this.imageUserManger.Source = null;
		}

		private void btnPhotoUser_Click(object sender, RoutedEventArgs e)
		{
			var photoSource = dsrsrc.getPhotoFromCamra();
			if (photoSource != null)
				dsrsrc.ShowImageFromBuffer(photoSource, this.imageUserManger);
		}

	}
}
