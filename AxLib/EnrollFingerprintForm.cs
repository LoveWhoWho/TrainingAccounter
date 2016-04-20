using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace AxLib
{
    public partial class EnrollFingerprintForm : Form
    {
        string m_sFingerprint;
        int m_nPressCount = 0;
        bool m_bEnrollFinish = false;
        string m_sFpPicFilePath = "FingerPrintTemp.jpg";

        public string Fingerprint
        {
            get
            {
                return m_sFingerprint;
            }
        }

        public EnrollFingerprintForm(string m_sFingerPrintSensorOcx)
        {
            InitializeComponent();
            ControlBox = false;
            if (m_sFingerPrintSensorOcx == "1")
            {
                picFp.Visible = false;
                axDoronUruActiveXCtrl.Terminate();
                axDoronUruActiveXCtrl.Init(0.001, 1000, true);
                this.axDoronUruActiveXCtrl.Notify += new AxDORONURUACTIVEXLib._DDoronUruActiveXEvents_NotifyEventHandler(axDoronUruActiveXCtrl_Notify);
                this.axDoronUruActiveXCtrl.NotifyCount += new AxDORONURUACTIVEXLib._DDoronUruActiveXEvents_NotifyCountEventHandler(axDoronUruActiveXCtrl_NotifyCount);
            }
            else if (m_sFingerPrintSensorOcx == "0")
            {
                picFp.Visible = true;
                axZKFPEngX.SensorIndex = 0;
                axZKFPEngX.EnrollCount = 4;
                axZKFPEngX.EndEngine();
                switch (axZKFPEngX.InitEngine())
                {
                    case 0:
                        lblMsg.Text = "指纹仪初始化成功！";
                        this.axZKFPEngX.OnImageReceived += new AxZKFPEngXControl.IZKFPEngXEvents_OnImageReceivedEventHandler(this.axZKFPEngX_OnImageReceived);
                        this.axZKFPEngX.OnFeatureInfo += new AxZKFPEngXControl.IZKFPEngXEvents_OnFeatureInfoEventHandler(this.axZKFPEngX_OnFeatureInfo);
                        this.axZKFPEngX.OnEnroll += new AxZKFPEngXControl.IZKFPEngXEvents_OnEnrollEventHandler(this.axZKFPEngX_OnEnroll);
                        break;
                    case 1:
                        ShowErrorMessage("指纹识别驱动程序加载失败！");
                        break;
                    case 2:
                        ShowErrorMessage("没有连接指纹仪！");
                        break;
                    case 3:
                        ShowErrorMessage("指定的指纹仪不存在！");
                        break;
                }
            }

            m_sFingerprint = "";
        }

        void axDoronUruActiveXCtrl_NotifyCount(object sender, AxDORONURUACTIVEXLib._DDoronUruActiveXEvents_NotifyCountEvent e)
        {
            if (e.count > 0)
                lblMsg.Text = "还需按" + e.count.ToString() + "次！";
            else
            {
                lblMsg.Text = "指纹采录完成！";
                btnOK.Enabled = true;
            }
        }

        void axDoronUruActiveXCtrl_Notify(object sender, AxDORONURUACTIVEXLib._DDoronUruActiveXEvents_NotifyEvent e)
        {
            axDoronUruActiveXCtrl.Invalidate();
            switch (e.notifyCode)
            {
                case 1:
                    lblMsg.Text = "连接指纹仪成功！";
                    break;
                case -1:
                    lblMsg.Text = "连接指纹仪失败！";
                    break;
                case -10:
                    lblMsg.Text = "无效的指纹仪设备！";
                    break;
            }
            axDoronUruActiveXCtrl.Invalidate();

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rdbDoronUruActiveX4B.Checked)
            {
                axDoronUruActiveXCtrl.GetBase64Feature(ref m_sFingerprint);
                if (m_sFingerprint.Length > 1000)
                    m_sFingerprint = m_sFingerprint.Substring(0, 1000);

                axDoronUruActiveXCtrl.Terminate();
            }
            else if (rdbBioKey.Checked)
            {
                axZKFPEngX.EndEngine();
                if (picFp.Image != null)
                {
                    picFp.Image.Dispose();//释放文件资源
                    picFp.Image = null;//清除图片
                }
            }
            
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_sFingerprint = "";
            if (rdbDoronUruActiveX4B.Checked)
                axDoronUruActiveXCtrl.Terminate();
            else if (rdbBioKey.Checked)
            {
                axZKFPEngX.EndEngine();
                if (picFp.Image != null)
                {
                    picFp.Image.Dispose();//释放文件资源
                    picFp.Image = null;//清除图片
                }
            }

            this.Close();
        }

        private void EnrollFingerprintForm_Load(object sender, EventArgs e)
        {
            if (rdbDoronUruActiveX4B.Checked)
                axDoronUruActiveXCtrl.Enroll("", "");
            else if (rdbBioKey.Checked)
                axZKFPEngX.BeginEnroll();
        }

        private void axZKFPEngX_OnImageReceived(object sender, AxZKFPEngXControl.IZKFPEngXEvents_OnImageReceivedEvent e)
        {
            if (m_bEnrollFinish)
                return;
            if (picFp.Image != null)
            {
                picFp.Image.Dispose();//释放文件资源
                picFp.Image = null;//清除图片
            }
            //try
            //{
            //    if (System.IO.File.Exists(m_sFpPicFilePath))
            //        System.IO.File.Delete(m_sFpPicFilePath);//删除图片文件
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            
            try
            {
                axZKFPEngX.SaveJPG(m_sFpPicFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            picFp.SizeMode = PictureBoxSizeMode.StretchImage;
            picFp.Image = System.Drawing.Bitmap.FromFile(m_sFpPicFilePath);
            picFp.Refresh();
        }

        private void axZKFPEngX_OnFeatureInfo(object sender, AxZKFPEngXControl.IZKFPEngXEvents_OnFeatureInfoEvent e)
        {
            if (m_bEnrollFinish)
                return;
            if (e.aQuality != 0)
            {
                if (e.aQuality == 1)
                    ShowErrorMessage("指纹特征点不够！请重按。");
                else
                    ShowErrorMessage("其它原因导致不能取到指纹特征！请重按。");
            }
            else
            {
                m_nPressCount++;
                if (axZKFPEngX.EnrollCount != m_nPressCount)
                    lblMsg.Text = "还需按 " + Convert.ToString(axZKFPEngX.EnrollCount - m_nPressCount) + " 次！";
            }
        }

        private void axZKFPEngX_OnEnroll(object sender, AxZKFPEngXControl.IZKFPEngXEvents_OnEnrollEvent e)
        {
            if (e.actionResult)
            {
                m_sFingerprint = axZKFPEngX.EncodeTemplate1(e.aTemplate);
                lblMsg.Text = "指纹登记成功！";
                axZKFPEngX.CancelEnroll();
                m_bEnrollFinish = true;
                btnOK.Enabled = true;
            }
            else
                ShowErrorMessage("指纹登记失败！请重新登记。");
        }
        public static void ShowErrorMessage(string msg)
        {
            MessageBox.Show(msg, "错误",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void rdbBioKey_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbDoronUruActiveX4B.Checked)
            {
                picFp.Visible = false;
                axDoronUruActiveXCtrl.Terminate();
                axDoronUruActiveXCtrl.Init(0.001, 1000, true);
                this.axDoronUruActiveXCtrl.Notify += new AxDORONURUACTIVEXLib._DDoronUruActiveXEvents_NotifyEventHandler(axDoronUruActiveXCtrl_Notify);
                this.axDoronUruActiveXCtrl.NotifyCount += new AxDORONURUACTIVEXLib._DDoronUruActiveXEvents_NotifyCountEventHandler(axDoronUruActiveXCtrl_NotifyCount);
            }
            else if (rdbBioKey.Checked)
            {
                picFp.Visible = true;
                axZKFPEngX.SensorIndex = 0;
                axZKFPEngX.EnrollCount = 4;
                axZKFPEngX.EndEngine();
                switch (axZKFPEngX.InitEngine())
                {
                    case 0:
                        lblMsg.Text = "指纹仪初始化成功！";
                        this.axZKFPEngX.OnImageReceived += new AxZKFPEngXControl.IZKFPEngXEvents_OnImageReceivedEventHandler(this.axZKFPEngX_OnImageReceived);
                        this.axZKFPEngX.OnFeatureInfo += new AxZKFPEngXControl.IZKFPEngXEvents_OnFeatureInfoEventHandler(this.axZKFPEngX_OnFeatureInfo);
                        this.axZKFPEngX.OnEnroll += new AxZKFPEngXControl.IZKFPEngXEvents_OnEnrollEventHandler(this.axZKFPEngX_OnEnroll);
                        break;
                    case 1:
                        ShowErrorMessage("指纹识别驱动程序加载失败！");
                        break;
                    case 2:
                        ShowErrorMessage("没有连接指纹仪！");
                        break;
                    case 3:
                        ShowErrorMessage("指定的指纹仪不存在！");
                        break;
                }
            }
        }
    }
}