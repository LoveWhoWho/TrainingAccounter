using System;
using System.Windows;
using System.Windows.Controls;
namespace WPF.DazzleUI2.Controls
{
	public class DazzleTabControl : TabControl
	{
        static DazzleTabControl()
		{
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DazzleTabControl), new FrameworkPropertyMetadata(typeof(DazzleTabControl)));
		}
	}
}
