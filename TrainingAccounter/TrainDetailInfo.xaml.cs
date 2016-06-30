using System;
using System.Windows;
using DBAccessProc.Schema;
using TrainingControl;
using DBAccessProc;
using System.Collections.Generic;
using System.IO;   //负责解释ReportDocument类

namespace TrainingAccounter
{
    /// <summary>
    /// TrainDetailInfo.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class TrainDetailInfo : Window
    {
		DsRsrc dsrsrc = new DsRsrc();
        TrainManagementDataSet.TraProcessInfoDataTableRow _row;
        public TrainDetailInfo(TrainManagementDataSet.TraProcessInfoDataTableRow row)
        {
            InitializeComponent();
            _row = row;
        }
		string m_sFilePath = Common.PrintFilePath;//打印文件存放地址
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
			dsrsrc.trainMangeDataSet.TraProcessPointsDataTable.Clear();
			DBAccessProc.DBAccessHelper.GetTraProcessPoints(_row.SEQ_NO, dsrsrc.trainMangeDataSet);
			dgDetailRecords.ItemsSource = dsrsrc.trainMangeDataSet.TraProcessPointsDataTable;
            txtAutoId.Text = _row.AUTO_ID;
            txtAutoType.Text = _row.AUTO_TYPE_CD;
            txtDate.Text = _row.TRAIN_DT;
            txtDrivingSchool.Text = _row.DS_NAME;
            txtEndTime.Text = _row.TRAIN_END_TS.ToString("yyyy-MM-dd HH:mm:ss");
            txtExamCd.Text = _row.EXAM_CD;
            txtName.Text = _row.TRAINE_NAME;
            txtPidNo.Text = _row.PID_NO;
            txtSeqNo.Text = _row.SEQ_NO.ToString();
            txtStartTime.Text = _row.TRAIN_START_TS.ToString("yyyy-MM-dd HH:mm:ss");
            txtTrainer.Text = _row.TRAINER_NAME;
            txtTrainMileage.Text = _row.TRAIN_MILEAGE.ToString();
            txtTrainTime.Text = _row.TRAINING_TIME.ToString();
            txtTrainTries.Text = _row.TRAIN_TRIES.ToString();           
            txtTrainMode.Text = _row.BILL_MODEL.ToString();
        }      
      
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ExportExcel.ExportDetailRecordToExcel(txtPidNo.Text, Common.TempFilePath, dgDetailRecords))
                {
                    MessageBox.Show("导出成功!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败，错误：" + ex.Message);
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
			string xslName = m_sFilePath + @"\xsl\SerchResultDetail.xsl";
			if (File.Exists(xslName))
			{
				if (this.dgDetailRecords.Items.Count == 0)
				{
					System.Windows.MessageBox.Show(@"列表中无可用数据");
					return;
				}
			string htmlName=	dsrsrc.PrintDoc(xslName, m_sFilePath,1);
			if (htmlName != "")
			{
				PrintPreview pr = new PrintPreview(htmlName);
				pr.Show();
			}
			else
			{
				System.Windows.MessageBox.Show("找不到生成的HTML文件。");
				return;
			}
			}
			else
			{
				System.Windows.MessageBox.Show("找不到格式文件。");
				return;
			}          
           
        }
    }
  
}
