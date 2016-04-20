namespace AxLib
{
    partial class PhotoSnapper
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhotoSnapper));
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.axAvCaputureCtrlSnapper = new AxAVCAPUTURECTRLLib.AxAvCaputureCtrl();
            ((System.ComponentModel.ISupportInitialize)(this.axAvCaputureCtrlSnapper)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(20, 400);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(425, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "提示：用鼠标点击将要截取的考生头像的中心点，系统将自动锁定并截取照片。";
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClose.Location = new System.Drawing.Point(430, 400);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(58, 25);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "关闭";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // axAvCaputureCtrlSnapper
            // 
            this.axAvCaputureCtrlSnapper.Enabled = true;
            this.axAvCaputureCtrlSnapper.Location = new System.Drawing.Point(1, 1);
            this.axAvCaputureCtrlSnapper.Name = "axAvCaputureCtrlSnapper";
            this.axAvCaputureCtrlSnapper.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAvCaputureCtrlSnapper.OcxState")));
            this.axAvCaputureCtrlSnapper.Size = new System.Drawing.Size(640, 480);
            this.axAvCaputureCtrlSnapper.TabIndex = 0;
            // 
            // PhotoSnapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ClientSize = new System.Drawing.Size(500, 450);           
            this.Controls.Add(this.axAvCaputureCtrlSnapper);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.MaximizeBox = false;
            this.Name = "PhotoSnapper";
            this.Text = "照相";
            this.Load += new System.EventHandler(this.PhotoSnapper_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axAvCaputureCtrlSnapper)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClose;
        private AxAVCAPUTURECTRLLib.AxAvCaputureCtrl axAvCaputureCtrlSnapper;
    }
}