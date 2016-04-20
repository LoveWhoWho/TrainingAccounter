using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AxLib
{
    public partial class AxLibControl : UserControl
    {
        private AxTERMBLib.AxTermb TermbCTRL;
        private AxDORONURUACTIVEXLib.AxDoronUruActiveX axDoronUruActiveXCtrl;
        private byte[] m_sPhotoBuf;
        string picName = "";
        public  PhotoSnapper ps = null;
        IDCardInfo idCardInfo = new IDCardInfo();
        /// <summary>
        /// 身份证相关信息
        /// </summary>
        public IDCardInfo idCard
        {
            get { return idCardInfo; }
            set { idCardInfo = value; }
        }
        public AxLibControl()
        {
            InitializeComponent();
            TermbCTRL = new AxTERMBLib.AxTermb();
            axDoronUruActiveXCtrl = new AxDORONURUACTIVEXLib.AxDoronUruActiveX();
             ps = new PhotoSnapper(picName); 
        }
        
        /// <summary>
        /// 保存身份证信息对象类
        /// </summary>
        public class IDCardInfo
        {
            private string m_sPidNo;
            private string m_sName;
            private string m_sSex;
            private DateTime m_dtBirthDate;
            private byte[] m_sPhotoBuffer;
            private string m_sAddress;
            /// <summary>
            /// 学员姓名
            /// </summary>
            public string sName
            {
                get { return m_sName; }
                set { m_sName = value; }
            }
            /// <summary>
            /// 身份证号
            /// </summary>
            public string sPidNo
            {
                get { return m_sPidNo; }
                set { m_sPidNo = value; }
            }
            /// <summary>
            /// 性别
            /// </summary>
            public string sSex
            {
                get { return m_sSex; }
                set { m_sSex = value; }
            }
            /// <summary>
            /// 出生日期
            /// </summary>
            public DateTime sBirthDate
            {
                get { return m_dtBirthDate; }
                set { m_dtBirthDate = value; }
            }
            /// <summary>
            /// 身份证照片
            /// </summary>
            public byte[] sPhotoBuffer
            {
                get { return m_sPhotoBuffer; }
                set { m_sPhotoBuffer = value; }
            }
            /// <summary>
            /// 详细地址
            /// </summary>
            public string sAddress
            {
                get { return m_sAddress; }
                set { m_sAddress = value; }
            }
        }
        
        /// <summary>
        ///  获取身份证信息，读取成功返回ReadSuccess，身份证阅读设备初始化失败返回InitFalse，读卡失败返回ReadFalse
        /// </summary>
        /// <returns></returns>
        public string GetIDCardInfo()
        {
            string sReturnInfo = "ReadSuccess";
            if (this.axTermb.OpenComm(1001) != 1)
            {
                sReturnInfo = "InitFalse";
                //MessageBox.Show("身份证阅读设备初始化失败！");
                return sReturnInfo;
            }
            if (this.axTermb.Authen() == 1)
            {
                int rc = this.axTermb.ReadCardPath("C:\\herd.wlt", 1);
                if (rc == 1)
                {
                    //显示基本信息,如果记录唯一则添加到待安排项中
                    idCardInfo.sName = this.axTermb.SName.ToString();
                    idCardInfo.sPidNo = this.axTermb.sIDNo.ToString();
                    if (this.axTermb.sSex.ToString() == "1")
                        idCardInfo.sSex = "男";
                    else
                        idCardInfo.sSex = "女";
                    idCardInfo.sBirthDate = new DateTime(Convert.ToInt32(this.axTermb.sBornDate.Substring(0, 4)), Convert.ToInt32(this.axTermb.sBornDate.Substring(4, 2)), Convert.ToInt32(this.axTermb.sBornDate.Substring(6, 2)));// this.axTermb.sBornDate;
                    //dtpBirthDt.Value = new DateTime(Convert.ToInt32(sBirthDate.Substring(0, 4)), Convert.ToInt32(sBirthDate.Substring(4, 2)), Convert.ToInt32(sBirthDate.Substring(6, 2)));

                    m_sPhotoBuf = null;
                    ImageBuffer("C:\\herd.bmp", ref m_sPhotoBuf);
                    idCardInfo.sPhotoBuffer = m_sPhotoBuf;
                    idCardInfo.sAddress = this.axTermb.sAddress.ToString();//详细地址
                    }
                else
                {
                    //MessageBox.Show("读卡失败！");
                    sReturnInfo = "";
                }
                this.axTermb.EndComm();
                return sReturnInfo;
            }
            else
            {
                //MessageBox.Show("读卡失败！");
                sReturnInfo = "ReadFalse";
            }
            this.axTermb.EndComm();
            return sReturnInfo;

            
        }
        /// <summary>
        /// 返回照片元数据
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="buffer"></param>
        public  void ImageBuffer(string imagePath, ref byte[] buffer)
        {
            try
            {
                System.IO.FileStream stream = new System.IO.FileStream(imagePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                buffer = new byte[stream.Length];

                stream.Read(buffer, 0, (int)stream.Length);
                stream.Close();

            }
            catch (Exception ex)
            {
                throw new ApplicationException("读取图像错误: " + ex.Message);
            }
        }
        /// <summary>
        /// 显示照片
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="picBox"></param>
        public static void ShowImageFromBuffer(byte[] buffer, PictureBox picBox)
        {
            int width = picBox.Width;
            int height = picBox.Height;
            bool bUseOrigSize = true;
            System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer, true);
            try
            {
                stream.Write(buffer, 0, buffer.Length);
                Bitmap newimg = new Bitmap(stream);
                if (newimg.Width > width || newimg.Height > height)
                    bUseOrigSize = false;
                Bitmap showimg = null;
                if (bUseOrigSize)
                    showimg = new Bitmap(newimg, new Size(newimg.Width, newimg.Height));
                else
                    showimg = new Bitmap(newimg, new Size(width, height));
                picBox.Image = showimg;
            }
            catch (Exception ex)
            {
                stream.Close();
                throw new ApplicationException("显示图像错误。" + "," + ex.Message);
            }
            stream.Close();
        }
        /// <summary>
        /// 读取指纹信息，返回指纹字符串
        /// </summary>
        /// <returns>返回指纹字符串</returns>
        public string EnrollFingerprint()
        {
            string sFingerprint = "";

            EnrollFingerprintForm efForm = new EnrollFingerprintForm(DBAccessProc.Common.FingerPrintSensorOcx);
            efForm.ShowDialog();
            sFingerprint = efForm.Fingerprint;

            return sFingerprint;
        }
    }
}
