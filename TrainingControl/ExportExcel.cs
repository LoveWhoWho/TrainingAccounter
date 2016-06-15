using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Controls;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using DBAccessProc.Schema;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
namespace TrainingControl
{
    public class ExportExcel
    {
        public static bool ExportRecordToExcel(string fileName,string filePath,DataGrid grid)
        {
            try
            {
                IWorkbook hssfworkbook = new HSSFWorkbook();
                ISheet sheet = hssfworkbook.CreateSheet();
                IRow row1 = sheet.CreateRow(0);
                if (grid.Columns.Count>0)
                {
                    for (int i = 0; i < grid.Columns.Count; i++)
                    {
                        sheet.GetRow(0).CreateCell(i).SetCellValue(grid.Columns[i].Header.ToString());
                        sheet.SetColumnWidth(i, (int)grid.Columns[i].Width.Value * 45);
                        ICell cell = sheet.GetRow(0).GetCell(i);
                        IFont font = hssfworkbook.CreateFont();
                        font.FontName = "宋体";
                        font.FontHeightInPoints = 12;
                        ICellStyle cellStyle = hssfworkbook.CreateCellStyle();
                        cellStyle.SetFont(font);
                        cellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle = cellStyle;
                    }

                    for (int j = 0; j < grid.Items.Count; j++)
                    {
                       
                        sheet.CreateRow(j + 1);
                        var row = (TrainManagementDataSet.TraProcessInfoDataTableRow)(grid.Items[j] as DataRowView).Row;
                        sheet.GetRow(j + 1).CreateCell(0).SetCellValue(row.SEQ_NO);
                        sheet.GetRow(j + 1).CreateCell(1).SetCellValue(row.AUTO_ID);
                        sheet.GetRow(j + 1).CreateCell(2).SetCellValue(row.TRAINE_NAME);
                        sheet.GetRow(j + 1).CreateCell(3).SetCellValue(row.PID_NO);
                        sheet.GetRow(j + 1).CreateCell(4).SetCellValue(row.TRAIN_DT);
                        sheet.GetRow(j + 1).CreateCell(5).SetCellValue(row.TRAINER_NAME);
                        sheet.GetRow(j + 1).CreateCell(6).SetCellValue(row.TRAINING_TIME);
                        sheet.GetRow(j + 1).CreateCell(7).SetCellValue(row.TRAIN_MILEAGE);
                        sheet.GetRow(j + 1).CreateCell(8).SetCellValue(row.TRAIN_TRIES);
                        sheet.GetRow(j + 1).CreateCell(9).SetCellValue(row.DS_NAME);
                        sheet.GetRow(j + 1).CreateCell(10).SetCellValue(row.TRABOOK_SEQ_NO);
                        for (int i = 0; i < 11; i++)
                        {
                            ICell cell = sheet.GetRow(j + 1).GetCell(i);
                            IFont font = hssfworkbook.CreateFont();
                            font.FontName = "宋体";
                            font.FontHeightInPoints = 12;
                            ICellStyle cellStyle = hssfworkbook.CreateCellStyle();
                            cellStyle.SetFont(font);
                            cellStyle.Alignment = HorizontalAlignment.Center;
                            cell.CellStyle = cellStyle;
                        }
                    }
                }
                using (FileStream file = new FileStream(filePath + "\\" + fileName + ".xls", FileMode.Create))
                {
                    hssfworkbook.Write(file);
                    file.Close();
                    hssfworkbook = null;
                    sheet = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("无法创建文件：" + ex.Message);
            }
        }

        public static bool ExportDetailRecordToExcel(string fileName,string filePath,DataGrid grid)
        {
            try
            {
                IWorkbook hssfworkbook = new HSSFWorkbook();
                ISheet sheet = hssfworkbook.CreateSheet();
                IRow row1 = sheet.CreateRow(0);
                if (grid.Columns.Count > 0)
                {
                    for (int i = 0; i < grid.Columns.Count; i++)
                    {
                        sheet.GetRow(0).CreateCell(i).SetCellValue(grid.Columns[i].Header.ToString());
                        sheet.SetColumnWidth(i, (int)grid.Columns[i].Width.Value * 45);
                        ICell cell = sheet.GetRow(0).GetCell(i);
                        IFont font = hssfworkbook.CreateFont();
                        font.FontName = "宋体";
                        font.FontHeightInPoints = 12;
                        ICellStyle cellStyle = hssfworkbook.CreateCellStyle();
                        cellStyle.SetFont(font);
                        cellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle = cellStyle;
                    }

                    for (int j = 0; j < grid.Items.Count; j++)
                    {

                        sheet.CreateRow(j + 1);
                        var row = (TrainManagementDataSet.TraProcessPointsDataTableRow)(grid.Items[j] as DataRowView).Row;
                        sheet.GetRow(j + 1).CreateCell(0).SetCellValue(row.SEQ_NO);
                        sheet.GetRow(j + 1).CreateCell(1).SetCellValue(row.EI_CODE);
                        sheet.GetRow(j + 1).CreateCell(2).SetCellValue(row.EI_DESC);
                        sheet.GetRow(j + 1).CreateCell(3).SetCellValue(row.VL_DESC);
                        sheet.GetRow(j + 1).CreateCell(4).SetCellValue(row.VL_POINTS);
                        sheet.GetRow(j + 1).CreateCell(5).SetCellValue(row.TRA_TYPE);
                        sheet.GetRow(j + 1).CreateCell(6).SetCellValue(row.EI_TS_TAMP.ToString("yyyy-MM-dd HH:mm:ss"));
                        sheet.GetRow(j + 1).CreateCell(7).SetCellValue(row.TRA_MODE);
                        for (int i = 0; i < 8; i++)
                        {
                            ICell cell = sheet.GetRow(j + 1).GetCell(i);
                            IFont font = hssfworkbook.CreateFont();
                            font.FontName = "宋体";
                            font.FontHeightInPoints = 12;
                            ICellStyle cellStyle = hssfworkbook.CreateCellStyle();
                            cellStyle.SetFont(font);
                            cellStyle.Alignment = HorizontalAlignment.Center;
                            cell.CellStyle = cellStyle;
                        }

                        
                    }
                }
                using (FileStream file = new FileStream(filePath + "\\" + fileName + ".xls", FileMode.Create))
                {
                    hssfworkbook.Write(file);
                    file.Close();
                    hssfworkbook = null;
                    sheet = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("无法创建文件：" + ex.Message);
            }
        }
    }
}
