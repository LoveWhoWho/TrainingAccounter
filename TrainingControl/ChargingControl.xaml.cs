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
        /// 
        /// </summary>
        /// <param name="name">The name.</param>
        public delegate void ShowName(string name);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basePhoto">The base photo.</param>
        public delegate void ShowPhoto(string basePhoto);
        /// <summary>
        /// The _brush
        /// </summary>
        private readonly Brush _brush = new SolidColorBrush(Colors.OrangeRed);
        /// <summary>
        /// Initializes a new instance of the <see cref="ChargingControl"/> class.
        /// </summary>
        public ChargingControl()
        {
            InitializeComponent();
            Top = SystemParameters.PrimaryScreenHeight - Height;
            Left = 1.0;
            InitPhoto();
            ShowInTaskbar = false;
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
            imgPhoto.Source = myBitmapImage;
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
        public void ChangeShowInfo(ChargesInfo info)
        {
            if (info != null)
            {
                labTime.FontWeight = FontWeights.Normal;
                labTime.Foreground = new SolidColorBrush(Colors.Black);
                txtTrainedTime.Foreground = new SolidColorBrush(Colors.Black);
                switch (info.Mode)
                {
                    case "Time":
                    {
                        labTime.FontWeight = FontWeights.Bold;
                        labTime.Foreground = _brush;
                        txtTrainedTime.Foreground = _brush;
                        int hour = (int) info.CurrentMinutes/1;
                        int min = (int) ((info.CurrentMinutes - hour)*60.0/1.0);
                        int sec = (int) (Math.Round((info.CurrentMinutes - hour)*60.0 - min, 3)*60.0);
                        txtTrainedMileage.Text = Math.Round(Math.Abs(info.CurrentMileage*1000.0), 2) + "米";
                        txtTrainedTime.Text = string.Concat(hour, ":", min, ":", sec);
                        txtTrainedTimes.Text = Math.Abs(info.SurplusTimes) + "次";
                         break;
                    }
                    case "Mileage":
                    {
                        labMileage.FontWeight = FontWeights.Bold;
                        labMileage.Foreground = _brush;
                        txtTrainedMileage.Foreground = _brush;
                        txtTrainedMileage.Text = Math.Round(info.CurrentMileage * 1000.0, 2) + "米";
                        txtTrainedTimes.Text = Math.Abs(info.SurplusTimes) + "次";
                        double cmin = Math.Abs(info.CurrentMinutes);
                        int hour = (int)cmin / 1;
                        int min = (int)((cmin - hour) * 60.0 / 1.0);
                        int sec = (int)(Math.Round((cmin - hour) * 60.0 - min, 3) * 60.0);
                        txtTrainedTime.Text = string.Concat(hour, ":", min, ":", sec);
                        break;
                    }
                    case "Tries":
                    {
                        labTries.FontWeight = FontWeights.Bold;
                        labTries.Foreground = _brush;
                        txtTrainedTimes.Foreground = _brush;
                        txtTrainedMileage.Text = Math.Round(Math.Abs(info.CurrentMileage * 1000.0), 2) + "米";
                        txtTrainedTimes.Text = info.SurplusTimes + "次";
                        double cmin = Math.Abs(info.CurrentMinutes);
                        int hour = (int)cmin / 1;
                        int min = (int)((cmin - hour) * 60.0 / 1.0);
                        int sec = (int)(Math.Round((cmin - hour) * 60.0 - min, 3) * 60.0);
                        txtTrainedTime.Text = string.Concat(hour, ":", min, ":", sec);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Shows the name of the trainer.
        /// </summary>
        /// <param name="name">The name.</param>
        public void ShowTrainerName(string name)
        {
            this.TrainerName.Text = name;
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
                    this.imgPhoto.Source = myBitmapImage;
                }
            }
            catch
            {
                this.InitPhoto();
            }
        }

        /// <summary>
        /// Calls the changes show information.
        /// </summary>
        /// <param name="info">The information.</param>
        public void CallChangesShowInfo(ChargesInfo info)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                this.ChangeShowInfo(info);
            }));
        }
        /// <summary>
        /// Calls the name of the show trainer.
        /// </summary>
        /// <param name="name">The name.</param>
        public void CallShowTrainerName(string name)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                this.ShowTrainerName(name);
            }));
        }
        /// <summary>
        /// Calls the show trainer photo.
        /// </summary>
        /// <param name="base64Photo">The base64 photo.</param>
        public void CallShowTrainerPhoto(string base64Photo)
        {
            base.Dispatcher.Invoke(new Action(delegate
            {
                this.ShowTrainerPhoto(base64Photo);
            }), new object[0]);
        }
    }
}
