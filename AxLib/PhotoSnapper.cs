using System;
using System.Drawing;
using System.Windows.Forms;
//using AVCAPUTURECTRLLib;
using DBAccessProc;


namespace AxLib
{
    public partial class PhotoSnapper : Form
    {
        string m_sPhotoName;
        public PhotoSnapper(string sPhotoName)
        {
            InitializeComponent();
            axAvCaputureCtrlSnapper.MouseCaptureChanged += new System.EventHandler(this.axAvCaputureCtrlSnapper_MouseCaptureChanged);
            m_sPhotoName = sPhotoName;
        }

        private void PhotoSnapper_Load(object sender, EventArgs e)
        {
            bool rc = this.axAvCaputureCtrlSnapper.SetDeviceName(DBAccessProc.Common.WebCamName);
            if (!rc)
            {
                System.Windows.Forms.MessageBox.Show("照相设备初始化失败！");
                Close();
                return;
            }

            axAvCaputureCtrlSnapper.StartPreview();
        }

        private void axAvCaputureCtrlSnapper_MouseCaptureChanged(Object sender, EventArgs e)
        {
            try
            {
                Point ptCursor = Cursor.Position;
                ptCursor = axAvCaputureCtrlSnapper.PointToClient(ptCursor);

                //Assume preview size is Width:Height = 4:3, photo size is 3:4
                int nPhotoWidth = Convert.ToInt32(DBAccessProc.Common.PhotoWidth);
                int nPhotoHeight = Convert.ToInt32(DBAccessProc.Common.PhotoHeight);

                int nLeft = ptCursor.X - nPhotoWidth / 2;
                if (nLeft > axAvCaputureCtrlSnapper.Width - nPhotoWidth)
                    nLeft = axAvCaputureCtrlSnapper.Width - nPhotoWidth;
                if (nLeft < 0)
                    nLeft = 0;

                int nTop = ptCursor.Y - nPhotoHeight / 2;
                if (nTop > axAvCaputureCtrlSnapper.Height - nPhotoHeight)
                    nTop = axAvCaputureCtrlSnapper.Height - nPhotoHeight;
                if (nTop < 0)
                    nTop = 0;

                axAvCaputureCtrlSnapper.SnapCaptureOnArea(m_sPhotoName, nLeft, nTop, nPhotoWidth, nPhotoHeight);
                Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("系统错误：" + ex.Message);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            axAvCaputureCtrlSnapper.StopPreview();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            btnClose.Focus();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

     

        //private void InitializeComponent()
        //{
        //    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhotoSnapper));
        //    this.SuspendLayout();
        //    // 
        //    // PhotoSnapper
        //    // 
        //    this.BackColor = System.Drawing.SystemColors.Highlight;
        //    this.ClientSize = new System.Drawing.Size(282, 253);
        //    this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        //    this.Name = "PhotoSnapper";
        //    this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
        //    this.ResumeLayout(false);

        //}
    }
}