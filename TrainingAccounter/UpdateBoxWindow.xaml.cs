using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace TrainingAccounter
{
    /// <summary>
    /// UpdateBoxWindow.xaml 的交互逻辑
    /// </summary>
    public delegate void ChangeHandler(string text); 

    public partial class UpdateBoxWindow : Window
    {
		DsRsrc dsrsrc = new DsRsrc();
		TrainingControl.ReadAndWrite readJStoSql = new ReadAndWrite();       
        string textPara = string.Empty;
        double dBalanceEnd = 0;
        double dThisTraBalance = 0;
        double dTotalBalance = 0;
        public UpdateBoxWindow(string sPidNo, string sName, Double dBalance, string sTraAmount, Double[] actrueBalance, int[] sBookSeqNo, string BillMode, double dBalance1)
        {
            InitializeComponent();
            if (dBalance > 0)
            {
                dBalanceEnd = dBalance;
            }
            else
            {
                dBalanceEnd = 0;
            }
            dThisTraBalance = actrueBalance[0];
            m_sPidNo = sPidNo;
            SeqNo = sBookSeqNo;
			dTotalBalance = dBalance1;// +dBalanceEnd;
            if (BillMode == "Time")
                this.lbTratime.Content = "(分钟)";           
            else if (BillMode == "Tries")            
                this.lbTratime.Content = "(次)";
            if (BillMode == "Mileage")
                this.lbTratime.Content = "(公里)";            
            this.tbxName.Text = sName;
            this.tbxPidNo.Text = sPidNo;
         //   this.tbxResuite.Text = dBalanceEnd.ToString();
            this.tbxBalance.Text = dTotalBalance.ToString();
            this.tbxActra.Text = sTraAmount;
            this.tbxAcBalance.Text = actrueBalance[0].ToString();
        }
        private static string m_sPidNo;
        private static int[] SeqNo = new int[2];
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

		private void btnUpd_Click(object sender, RoutedEventArgs e)
		{
			try
			{				
				var s = DriveInfo.GetDrives();
				bool isDisk = false;
				foreach (var drive in s)
				{
					if (drive.DriveType == DriveType.Removable)
					{
						isDisk = true;
						var readResult = readJStoSql.ReadTrainLicense(drive.Name.ToString());
						if (readResult != null)
						{
							for (int i = 0; i < readResult.Count; i++)
							{
								if (readResult[i].TraBookSeqNo.ToString().Trim() == SeqNo[0].ToString().Trim())
								{
									//删除U盘许可信息
									var uPath = drive.Name.ToString() + "TrainLicense.dat";
									var newUpath = drive.Name.ToString() + DateTime.Now.ToString("yyyy-MM-dd") + "-" + SeqNo[0] + "-TrainLicense.dat";
									if (File.Exists(uPath))
									{										
										//readResult[i].MileageLmt = 0;
										//readResult[i].TimeLmt = 0;
										//readResult[i].TriesLmt = 0;
										//readResult[i].TrainDetail = null;
										//var traLincense = JsonConvert.SerializeObject(readResult);
										//readJStoSql.CreateLogFileAndWriteLog(drive.Name.ToString(), traLincense, readJStoSql.StrWholeRead[1]);
										DBAccessProc.DBAccessHelper.UpdBalanceByPidNo(m_sPidNo, dTotalBalance, SeqNo);
										DBAccessProc.DBAccessHelper.UpdTraBookTraBalance(SeqNo[0].ToString(), dThisTraBalance);
										dsrsrc.getTraStudinfo(m_sPidNo);
										string endBalance = dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["BALANCE"].ToString();
										textPara = string.Format("身份证号码为：" + m_sPidNo + " 的学员结算完成，账户余额：" + endBalance + "元");
										if (dBalanceEnd > 0)
											DBAccessProc.DBAccessHelper.InsRechargeRecord(dBalanceEnd, m_sPidNo, "", "结算返还");

                                        File.Copy(uPath, newUpath, true);
                                        File.Delete(uPath);
									}
								}

							}
						}
						else
						{ 
							MessageBox.Show("没有读取到可用的许可信息，无法完成结算。");
							return;
						}
			 		}
			 }
				if (!isDisk)
				{
					MessageBox.Show("未检测到移动设备，请检查连接");
					return;
				}
				this.Close();
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show("更新结算信息到数据库失败，请重试。错误信息：" + ex.Message);
				return;
			}
		}
	
    }
}
