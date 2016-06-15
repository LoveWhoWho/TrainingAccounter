using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TrainingControl
{
    /// <summary>
    /// ChargingControl.xaml 的交互逻辑
    /// </summary>
    public partial class ChargingControl : Window
    {
        /// <summary>
        /// 更新学员信息
        /// </summary>
        /// <param name="traineeInfo">The trainee information.</param>
        public delegate void ShowTrineeInfo(ChargeControlInfo traineeInfo);
 
        /// <summary>
        /// Initializes a new instance of the <see cref="ChargingControl"/> class.
        /// </summary>
        public ChargingControl()
        {
            InitializeComponent();
            Top = 0;
            Left = SystemParameters.PrimaryScreenWidth / 2 - (Width / 2);
            ShowInTaskbar = false;
            Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Initializes the photo.
        /// </summary>
        private void InitPhoto()
        {
            Assembly assembly = GetType().Assembly;
            string[] s= assembly.GetManifestResourceNames();
            Stream streamSmall = assembly.GetManifestResourceStream("TrainingControl.Resources.head.png");
            BitmapImage myBitmapImage = new BitmapImage();
            if (streamSmall != null)
            {
                myBitmapImage.BeginInit();
                byte[] bytes = new byte[streamSmall.Length];
                streamSmall.Read(bytes, 0, bytes.Length);
                streamSmall.Seek(0L, SeekOrigin.Begin);
                myBitmapImage.StreamSource = new MemoryStream(bytes);
                myBitmapImage.EndInit();
            }
            //imgPhoto.Source = myBitmapImage;
        }

        /// <summary>
        /// Handles the MouseDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// Changes the show information.
        /// </summary>
        /// <param name="info">The information.</param>
        public void ChangeShowInfo(ChargeControlInfo info)
        {

            if (info != null)
            {
                txtTrainerName.Text = info.Name;
                int hour = (int)(info.Time / 3600);
                int min = (int)((info.Time - hour * 3600) / 60);
                int sec = (int)(info.Time - hour * 3600 - min * 60);
                txtStartTime.Text = "开始时间: " + info.StartTime;
                txtTrainedTime.Text = "训练时长: " + string.Concat(string.Format("{0:D2}", hour), ":", string.Format("{0:D2}", min), ":", string.Format("{0:D2}", sec));
                txtTrainedTries.Text = "训练次数: " + info.Tries + "次";
                txbBalance.Text = "当前余额：" + info.Balance + "元";
            }
        }

       
        /// <summary>
        /// Shows the trainer photo.
        /// </summary>
        /// <param name="base64Photo">The base64 photo.</param>
        public void ShowTrainerPhoto(string base64Photo)
        {
            try
            {
                if (!string.IsNullOrEmpty(base64Photo))
                {
                    byte[] photo = Convert.FromBase64String(base64Photo);
                    BitmapImage myBitmapImage = new BitmapImage();
                    myBitmapImage.BeginInit();
                    myBitmapImage.StreamSource = new MemoryStream(photo);
                    myBitmapImage.EndInit();
                    //this.imgPhoto.Source = myBitmapImage;
                }
            }
            catch
            {
                this.InitPhoto();
            }
        }

        /// <summary>
        /// Calls the change show information.
        /// </summary>
        /// <param name="info">The information.</param>
        public void CallChangeShowInfo(ChargeControlInfo info)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                this.ChangeShowInfo(info);
            }));
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
