using System;
using System.Collections.Generic;
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
using WPF.DazzleUI2.Controls;
using TrainingControl;
using System.Threading;
using WPF.DazzleUI2;
using System.Reflection;

namespace TrainingAccounter
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : DazzleWindow
	{
		public MainWindow()
		{
			DBAccessProc.ConfigurationManager.ConfigFileName = "TraineeManage.config";
			//注册控件
			if (dsrsrc.RegisterControls("TrainingAccounter"))
				axLibCtl = new AxLib.AxLibControl();
			InitializeComponent();
            statusBarItem.SetBinding(System.Windows.Controls.Primitives.StatusBarItem.ContentProperty, new Binding("MainBarText") { Source = dsrsrc });
			tbxTitle.Text = "驾训管理系统   主版本:1.0";
			statusBar.Content ="版本详情："+System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			//读卡器线程
			SearchExamineeThread = new Thread(new ThreadStart(SearchExaminee));
			SearchExamineeThread.IsBackground = true;
			SearchExamineeThread.Start();				
		}	
		public delegate void UpdatePidNoDelegate(string pidNo, string name,byte[] photo, string adress, bool status);
		DsRsrc dsrsrc = new DsRsrc();

		Thread SearchExamineeThread = null;//读卡器   
		AxLib.AxLibControl axLibCtl = null;

		
		private void DazzleButton_Click(object sender, RoutedEventArgs e)
		{
			Environment.Exit(0);
		}
		private void DazzleButton_Click_1(object sender, RoutedEventArgs e)
		{
			this.WindowState = System.Windows.WindowState.Maximized;
		}
		private void DazzleButton_Click_2(object sender, RoutedEventArgs e)
		{
			this.WindowState = System.Windows.WindowState.Minimized;
		}
		private void menu_Click(object sender, RoutedEventArgs e)
		{
			menu1.IsOpen = true;
		}
		//主线程方法
		private void SearchExaminee()
		{
			try
			{
				while (true)
				{
					Thread.Sleep(800);
					string sRetInfo = axLibCtl.GetIDCardInfo();
					if (sRetInfo == "ReadSuccess")
					{
						if (sRetInfo != "")
							this.Dispatcher.Invoke(new UpdatePidNoDelegate(updatePidNo), axLibCtl.idCard.sPidNo.ToString(), axLibCtl.idCard.sName.ToString(), axLibCtl.idCard.sSex.ToString(), axLibCtl.idCard.sPhotoBuffer, axLibCtl.idCard.sAddress, true);
						else
							this.Dispatcher.Invoke(new UpdatePidNoDelegate(updatePidNo), "", "", null, "", true);
					}
					else if (sRetInfo == "InitFalse")
					{
						this.Dispatcher.Invoke(new UpdatePidNoDelegate(updatePidNo), "", "", null, "", false);
					}
					else
					{
						this.Dispatcher.Invoke(new UpdatePidNoDelegate(updatePidNo), "", "", null, "", true);
					}
				}
			}
			catch (Exception e)
			{
				System.Windows.MessageBox.Show("查询信息失败，错误原因：" + e.Message);
			}
		}
		private void updatePidNo(string strpidNo, string name, byte[] photo, string address, bool status)
		{
			if (status && strpidNo != "")
			{
				TrainingControl.saveCardInfo sCardInfo = new saveCardInfo();				
				this.imageReaderStasus.Source = new BitmapImage(new Uri(@"/TrainingAccounter;component/Resource/Yes.png", UriKind.Relative));
				dsrsrc.PidNo = strpidNo;
				dsrsrc.Photo =dsrsrc.getImageSource(photo);
				dsrsrc.Name = name;
				dsrsrc.Address = address;				
			}
			else if (status && strpidNo == "")
			{			
				this.imageReaderStasus.Source = new BitmapImage(new Uri(@"/TrainingAccounter;component/Resource/Yes.png", UriKind.Relative));
			}
			else
			{			
				this.imageReaderStasus.Source = new BitmapImage(new Uri(@"/TrainingAccounter;component/Resource/No.png", UriKind.Relative));
			}
		}	
		private void serstudent_Click(object sender, RoutedEventArgs e)
		{
			stuManage stuwindow = new stuManage(dsrsrc);
			this.frameStu.Content = stuwindow;
			serstudent.Background = new SolidColorBrush(Color.FromRgb(20, 126, 245));
			meuRecharge.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
			serManage.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
		}
		private void meuRecharge_Click(object sender, RoutedEventArgs e)
		{
			rechargeManage regwindow = new rechargeManage(dsrsrc);
			this.frameStu.Content = regwindow;
			serstudent.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
			meuRecharge.Background = new SolidColorBrush(Color.FromRgb(20, 126, 245));
			serManage.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
		}

		private void serManage_Click(object sender, RoutedEventArgs e)
		{
			settleMent settwindow = new settleMent(dsrsrc);
			this.frameStu.Content = settwindow;
			serstudent.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
			meuRecharge.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
			serManage.Background = new SolidColorBrush(Color.FromRgb(20, 126, 245));
		}

		private void perBook_Click(object sender, RoutedEventArgs e)
		{
			bookMange bookwindow = new bookMange(dsrsrc);
			this.frameBook.Content = bookwindow;
			perBook.Background = new SolidColorBrush(Color.FromRgb(20, 126, 245));
			muiBook.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));

		}

		private void muiBook_Click(object sender, RoutedEventArgs e)
		{
			BulkBooking bookwindow = new BulkBooking(dsrsrc);
			this.frameBook.Content = bookwindow;
			muiBook.Background = new SolidColorBrush(Color.FromRgb(20, 126, 245));
			perBook.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
		}

		private void DazzleTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.Source is TabControl)
			{
				if ((e.Source as TabControl).SelectedIndex == 0)
				{
					stuManage stuwindow = new stuManage(dsrsrc);
					this.frameStu.Content = stuwindow;
					serstudent.Background = new SolidColorBrush(Color.FromRgb(20, 126, 245));
					meuRecharge.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
					serManage.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
				}
				else if ((e.Source as TabControl).SelectedIndex == 1)
				{
					bookMange bookwindow = new bookMange(dsrsrc);
					this.frameBook.Content = bookwindow;
					perBook.Background = new SolidColorBrush(Color.FromRgb(20, 126, 245));
					muiBook.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
				}
				else if ((e.Source as TabControl).SelectedIndex == 2)
				{
					serTraResult serWindow = new serTraResult(dsrsrc);
					this.frameSer.Content = serWindow;
				}
				else if ((e.Source as TabControl).SelectedIndex == 3)
				{
					drvInfo infoWindow = new drvInfo(dsrsrc);
					frameInfo.Content = infoWindow;
					drvInfo.Background = new SolidColorBrush(Color.FromRgb(20, 126, 245));
					carInfo.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
					trainerInfo.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
				}
				else
				{
					userInfo infoWindow = new userInfo(dsrsrc);
					frameUser.Content = infoWindow;
					userInfo.Background = new SolidColorBrush(Color.FromRgb(20, 126, 245));
					baseInfo.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));		
				}
			}
		}

		private void trainerInfo_Click(object sender, RoutedEventArgs e)
		{
			trainerInfo infoWindow = new trainerInfo(dsrsrc);
			frameInfo.Content = infoWindow;
			trainerInfo.Background = new SolidColorBrush(Color.FromRgb(20, 126, 245));
			carInfo.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
			drvInfo.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
		}

		private void carInfo_Click(object sender, RoutedEventArgs e)
		{
			carInfo infoWindow = new carInfo(dsrsrc);
			frameInfo.Content = infoWindow;
			carInfo.Background = new SolidColorBrush(Color.FromRgb(20, 126, 245));
			drvInfo.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
			trainerInfo.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
		}

		private void drvInfo_Click(object sender, RoutedEventArgs e)
		{
			drvInfo infoWindow = new drvInfo(dsrsrc);
			frameInfo.Content = infoWindow;
			drvInfo.Background = new SolidColorBrush(Color.FromRgb(20, 126, 245));
			carInfo.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
			trainerInfo.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));
		}

		private void userInfo_Click(object sender, RoutedEventArgs e)
		{
			userInfo infoWindow = new userInfo(dsrsrc);
			frameUser.Content = infoWindow;
			userInfo.Background = new SolidColorBrush(Color.FromRgb(20, 126, 245));
			baseInfo.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));		
		}

		private void baseInfo_Click(object sender, RoutedEventArgs e)
		{
			baseInfo infoWindow = new baseInfo(dsrsrc);
			frameUser.Content = infoWindow;
			baseInfo.Background = new SolidColorBrush(Color.FromRgb(20, 126, 245));
			userInfo.Background = new SolidColorBrush(Color.FromRgb(217, 223, 239));		
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			versionChange vc = new versionChange();
			vc.ShowDialog();
		}

		private void MenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			Environment.Exit(0);
		}
		

	}
}
