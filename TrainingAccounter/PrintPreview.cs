using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TrainingAccounter
{
	public partial class PrintPreview : Form
	{
		public PrintPreview(string htmlReportName)
		{
			InitializeComponent();
			this.wbReport.Navigate(htmlReportName);          
		}
		public void PrintDoc()
		{
			this.wbReport.Print();
		}

		private void btnPrint_Click(object sender, EventArgs e)
		{
			PrintDoc();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	
	}
}
