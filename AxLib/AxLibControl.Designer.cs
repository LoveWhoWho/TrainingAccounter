namespace AxLib
{
    partial class AxLibControl
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxLibControl));
            this.axDoronUruActiveX = new AxDORONURUACTIVEXLib.AxDoronUruActiveX();
            this.axTermb = new AxTERMBLib.AxTermb();
            ((System.ComponentModel.ISupportInitialize)(this.axDoronUruActiveX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axTermb)).BeginInit();
            this.SuspendLayout();
            // 
            // axDoronUruActiveX
            // 
            this.axDoronUruActiveX.Enabled = true;
            this.axDoronUruActiveX.Location = new System.Drawing.Point(0, 0);
            this.axDoronUruActiveX.Name = "axDoronUruActiveX";
            this.axDoronUruActiveX.Size = new System.Drawing.Size(75, 23);
            this.axDoronUruActiveX.TabIndex = 0;
            // 
            // axTermb
            // 
            this.axTermb.Enabled = true;
            this.axTermb.Location = new System.Drawing.Point(175, 3);
            this.axTermb.Name = "axTermb";
            this.axTermb.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTermb.OcxState")));
            this.axTermb.Size = new System.Drawing.Size(102, 126);
            this.axTermb.TabIndex = 3;
            // 
            // AxLibControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.axTermb);
            this.Name = "AxLibControl";
            this.Size = new System.Drawing.Size(569, 342);
            ((System.ComponentModel.ISupportInitialize)(this.axDoronUruActiveX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axTermb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxDORONURUACTIVEXLib.AxDoronUruActiveX axDoronUruActiveX;
        private AxTERMBLib.AxTermb axTermb;

    }
}
