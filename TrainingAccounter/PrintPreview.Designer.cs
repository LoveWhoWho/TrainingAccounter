namespace TrainingAccounter
{
	partial class PrintPreview
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.wbReport = new System.Windows.Forms.WebBrowser();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnPrint = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// wbReport
			// 
			this.wbReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.wbReport.Location = new System.Drawing.Point(1, 37);
			this.wbReport.MinimumSize = new System.Drawing.Size(20, 20);
			this.wbReport.Name = "wbReport";
			this.wbReport.Size = new System.Drawing.Size(784, 777);
			this.wbReport.TabIndex = 0;
			// 
			// btnClose
			// 
			this.btnClose.BackColor = System.Drawing.SystemColors.Highlight;
			this.btnClose.Image = global::TrainingAccounter.Properties.Resources.Exit;
			this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnClose.Location = new System.Drawing.Point(124, 3);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(97, 28);
			this.btnClose.TabIndex = 4;
			this.btnClose.Text = "关闭(&C)";
			this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnClose.UseVisualStyleBackColor = false;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnPrint
			// 
			this.btnPrint.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.btnPrint.Image = global::TrainingAccounter.Properties.Resources.print;
			this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnPrint.Location = new System.Drawing.Point(12, 3);
			this.btnPrint.Name = "btnPrint";
			this.btnPrint.Size = new System.Drawing.Size(97, 28);
			this.btnPrint.TabIndex = 3;
			this.btnPrint.Text = "打印(&P)";
			this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnPrint.UseVisualStyleBackColor = false;
			this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
			// 
			// PrintPreview
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(786, 716);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnPrint);
			this.Controls.Add(this.wbReport);
			this.Name = "PrintPreview";
			this.Text = "PrintPreview";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.WebBrowser wbReport;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnPrint;
	}
}