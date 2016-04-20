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
    /// serRecharge.xaml 的交互逻辑
    /// </summary>
    public partial class serRecharge : Window
    {
		DsRsrc dsrsrc = new DsRsrc();
        public serRecharge(string spidNo, DateTime dStart,DateTime dEndtime)
        {
            InitializeComponent();
            getFinace(spidNo, dStart, dEndtime);
        }
        private void getFinace(string pidNo, DateTime strtime, DateTime endtime)
        {
			dsrsrc.trainMangeDataSet.RECHARGE_RECORDDataTable.Clear();
			DBAccessProc.DBAccessHelper.getRechargeRecord(pidNo, strtime, endtime, dsrsrc.trainMangeDataSet);
			if (dsrsrc.trainMangeDataSet.RECHARGE_RECORDDataTable.Rows.Count > 0)
            {
				dataGridFinace.ItemsSource = dsrsrc.trainMangeDataSet.RECHARGE_RECORDDataTable.DefaultView;
            }
        }

       

    }
}
