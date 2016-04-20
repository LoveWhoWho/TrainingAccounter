using DBAccessProc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TrainingControl;
using WPF.DazzleUI2.Controls;

namespace TrainingAccounter
{
    /// <summary>
    /// AddTrainee.xaml 的交互逻辑
    /// </summary>
	public partial class AddTrainee : Window
    {
		public AddTrainee(DsRsrc dsrsrc)
        {
            InitializeComponent();          
			addDsrsrc = dsrsrc;
			this.txbPidNo.SetBinding(TextBox.TextProperty, new Binding("PidNo") { Source = addDsrsrc });
			txbUserName.SetBinding(TextBox.TextProperty, new Binding("Name") { Source = addDsrsrc });
			txbAddress.SetBinding(TextBox.TextProperty, new Binding("Address") {Source=addDsrsrc });
			myImage.SetBinding(Image.SourceProperty, new Binding("Photo") { Source=addDsrsrc});
			txbPidNo.Text = "";
			txbUserName.Text = "";
			txbPhoneNo.Text = "";
			txbAddress.Text = "";
			bindSource();		
        }
		DsRsrc addDsrsrc;
        AxLib.AxLibControl axLibCtl = null;
        string sFingerPrint = string.Empty;
		
        //public delegate void ChangeTextHandler(string text);
        //public event ChangeTextHandler ChangeTextEvent;
        public string _pidNo { get; set; }
        public string _name { get; set; }
        public byte[] _photo { get; set; }
        public string _address { get; set; }
        public string _sFingerPrint { get; set; }

        private void txbPidNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbPidNo.Text.Trim().Length == 15 || txbPidNo.Text.Trim().Length == 18)
            {
                string identityCard = txbPidNo.Text.Trim();
                string birthday = "";
                string sex = "";

                //处理18位的身份证号码从号码中得到生日和性别代码
                if (identityCard.Length == 18)
                {
                    birthday = identityCard.Substring(6, 4) + "-" + identityCard.Substring(10, 2) + "-" + identityCard.Substring(12, 2);
                    sex = identityCard.Substring(14, 3);
                }
                //处理15位的身份证号码从号码中得到生日和性别代码
                if (identityCard.Length == 15)
                {
                    birthday = "19" + identityCard.Substring(6, 2) + "-" + identityCard.Substring(8, 2) + "-" + identityCard.Substring(10, 2);
                    sex = identityCard.Substring(12, 3);
                }
                dpBirthDay.Text = birthday;
                //性别代码为偶数是女性奇数为男性
                if (int.Parse(sex) % 2 == 0)
                {
                    this.rdbWoman.IsChecked = true;
                }
                else
                {
                    this.rdbMan.IsChecked = true;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {			
			int userType = 0;
            if (txbPidNo.Text == "" || txbUserName.Text == "" || cmbLicenseTypeCd.SelectedValue == null)
            {
                System.Windows.MessageBox.Show("带*项不能为空，请检查后重新保存");
                return;
            }
            byte[] UserPhoto=null;		
            try
            {
                if(_photo==null)
                {
                     BitmapImage bmp = myImage.Source as BitmapImage;
                if (bmp != null)
                {
					UserPhoto = addDsrsrc.PhotoSave(bmp);
				}
					//用户类型			
				if (this.rdbVip.IsChecked == true)
					userType = 1;
                }
				DBAccessHelper.SaveTraStudInfo(txbPidNo.Text.Trim(), txbUserName.Text.Trim(), UserPhoto, this.rdbMan.IsChecked == true ? "男" : "女", this.dpBirthDay.SelectedDate.Value, _sFingerPrint, cmbDrivingSchool.SelectedValue == null ? -1 : Convert.ToInt32(cmbDrivingSchool.SelectedValue.ToString()), this.txbPhoneNo.Text.Trim(), ".", dpRegDate.SelectedDate.Value.ToString("yyyy-MM-dd"), "", "", cmbLicenseTypeCd.SelectedValue.ToString(), "", "", "", "", Convert.ToDouble("0"), txbAddress.Text.Trim(), userType);
                addDsrsrc.MainBarText = "学员：" + txbUserName.Text.Trim() + ",注册信息成功，可以进行预约";
                tblSaveSta.Text = "保存成功";
                txbPidNo.Text = "";
                txbUserName.Text = "";
                txbPhoneNo.Text = "";
                txbAddress.Text = "";               
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("保存学员信息失败，错误信息：" + ex.Message.ToString() + "");
                tblSaveSta.Text = "保存失败";
                return;
            }
          
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
		//protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		//{
		//	this.Hide();
		//	e.Cancel = true;
		//}
        private void btnEnrollFingerprint_Click(object sender, RoutedEventArgs e)
        {
            if (txbPidNo.Text.Trim() == "")
            {
                System.Windows.MessageBox.Show("身份证不能为空！");
                return;
            }
            sFingerPrint = axLibCtl.EnrollFingerprint();
            if (sFingerPrint == "")
            {
                System.Windows.MessageBox.Show("请重新提取指纹！");
                return;
            }


			addDsrsrc.getTraStudinfo(this.txbPidNo.Text);
			if (addDsrsrc.trainMangeDataSet.TraineeDataTable.Count > 0)
            {
                //保存指纹
                DBAccessProc.DBAccessHelper.SaveTraineeFingerPrint(sFingerPrint, this.txbPidNo.Text);
                System.Windows.MessageBox.Show("指纹保存成功！");
            }
            if (!string.IsNullOrEmpty(sFingerPrint))
            {
                //  lblFingerPrint.Visibility = Visibility.Visible;
                lblFingerPrint1.Visibility = Visibility.Hidden;
            }
            else
            {
                //   lblFingerPrint.Visibility = Visibility.Hidden;
                lblFingerPrint1.Visibility = Visibility.Visible;
            }
        }    
		private void bindSource ()
		{
			//绑定驾校和车型    
		
				addDsrsrc.getAllDrvSchool();
				this.cmbDrivingSchool.ItemsSource = addDsrsrc.trainMangeDataSet.DrvSchoolDataTable.DefaultView;		
				addDsrsrc.GetAllSetting();
				this.cmbLicenseTypeCd.ItemsSource = addDsrsrc.TrainLicenseType;		
				cmbLicenseTypeCd.SelectedIndex = 0;
		}

		private void btnPhotoUser_Click(object sender, RoutedEventArgs e)
		{
			var photoSource = addDsrsrc.getPhotoFromCamra();
			if (photoSource != null)
				addDsrsrc.ShowImageFromBuffer(photoSource, this.myImage);
		}
	

    }
}
