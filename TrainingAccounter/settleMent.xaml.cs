using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrainingControl;

namespace TrainingAccounter
{
	/// <summary>
	/// settleMent.xaml 的交互逻辑
	/// </summary>
	public partial class settleMent : Page
	{
		public settleMent(DsRsrc dsr)
		{
			InitializeComponent();
			dsrsrc = dsr;
			dsrsrc.getAllDrvSchool();
			cbxDevName.ItemsSource = dsrsrc.trainMangeDataSet.DrvSchoolDataTable.DefaultView;
			tbxSerPidNo.SetBinding(TextBox.TextProperty, new Binding("PidNo") { Source = dsrsrc });
		}
		DsRsrc dsrsrc;
		DateTime m_StarTime;
		DateTime m_EndTime;
		private void dataGridSettlement_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (dataGridSettlement.SelectedItems.Count == 1)
			{
				var temp = (DataRowView)dataGridSettlement.SelectedItems[0];
				tbxSerPidNo.Text = temp["PID_NO"].ToString();				
			}
		}
		private void datePickerSerStr_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			m_StarTime = this.datePickerSerStr.SelectedDate.Value;
		}

		private void datePickerSerEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			m_EndTime = this.datePickerSerEnd.SelectedDate.Value;
		}
		private void btnSearch_Click(object sender, RoutedEventArgs e)
		{
				if (tbxSerPidNo.Text.ToString() == "" && datePickerSerStr.SelectedDate == null && datePickerSerEnd.SelectedDate == null)
				{
					System.Windows.Forms.MessageBox.Show("查询条件不能全部为空，请重新填写");
					return;
				}	
			 dsrsrc.GetTraBook(tbxSerPidNo.Text.ToString().Trim(), "", m_StarTime, m_EndTime);        
			if (dsrsrc.trainMangeDataSet.BookTrainingDataTable.Rows.Count > 0)
                {
					DataTable dt = dsrsrc.trainMangeDataSet.BookTrainingDataTable.Clone();
					DataRow[] drow = dsrsrc.trainMangeDataSet.BookTrainingDataTable.Select(null, "SEQ_NO ASC");
                    foreach (DataRow dr in drow)
                    {
                        switch (dr["CHECKSTATUS"].ToString())
                        {
                            case "UK":
                                dr["CHECKSTATUS"] = "未签到";
                                break;
                            case "CK":
                                dr["CHECKSTATUS"] = "已签到";
                                break;
                            case "IK":
                                dr["CHECKSTATUS"] = "训练中";
                                break;
                            case "EK":
                                dr["CHECKSTATUS"] = "已结束";
                                break;
                            case "SK":
                                dr["CHECKSTATUS"] = "已结算";
                                break;
                            case "RK":
                                dr["CHECKSTATUS"] = "已取消";
                                break;
                        }
                    switch (dr["BILL_MODE"].ToString())
                    {
                        case "Time":
                            dr["RECHARGE_AMOUNT"] = dr["RECHARGE_AMOUNT"] + " 小时";
                            break;
                        case "Tries":
                            dr["RECHARGE_AMOUNT"] = dr["TRIES_AMOUNT"] + " 次";
                            break;
                        case "Mileage":
                            dr["RECHARGE_AMOUNT"] = dr["MILEAGE_AMOUNT"] + " 公里";
                            break;
                        case "StudyTime":
                            dr["RECHARGE_AMOUNT"] = dr["RECHARGE_AMOUNT"] + " 个学时";
                            break;
                    }
                        dt.Rows.Add(dr.ItemArray);
                    }
                    dataGridSettlement.ItemsSource = dt.DefaultView;
                }
            else
                dataGridSettlement.ItemsSource = null;
            dsrsrc.MainBarText = "共查询到：" + dsrsrc.trainMangeDataSet.BookTrainingDataTable.Rows.Count + "条数据";
        
			}
		private void btnSetComplete_Click(object sender, RoutedEventArgs e)
		{
			//设置不能结算的条件     
			bool isSettlement = false;//标志是否可以结算
			Double ActualTimes = 0;//实际消耗时间
			double dBlance = 0;//余额
			int ActualTries = 0;//实际训练次数
			double ActualMileage = 0;//实际训练里程
			Double ActualBalance = 0;//实际可结算的余额
			string sTraAmount = "";// 训练量
			Double[] dActualTimes = new Double[2];
			Double[] dActualBalance = new Double[2];
			int[] sBookSeqNo = new int[2];
			int count = this.dataGridSettlement.SelectedItems.Count;
			TrainingControl.ReadAndWrite readJStoSql = new ReadAndWrite();
			if (this.dataGridSettlement.SelectedItems.Count <= 0)
			{
				System.Windows.Forms.MessageBox.Show("没有选择任何要结算的预约，请重新选择。");
				return;
			}
			else if (count == 1)
			{
				var tempB = (DataRowView)dataGridSettlement.SelectedItems[0];
				dsrsrc.getTraStudinfo(tempB["PID_NO"].ToString());
				if (dsrsrc.trainMangeDataSet.TraineeDataTable.Rows.Count <= 0)
					return;
				switch (tempB["CHECKSTATUS"].ToString())
				{
					case "未签到":
						if (System.Windows.Forms.MessageBox.Show("本次预约尚未签到。是否进行结算？", "提示信息：", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
						{
							isSettlement = true;
							dBlance = Double.Parse(dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["BALANCE"].ToString());
						}
						else
							return;
						break;
					case "已签到":
						System.Windows.Forms.MessageBox.Show("已经签到，但未读取到训练数据，无法进行结算。");
						return;
					case "训练中":
						//检测是否有U盘插入
						var s = DriveInfo.GetDrives();
						bool isDisk = false;
						foreach (var drive in s)
						{
							if (drive.DriveType == DriveType.Removable)
							{
								isDisk = true;
								var row = dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0];
								if (readJStoSql.CompareDisk(drive.Name.ToString(), row["PID_NO"].ToString(), row["DISK_ID"].ToString()))
									isSettlement = true;
								else
								{
									MessageBox.Show("当前插入的U盘不是此学员对应的U盘，请插入正确的U盘");
									return;
								}
							}
						}
						if (isDisk == false)
						{
							MessageBox.Show("没有检测到移动设备，无法进行结算。");
							return;
						}
						break;
					case "已结束":
						isSettlement = true;
						break;
					case "已结算":
						System.Windows.Forms.MessageBox.Show("本次预约已经结算完成，无法再次结算。");
						return;
				}
				if (isSettlement == true)
				{

					dsrsrc.getTraProcessInfo("", "", "", 0, "1900-01-01", "1900-01-01", "", tempB["PID_NO"].ToString(), "", "", "", "", int.Parse(tempB["SEQ_NO"].ToString()));
					if (dsrsrc.trainMangeDataSet.TraProcessInfoDataTable.Rows.Count > 0)
					{
						ActualTimes = double.Parse(dsrsrc.trainMangeDataSet.TraProcessInfoDataTable.Rows[0]["TRAINING_TIME"].ToString());
						ActualTries = int.Parse(dsrsrc.trainMangeDataSet.TraProcessInfoDataTable.Rows[0]["TRAIN_TRIES"].ToString());
						ActualMileage = double.Parse(dsrsrc.trainMangeDataSet.TraProcessInfoDataTable.Rows[0]["TRAIN_MILEAGE"].ToString());
					}
					dActualTimes[0] = ActualTimes;
					sBookSeqNo[0] = int.Parse(tempB["SEQ_NO"].ToString());
					string sBillMode = tempB["BILL_MODE"].ToString();
					double dChargingStandard = 0;
					DataRow[] drow = dsrsrc.trainMangeDataSet.BookTrainingDataTable.Select("SEQ_NO = '" + sBookSeqNo[0] + "'");
					dChargingStandard = dsrsrc.GetChargingStandard(dsrsrc.sBillingMode, dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["LICENSE_TYPE_CD"].ToString());
                    double dBookBalance = double.Parse(drow[0]["THIS_BALANCE"].ToString());
                    var rowStu = dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0];
                    bool isVip = (int.Parse(rowStu["STUDENT_TYPE"].ToString()) == 1) ? true : false;
                    if (tempB["CHECKSTATUS"].ToString() != "未签到")
                    {
                        if (isVip)
                        {
                                dBlance = Math.Round(Double.Parse(dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["UNDER_BALANCE"].ToString()) +
                                Double.Parse(dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["BALANCE"].ToString()), 2);                              
                                ActualBalance = Math.Round(dBookBalance > dActualBalance[0] ? (dBookBalance - dActualBalance[0]) : 0);
                        }                                         
                        else 
                        {
                                dBlance = Double.Parse(dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["BALANCE"].ToString());
                                ActualBalance = dBookBalance;
                        }

                    }
                    if (sBillMode == "Time" || sBillMode == "StudyTime")
                    {
                        if (isVip)
                        {
                            if (sBillMode == "StudyTime")
                                dActualBalance[0] = Math.Round(ActualTimes * dChargingStandard/Math.Round(double.Parse(dsrsrc.StudyHasTime),2), 2);
                            else
                                dActualBalance[0] = Math.Round(ActualTimes * Math.Round((dChargingStandard / 60), 2), 2);
                            sTraAmount = ActualTimes.ToString();
                        }
                        else {

                            dActualBalance[0] = dBookBalance;
                            sTraAmount = drow[0]["RECHARGE_AMOUNT"].ToString();
                        }
					}
					else if (sBillMode == "Tries")
					{
                        if (isVip)
                        {
                            dActualBalance[0] = Math.Round(ActualTries * dChargingStandard, 2);
                            sTraAmount = ActualTries.ToString();
                        }
                        else
                        {
                            dActualBalance[0] = dBookBalance;
                            sTraAmount = drow[0]["TRIES_AMOUNT"].ToString();
                        }
					}
                    else if (sBillMode == "Mileage")
                    {
                        if (isVip)
                        {
                            dActualBalance[0] = Math.Round(ActualMileage * dChargingStandard, 2);
                            sTraAmount = ActualMileage.ToString();
                        }
                        else
                        { 
                             dActualBalance[0] = dBookBalance;
                             sTraAmount = drow[0]["MILEAGE_AMOUNT"].ToString();
                        }
                    }
                 
					UpdateBoxWindow ubw = new UpdateBoxWindow(tempB["PID_NO"].ToString(), tempB["TRAINE_NAME"].ToString(), ActualBalance, sTraAmount, dActualBalance, sBookSeqNo, sBillMode, dBlance);				
					ubw.ShowDialog();
                }
				}
			}
		}
	
		
	}


