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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TrainingControl
{
    /// <summary>
    /// AuthorizationControl.xaml 的交互逻辑
    /// </summary>
    public partial class AuthorizationControl : Window
    {
        public AuthorizationControl()
        {
            InitializeComponent();
        }

        //private bool result = false;

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }
}
