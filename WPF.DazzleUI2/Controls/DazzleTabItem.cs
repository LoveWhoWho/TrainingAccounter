using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace WPF.DazzleUI2.Controls
{
	public class DazzleTabItem : TabItem
	{
		public static readonly DependencyProperty MyMoverBrushProperty;
		public static readonly DependencyProperty MyEnterBrushProperty;
		public Brush MyMoverBrush
		{
			get
			{
				return base.GetValue(DazzleTabItem.MyMoverBrushProperty) as Brush;
			}
			set
			{
				base.SetValue(DazzleTabItem.MyMoverBrushProperty, value);
			}
		}
		public Brush MyEnterBrush
		{
			get
			{
				return base.GetValue(DazzleTabItem.MyEnterBrushProperty) as Brush;
			}
			set
			{
				base.SetValue(DazzleTabItem.MyEnterBrushProperty, value);
			}
		}
		static DazzleTabItem()
		{
			DazzleTabItem.MyMoverBrushProperty = DependencyProperty.Register("MyMoverBrush", typeof(Brush), typeof(DazzleTabItem), new PropertyMetadata(null));
			DazzleTabItem.MyEnterBrushProperty = DependencyProperty.Register("MyEnterBrush", typeof(Brush), typeof(DazzleTabItem), new PropertyMetadata(null));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DazzleTabItem), new FrameworkPropertyMetadata(typeof(DazzleTabItem)));
		}
        public DazzleTabItem()
		{
			base.Header = "测试按钮";
			base.Background = Brushes.Blue;
		}
	}
}
