using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TrainingControl;

namespace TrainingAccounter
{
    /// <summary>
    /// Recharge.xaml 的交互逻辑
    /// </summary>
    public delegate void ChangeTextHandler(string text); 
    public partial class Recharge : Window
    {
		DsRsrc dsrsrc ;
        public event ChangeTextHandler ChangeTextEvent;
        private string tempText = string.Empty;
        public Recharge(DsRsrc dsr)
        {
            InitializeComponent();
			dsrsrc = dsr;         
        }
		public string _sChargingMode { get; set; }
		public string _sCarType { get; set; }
        private void btnDeposit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double temp = 0;
                if (!Double.TryParse(this.txbRechargeAccount.Text.ToString().Trim(), out temp))
                {
                    System.Windows.MessageBox.Show("填入的充值金额不合法，请重新输入！");
                    return;
                }
                if (temp <=0)
                {
                    System.Windows.MessageBox.Show("填入的充值金额不合法，请重新输入！");
                    return;
                }
                if (tboxSeqNo.Text.Trim() == "")
                {
                    System.Windows.MessageBox.Show("请先选择一个学员！");
                    return;
                }
				if (this.rdbCustom.IsChecked == false)
				{
					dsrsrc.GetAllSetting();
					string sRxTxt = "VipPreferential" + _sCarType;
				    temp+=double.Parse(dsrsrc.TraSettingValues[sRxTxt].ToString())*temp;             
				}
				dsrsrc.RechargeAccount(tboxSeqNo.Text.Trim(), temp, txbPidNo.Text, tbxManger.Text.Trim(), "充值");
				dsrsrc.getTraStudinfoBySeqNo(Double.Parse(tboxSeqNo.Text.ToString()));
				if (dsrsrc.trainMangeDataSet.TraineeDataTable.Rows.Count > 0)
                {
                    double dChargingStandard = 0;
                    dChargingStandard = dsrsrc.GetChargingStandard(dsrsrc.sBillingMode, dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["LICENSE_TYPE_CD"].ToString());
					if (dChargingStandard == 0)
						return;
					tboxBlce.Text = dsrsrc.trainMangeDataSet.TraineeDataTable.Rows[0]["BALANCE"].ToString();
                    txbRechargeAccount.Text = "";
                }
                 dsrsrc.MainBarText= tempText = string.Format("学员：" + txbUserName.Text + "充值完成，充值金额：" + txbRechargeAccount.Text + " 元；账户余额：" + tboxBlce.Text + " 元");
                 tblxStatus.Text = "充值完成";
            }
            catch(Exception ex)
            {
                MessageBox.Show("充值出错，错误信息：" + ex.Message.ToString());
                tblxStatus.Text = "充值失败";
                return;
            }
        }

        private void btnSettlement_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (ChangeTextEvent != null)
                ChangeTextEvent(tempText);		
        }   
    }
}
