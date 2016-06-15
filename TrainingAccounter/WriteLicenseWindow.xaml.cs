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
using System.Management;
using System.Data;
using System.IO;
using TrainingControl;
using Newtonsoft.Json;

namespace TrainingAccounter
{
    /// <summary>
    /// WriteLisenceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WriteLicenseWindow : Window
    {
		DsRsrc dsrsrc = new DsRsrc();
        ReadAndWrite readAndWrite = new ReadAndWrite();
        MainWindow mainWin = new MainWindow();
        private string m_sPath;
        DataTable dt = new DataTable();
        string sJsonTraLic = "";
        Dictionary<string,string> sSeqNo;
        private List<string> _serialNo = new List<string>();
        string sTraLicUid = "";

        public WriteLicenseWindow(string m_sJsonTraLic,Dictionary<string,string> sBookSeqNo)
        {
            InitializeComponent();
            sJsonTraLic = m_sJsonTraLic;
            sSeqNo = sBookSeqNo;
            dt = readAndWrite.ReadDisk();
            dataGridWriteLicense.Items.Clear();
            dataGridWriteLicense.ItemsSource = dt.DefaultView;
        }
        
        
        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
			try{
				bool isExistence = false;
            DataRowView mySelectedElement = (DataRowView)dataGridWriteLicense.SelectedItem;
            m_sPath = mySelectedElement.Row[0].ToString();
            _serialNo = readAndWrite.matchDriveLetterWithSerial(m_sPath);
            sTraLicUid = _serialNo[0].ToString();
			string pidNo = string.Empty;
			string seqNo = string.Empty;
            foreach (var item in sSeqNo.Keys)
			{
				try
				{
					double balance=0;
				    var listContent= JsonConvert.DeserializeObject<List<TrainLicense>>(sJsonTraLic);
					foreach (var listitem in listContent)
					{
						if(listitem.TraBookSeqNo.ToString()==sSeqNo[item].ToString())
						{ 
								balance=listitem.AccountBalance;
								pidNo = item;
								seqNo = sSeqNo[item];
								isExistence = true;
						}
					}
					if (!isExistence)
					{ 
						MessageBox.Show("不存在需要的信息,无法进行签到");
						return;
					}
					if (dsrsrc.TraCheckIn(sSeqNo[item], "CK", item, sTraLicUid, balance) != 1)
					{
						MessageBox.Show("写入许可失败。请重新写入。");
						return;
					}
                    dsrsrc.MainBarText = string.Format("学员:" + pidNo + "  签到成功，已经向移动设备中写入许可");
				}
				catch (Exception exm)
				{
					MessageBox.Show("更新签到信息出错，错误信息：" + exm.Message.ToString());
				}
			}
			readAndWrite.CreateLogFileAndWriteLog(m_sPath, sJsonTraLic, sTraLicUid);
				//更新主账户余额为0
			dsrsrc.RechargeAccount(seqNo, 0, pidNo,"签到", "清空账户余额");
            MessageBox.Show("签到成功！");
            mySelectedElement.Delete();
            this.Close();}
			catch(Exception ex)
			{
				MessageBox.Show("签到出错，错误信息："+ex.Message.ToString());
			}
        }
    }
}
