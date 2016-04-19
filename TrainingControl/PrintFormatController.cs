using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Xsl;
using DBAccessProc;
using TrainingControl;

namespace TrainingAccounter
{
    public class PrintFormatController
    {
		public DBAccessProc.Schema.TrainManagementDataSet m_ReExamResultDs = null;
        XslCompiledTransform transfromResult = new XslCompiledTransform();
        private string m_sFilePath;   
        public PrintFormatController(string filePath, ref DBAccessProc.Schema.TrainManagementDataSet dataset)
        {
             try
            {
                m_sFilePath = filePath + "\\" + DateTime.Now.ToString("yyyyMMdd");
				if (!Directory.Exists(m_sFilePath))
					Directory.CreateDirectory(m_sFilePath);
                m_ReExamResultDs = dataset;
                m_ReExamResultDs.Namespace = "";             
            }
            catch (Exception ex)
            {
               System.Windows.MessageBox.Show ("初始化PrintFormatController时失败，错误：" + ex.Message + ",位置：" + ex.StackTrace);
            }
        }

        private static void PrintDocument(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ((WebBrowser)sender).Print();
            ((WebBrowser)sender).Dispose();
        }

        public static bool PrintHtmlFile(string htmlFileName)
        {
            FileInfo fileHtml = new FileInfo(htmlFileName);
            if (fileHtml.Exists)
            {
                WebBrowser printBrowser = new WebBrowser();
                printBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(PrintDocument);
                printBrowser.Url = new Uri(htmlFileName);
                return true;
            }
            else
                return false;

            //delete digisign temp file;
            //if (Directory.Exists(DigitalSignatureHelper.sDigiSignTmpPath))
            //    Directory.Delete(DigitalSignatureHelper.sDigiSignTmpPath, true);
        }

        public static void ShowErrorMessage(string msg)
        {
            MessageBox.Show(msg, "错误",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowWarningMessage(string msg)
        {
            MessageBox.Show(msg, "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

       

        private string CreateXmlDataFile(int id)
        {
            string xmlFileName = "";
            try
			{
				if (id == 0) { xmlFileName = m_sFilePath + @"\" + DateTime.Now.ToString("yyyy-MM-dd-hh") + ".xml"; }
				else { xmlFileName = m_sFilePath + @"\" + DateTime.Now.ToString("yyyy-MM-dd-hh") + "detail.xml"; }
                 	
                m_ReExamResultDs.WriteXml(xmlFileName, XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("创建XML数据文件 " + xmlFileName + " 错误:" + ex.Message);
            }
            return xmlFileName;
        }

        private static void CreateHtmlFile(string cmd, string strParam)
        {
            Process batchExecute = new Process();
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(cmd);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = strParam;
			//	startInfo.UseShellExecute = false;
                batchExecute = new Process();
                batchExecute.StartInfo = startInfo;
		
				batchExecute.Start();
                batchExecute.WaitForExit();

                batchExecute = null;
            }
            catch (Exception ex)
            {
                batchExecute = null;
                throw new ApplicationException("xsl创建Html文件 " + strParam + " 错误:" + ex.Message);
            }
        }


        public string GePrtKm2ExamResultHtml(string sXslName, string sTitle,int id)
        {
            string sHtmlFileName = "";
            try
            {              
                string sXmlFileName = CreateXmlDataFile(id);
                //creaet Html
				if (id == 0) { sHtmlFileName = m_sFilePath + @"\" + DateTime.Now.ToString("yyyy-MM-dd-hh") + ".html"; }
				else { sHtmlFileName = m_sFilePath + @"\" + DateTime.Now.ToString("yyyy-MM-dd-hh") + "detail.html"; }      
                string strParam = sXmlFileName + " " + sXslName + " -o " + sHtmlFileName;
                CreateHtmlFile("msxsl", strParam);
                FileInfo fileHtml = new FileInfo(sHtmlFileName);
                if (fileHtml.Exists)
                    return sHtmlFileName;
                else
                    return "";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("创建统计打印文件 " + sHtmlFileName + "失败！错误信息：" + ex.Message);
            }
        }

      
    }
}
