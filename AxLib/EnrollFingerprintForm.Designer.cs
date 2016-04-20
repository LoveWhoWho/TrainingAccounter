namespace AxLib
{
    partial class EnrollFingerprintForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnrollFingerprintForm));
            this.axDoronUruActiveXCtrl = new AxDORONURUACTIVEXLib.AxDoronUruActiveX();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblMsg = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.axZKFPEngX = new AxZKFPEngXControl.AxZKFPEngX();
            this.picFp = new System.Windows.Forms.PictureBox();
            this.rdbBioKey = new System.Windows.Forms.RadioButton();
            this.rdbDoronUruActiveX4B = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.axDoronUruActiveXCtrl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axZKFPEngX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFp)).BeginInit();
            this.SuspendLayout();
            // 
            // axDoronUruActiveXCtrl
            // 
            this.axDoronUruActiveXCtrl.Enabled = true;
            this.axDoronUruActiveXCtrl.Location = new System.Drawing.Point(21, 39);
            this.axDoronUruActiveXCtrl.Name = "axDoronUruActiveXCtrl";
            this.axDoronUruActiveXCtrl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axDoronUruActiveXCtrl.OcxState")));
            this.axDoronUruActiveXCtrl.Size = new System.Drawing.Size(150, 200);
            this.axDoronUruActiveXCtrl.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(196, 143);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(103, 45);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "确定(&O)";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(196, 194);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(103, 45);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblMsg
            // 
            this.lblMsg.BackColor = System.Drawing.Color.Transparent;
            this.lblMsg.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMsg.ForeColor = System.Drawing.Color.Green;
            this.lblMsg.Location = new System.Drawing.Point(21, 12);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(278, 23);
            this.lblMsg.TabIndex = 4;
            this.lblMsg.Text = "提示信息";
            this.lblMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(177, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 54);
            this.label1.TabIndex = 5;
            this.label1.Text = "请考生将手指放在指纹仪上，按下然后抬起，重复4次。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(177, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "操作方法：";
            // 
            // axZKFPEngX
            // 
            this.axZKFPEngX.Enabled = true;
            this.axZKFPEngX.Location = new System.Drawing.Point(180, 12);
            this.axZKFPEngX.Name = "axZKFPEngX";
            this.axZKFPEngX.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axZKFPEngX.OcxState")));
            this.axZKFPEngX.Size = new System.Drawing.Size(24, 24);
            this.axZKFPEngX.TabIndex = 6;
            // 
            // picFp
            // 
            this.picFp.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.picFp.Location = new System.Drawing.Point(21, 39);
            this.picFp.Name = "picFp";
            this.picFp.Size = new System.Drawing.Size(150, 200);
            this.picFp.TabIndex = 7;
            this.picFp.TabStop = false;
            // 
            // rdbBioKey
            // 
            this.rdbBioKey.AutoSize = true;
            this.rdbBioKey.Checked = true;
            this.rdbBioKey.Location = new System.Drawing.Point(24, 263);
            this.rdbBioKey.Name = "rdbBioKey";
            this.rdbBioKey.Size = new System.Drawing.Size(59, 16);
            this.rdbBioKey.TabIndex = 10;
            this.rdbBioKey.Text = "BioKey";
            this.rdbBioKey.UseVisualStyleBackColor = true;
            this.rdbBioKey.CheckedChanged += new System.EventHandler(this.rdbBioKey_CheckedChanged);
            // 
            // rdbDoronUruActiveX4B
            // 
            this.rdbDoronUruActiveX4B.AutoSize = true;
            this.rdbDoronUruActiveX4B.Location = new System.Drawing.Point(140, 263);
            this.rdbDoronUruActiveX4B.Name = "rdbDoronUruActiveX4B";
            this.rdbDoronUruActiveX4B.Size = new System.Drawing.Size(125, 16);
            this.rdbDoronUruActiveX4B.TabIndex = 11;
            this.rdbDoronUruActiveX4B.Text = "DoronUruActiveX4B";
            this.rdbDoronUruActiveX4B.UseVisualStyleBackColor = true;
            // 
            // EnrollFingerprintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 291);
            this.Controls.Add(this.rdbDoronUruActiveX4B);
            this.Controls.Add(this.rdbBioKey);
            this.Controls.Add(this.picFp);
            this.Controls.Add(this.axZKFPEngX);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.axDoronUruActiveXCtrl);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnrollFingerprintForm";
            this.Text = "采录指纹";
            this.Load += new System.EventHandler(this.EnrollFingerprintForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axDoronUruActiveXCtrl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axZKFPEngX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AxDORONURUACTIVEXLib.AxDoronUruActiveX axDoronUruActiveXCtrl;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private AxZKFPEngXControl.AxZKFPEngX axZKFPEngX;
        private System.Windows.Forms.PictureBox picFp;
        private System.Windows.Forms.RadioButton rdbBioKey;
        private System.Windows.Forms.RadioButton rdbDoronUruActiveX4B;
    }
}