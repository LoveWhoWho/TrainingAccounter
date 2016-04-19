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
	public partial class wordView : Form
	{
		public wordView()
		{
			InitializeComponent();	
			this.axFramerControl1.Open(@"F:\机动车驾驶人远程考试系统接口说明.doc");
		}

		private void btnCreate_Click(object sender, EventArgs e)
		{
			this.axFramerControl1.CreateNew("Word.Document");
			//this.axFramerControl1.CreateNew("Excel.Sheet");
		}

		private void btnPrint_Click(object sender, EventArgs e)		{
			
			this.axFramerControl1.PrintOut();
		}

		private void btnPrintView_Click(object sender, EventArgs e)
		{
			this.axFramerControl1.PrintPreview();
		}
	}
}
