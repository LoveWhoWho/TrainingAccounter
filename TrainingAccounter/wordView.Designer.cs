namespace TrainingAccounter
{
	partial class wordView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(wordView));
			this.axFramerControl1 = new AxDSOFramer.AxFramerControl();
			this.btnPrint = new System.Windows.Forms.Button();
			this.btnPrintView = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.axFramerControl1)).BeginInit();
			this.SuspendLayout();
			// 
			// axFramerControl1
			// 
			this.axFramerControl1.AllowDrop = true;
			this.axFramerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.axFramerControl1.Enabled = true;
			this.axFramerControl1.Location = new System.Drawing.Point(0, 0);
			this.axFramerControl1.Name = "axFramerControl1";
			this.axFramerControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axFramerControl1.OcxState")));
			this.axFramerControl1.Size = new System.Drawing.Size(884, 733);
			this.axFramerControl1.TabIndex = 0;
			// 
			// btnPrint
			// 
			this.btnPrint.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.btnPrint.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnPrint.Location = new System.Drawing.Point(59, 4);
			this.btnPrint.Name = "btnPrint";
			this.btnPrint.Size = new System.Drawing.Size(62, 20);
			this.btnPrint.TabIndex = 1;
			this.btnPrint.Text = "打印";
			this.btnPrint.UseVisualStyleBackColor = false;
			this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
			// 
			// btnPrintView
			// 
			this.btnPrintView.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.btnPrintView.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnPrintView.Location = new System.Drawing.Point(127, 4);
			this.btnPrintView.Name = "btnPrintView";
			this.btnPrintView.Size = new System.Drawing.Size(62, 20);
			this.btnPrintView.TabIndex = 2;
			this.btnPrintView.Text = "预览";
			this.btnPrintView.UseVisualStyleBackColor = false;
			this.btnPrintView.Click += new System.EventHandler(this.btnPrintView_Click);
			// 
			// wordView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(884, 733);
			this.Controls.Add(this.btnPrintView);
			this.Controls.Add(this.btnPrint);
			this.Controls.Add(this.axFramerControl1);
			this.Name = "wordView";
			this.RightToLeftLayout = true;
			this.Text = "wordView";
			((System.ComponentModel.ISupportInitialize)(this.axFramerControl1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private AxDSOFramer.AxFramerControl axFramerControl1;
		private System.Windows.Forms.Button btnPrint;
		private System.Windows.Forms.Button btnPrintView;
	}
}