using DBAccessProc;
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
using System.Windows.Forms;
using System.IO;
namespace TrainingAccounter
{
	/// <summary>
	/// serTraResult.xaml 的交互逻辑
	/// </summary>
	public partial class serTraResult : Page
	{
		public serTraResult(DsRsrc dsr)
		{
			InitializeComponent();
			dsrsrc = dsr;
			dsrsrc.getAllDrvSchool();
			dsrsrc.GetAllSetting();
			dsrsrc.GetAllTrainer();
			dsrsrc.getAllTraCar();
			CboxDrvName.ItemsSource = dsrsrc.trainMangeDataSet.DrvSchoolDataTable.DefaultView;	
			this.CboxSisCarType.ItemsSource = dsrsrc.TrainLicenseType;
			CboxSisCarType.SelectedIndex = 0;
	
			
		}
		DsRsrc dsrsrc;		
		string m_sFilePath = Common.PrintFilePath;//打印文件存放地址
		private void miGetDetailRecords_Click(object sender, RoutedEventArgs e)
		{
			if (dataGridSisSerch.SelectedItems.Count > 0)
			{
				var row = (DBAccessProc.Schema.TrainManagementDataSet.TraProcessInfoDataTableRow)(dataGridSisSerch.SelectedItems[0] as DataRowView).Row;
				if (row != null)
				{
					TrainDetailInfo detailForm = new TrainDetailInfo(row);
					detailForm.ShowDialog();
				}
				else
				{
					throw new InvalidCastException("无法将行转换为TrainManagementDataSet.TraProcessInfoDataTableRow!");
				}
			}
		}

		private void BtnExport_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (ExportExcel.ExportRecordToExcel("训练信息", Common.TempFilePath, dataGridSisSerch))
				{
					System.Windows.MessageBox.Show("导出已完成!");
				}
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show("导出失败，错误：" + ex.Message);
			}
		}
		private void BtnPreview_Click(object sender, RoutedEventArgs e)
		{
			string xslName = m_sFilePath + @"\xsl\SerchResult.xsl";
			if (File.Exists(xslName))
			{
				if (this.dataGridSisSerch.Items.Count == 0)
				{
					System.Windows.MessageBox.Show(@"列表中无可用数据");
					return;
				}
				string htmlName=dsrsrc.PrintDoc(xslName, m_sFilePath,0);
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
        

			//报表实现方法：先导出Excel然后读取显示
			//根据模板生成word文件
			//ViewWord vw = new ViewWord(dsrsrc);
			//vw.Show();
			//wordView vw = new wordView();
			//vw.ShowDialog();
			//rePortForm rpf = new rePortForm(dsrsrc, "TraProcessInfoDataTable", Environment.CurrentDirectory.ToString() + @"\Template\Report1.rdlc");
			//rpf.ShowDialog();
			////  报表打印
			//string reportPath = Environment.CurrentDirectory.ToString() + @"\Template\XLJGCX.rpt";
			//viewReport pw = new viewReport();
			//pw.Data_Binding(0, reportPath, "TraProcessInfoDataTable", dsrsrc.trainMangeDataSet);
			//pw.Show();
		}
		private void BtnSisSerch_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var m_sCarType = CboxSisCarType.SelectedItem != null ? CboxSisCarType.SelectedItem.ToString() : "";
				var m_sTrainer = CboxTrainer.Text.Trim();
				var m_sDsId = CboxDrvName.SelectedValue != null ? int.Parse(CboxDrvName.SelectedValue.ToString()) : -1;
				var m_sAutoId = CboxAutoId.Text.Trim();

				var item = CboxTraCD.SelectedItem as ComboBoxItem;
				dsrsrc.getTraProcessInfo(item != null && item.Content.ToString() == "科目二" ? "RE" : "SE", m_sCarType, m_sTrainer, m_sDsId, StrTime.SelectedDate.Value.ToString("yyyy-MM-dd"), EndTime.SelectedDate.Value.ToString("yyyy-MM-dd"), SisName.Text.Trim(), SisPidNo.Text.Trim(), m_sAutoId, 0);
				dataGridSisSerch.ItemsSource = dsrsrc.trainMangeDataSet.TraProcessInfoDataTable.DefaultView;
				//dsrsrc.trainMangeDataSet.TraProcessPointsDataTable.Clear();
				//if (dsrsrc.trainMangeDataSet.TraProcessInfoDataTable.Rows.Count > 0)
				//{
				//    foreach(DataRow tempData in dsrsrc.trainMangeDataSet.TraProcessInfoDataTable.Rows)
				//    {
				//        dsrsrc.getTraProcessInfoPoints(int.Parse(tempData["SEQ_NO"].ToString()));
				//    }
				//}
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show("查询训练详细信息出错，错误信息:" + ex.Message + "");
			}
		}

		private void CboxSisCarType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
				if (this.CboxSisCarType.SelectedValue == null)
					return;
				else
				{
					try
					{
						//获取改变后的值					
						string strCarType = CboxSisCarType.SelectedValue.ToString();
						DataRow[] drs = dsrsrc.trainMangeDataSet.TrainerDataTable.Select("AllowableLicenseType = '" + strCarType + "'");
						if (drs.Length > 0)
						{
							DataTable db = dsrsrc.trainMangeDataSet.TrainerDataTable.Clone();
							foreach (DataRow item in drs)
							{
								db.Rows.Add(item.ItemArray);
							}
							this.CboxTrainer.ItemsSource = db.DefaultView;
						}
						DataRow[] drv = dsrsrc.trainMangeDataSet.TraCarDataTable.Select("AUTO_TYPE_CD = '" + strCarType + "'");
						if (drv.Length > 0)
						{
							DataTable db = dsrsrc.trainMangeDataSet.TraCarDataTable.Clone();
							foreach (DataRow item in drv)
							{
								db.Rows.Add(item.ItemArray);
							}
							this.CboxAutoId.ItemsSource = db.DefaultView;
						}					
					}
					catch (Exception ex)
					{
						System.Windows.Forms.MessageBox.Show(ex.ToString());
						return;
					}
				}
		}
	

		
	}
}
