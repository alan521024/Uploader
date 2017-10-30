using Microsoft.Win32;
using Microsoft.Windows.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DoubleXUI.Controls
{
    public class DxButton : Button
    {
        #region 自定义属性

        //圆形
        public static readonly DependencyProperty EllipseDiameterProperty = DependencyProperty.Register("EllipseDiameter", typeof(double), typeof(DxButton), new PropertyMetadata(22D));
        public static readonly DependencyProperty EllipseStrokeThicknessProperty = DependencyProperty.Register("EllipseStrokeThickness", typeof(double), typeof(DxButton), new PropertyMetadata(1D));

        public static readonly DependencyProperty IconLeftProperty = DependencyProperty.Register("IconLeft", typeof(string), typeof(DxButton));
        public static readonly DependencyProperty IconRightProperty = DependencyProperty.Register("IconRight", typeof(string), typeof(DxButton));
        public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register("IconHeight", typeof(double), typeof(DxButton), new PropertyMetadata(12D));
        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register("IconWidth", typeof(double), typeof(DxButton), new PropertyMetadata(12D));

        public double EllipseDiameter
        {
            get { return (double)GetValue(EllipseDiameterProperty); }
            set { SetValue(EllipseDiameterProperty, value); }
        }
        public double EllipseStrokeThickness
        {
            get { return (double)GetValue(EllipseStrokeThicknessProperty); }
            set { SetValue(EllipseStrokeThicknessProperty, value); }
        }


        public string IconLeft
        {
            get { return (string)GetValue(IconLeftProperty); }
            set { SetValue(IconLeftProperty, value); }
        }
        public string IconRight
        {
            get { return (string)GetValue(IconRightProperty); }
            set { SetValue(IconRightProperty, value); }
        }
        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }
        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }

        #endregion

        public DxButton()
        {
            this.DefaultStyleKey = typeof(DxButton);
        }
    }
}
