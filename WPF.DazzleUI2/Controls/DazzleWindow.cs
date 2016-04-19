using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPF.DazzleUI2.Util;
using System.Windows;

namespace WPF.DazzleUI2.Controls
{
    public class DazzleWindow : Window
    {
        public DazzleWindow()
        {
            this.DefaultStyleKey = typeof(DazzleWindow);

            //缩放，最大化修复
            WindowBehaviorHelper wh = new WindowBehaviorHelper(this);
            wh.RepairBehavior();

            this.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(DazzleWindow_MouseLeftButtonDown);
        }

        void DazzleWindow_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove(); 
        }
    }
}
