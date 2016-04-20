using DBAccessProc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TrainingControl;

namespace TrainingAccounter
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
		public class sss
		{
	public string abc;
	public string vbn;
	}

        public Login()
		{		
            InitializeComponent();
            this.txtExaminerId.Focus();
			DBAccessProc.ConfigurationManager.ConfigFileName = "TraineeManage.config";
			MouseLeftButtonDown += (o, args) => {
			   var hwnd = new WindowInteropHelper(this).Handle;
			   ReleaseCapture();
			   SendMessage(hwnd, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
		   };
        }
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		DsRsrc dsrsrc = new DsRsrc();
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
			if(!ChkOCDBLink(DBAccessProc.Common.DBConnectionString))
			{
				MessageBox.Show("数据库连接失败，请检查网络或配置");
				return;
			}
			try
			{
				if (txtExaminerId.Text.Trim() == "")
				{
				MessageBox.Show("请输入考试员/操作员编号！");
					return;
				}
				if (txtPassword.Password.Trim() == "")
				{
					MessageBox.Show("请输入密码！");
					return;
				}						
				DBAccessHelper.GetExaminer(txtExaminerId.Text.Trim().ToUpper(), txtPassword.Password.Trim(), dsrsrc.trainMangeDataSet);
				if (dsrsrc.trainMangeDataSet.TraManagerDataTable.Rows.Count == 0)
				{
					MessageBox.Show("登录失败！考试员/操作员编号和密码不匹配，请重试。");
					txtExaminerId.Text = "";
					txtPassword.Password = "";
				}
				else
				{				
					MainWindow mainWindow = new MainWindow();
					mainWindow.Show();
					this.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("登录失败！原因：" + ex.Message + "，位置：" + ex.StackTrace);
				Environment.Exit(0);
			}
			//string strPwd = DateTime.Today.ToString("yyyyMMdd");
			//if (pswBoxLogin.Password.Trim() == strPwd)
			//{
			//	MainWindow mainWindow = new MainWindow();
			//	mainWindow.Show();
			//	this.Close();
			//}
			//else
			//{
			//	MessageBox.Show("密码错误！");
			//	this.pswBoxLogin.Clear();
			//	this.pswBoxLogin.Focus();
			//}
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
		public static bool ChkOCDBLink(string connString)
		{
			bool result = true;
			SqlConnection conn = null;
			try {
				conn = new SqlConnection(connString);
				conn.Open();
				if (conn.State == ConnectionState.Open) {
					result = true;
					conn.Close();
				} else {
					result = false;
				}
			} catch (SqlException ex) {
				System.Diagnostics.Debug.WriteLine(ex.ToString());
				result = false;
			} finally {
				if (conn != null) {
					conn.Dispose();
					conn = null;
				}
			}
			return result;
		}

		private void minimizeButton_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void maximizeButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.WindowState == WindowState.Maximized)
				this.WindowState = WindowState.Normal;
			else
				this.WindowState = WindowState.Maximized;
		}

		private void closeButton_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				this.DragMove();
			}
		}

		private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (txtPassword.Background != null && txtPassword.Password.ToString() != "")
				txtPassword.Background = null;
			else if (txtPassword.Background == null && txtPassword.Password.ToString() == "")
			{
			
			}
		}

	
    }
}
