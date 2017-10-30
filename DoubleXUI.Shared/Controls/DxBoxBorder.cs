using Microsoft.Win32;
using Microsoft.Windows.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DoubleXUI.Controls
{
    public class DxBoxBorder : Border
    {
        #region 四方向边框属性定义

        public Brush LeftBorderBrush
        {
            get { return (Brush)GetValue(LeftBorderBrushProperty); }
            set { SetValue(LeftBorderBrushProperty, value); }
        }


        public static readonly DependencyProperty LeftBorderBrushProperty =
            DependencyProperty.Register("LeftBorderBrush", typeof(Brush), typeof(DxBoxBorder), new PropertyMetadata(null));

        public Brush TopBorderBrush
        {
            get { return (Brush)GetValue(TopBorderBrushProperty); }
            set { SetValue(TopBorderBrushProperty, value); }
        }


        public static readonly DependencyProperty TopBorderBrushProperty =
            DependencyProperty.Register("TopBorderBrush", typeof(Brush), typeof(DxBoxBorder), new PropertyMetadata(null));

        public Brush RightBorderBrush
        {
            get { return (Brush)GetValue(RightBorderBrushProperty); }
            set { SetValue(RightBorderBrushProperty, value); }
        }


        public static readonly DependencyProperty RightBorderBrushProperty =
            DependencyProperty.Register("RightBorderBrush", typeof(Brush), typeof(DxBoxBorder), new PropertyMetadata(null));

        public Brush BottomBorderBrush
        {
            get { return (Brush)GetValue(BottomBorderBrushProperty); }
            set { SetValue(BottomBorderBrushProperty, value); }
        }


        public static readonly DependencyProperty BottomBorderBrushProperty =
            DependencyProperty.Register("BottomBorderBrush", typeof(Brush), typeof(DxBoxBorder), new PropertyMetadata(null));

        #endregion

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {

            base.OnRender(dc);
            bool useLayoutRounding = base.UseLayoutRounding;

            Thickness borderThickness = this.BorderThickness;
            CornerRadius cornerRadius = this.CornerRadius;
            double topLeft = cornerRadius.TopLeft;
            bool flag = !DoubleUtil.IsZero(topLeft);
            Brush borderBrush = null;

            Pen pen = null;
            if (pen == null)
            {
                pen = new Pen();
                borderBrush = LeftBorderBrush;
                pen.Brush = LeftBorderBrush;
                if (useLayoutRounding)
                {
                    pen.Thickness = UlementEx.RoundLayoutValue(borderThickness.Left, DoubleUtil.DpiScaleX);
                }
                else
                {
                    pen.Thickness = borderThickness.Left;
                }
                if (borderBrush != null)
                {
                    if (borderBrush.IsFrozen)
                    {
                        pen.Freeze();
                    }
                }


                if (DoubleUtil.GreaterThan(borderThickness.Left, 0.0))
                {
                    double num = pen.Thickness * 0.5;
                    dc.DrawLine(pen, new Point(num, 0.0), new Point(num, base.RenderSize.Height));
                }
                if (DoubleUtil.GreaterThan(borderThickness.Right, 0.0))
                {

                    pen = new Pen();
                    pen.Brush = RightBorderBrush;
                    if (useLayoutRounding)
                    {
                        pen.Thickness = UlementEx.RoundLayoutValue(borderThickness.Right, DoubleUtil.DpiScaleX);
                    }
                    else
                    {
                        pen.Thickness = borderThickness.Right;
                    }
                    if (borderBrush != null)
                    {
                        if (borderBrush.IsFrozen)
                        {
                            pen.Freeze();
                        }
                    }

                    double num = pen.Thickness * 0.5;
                    dc.DrawLine(pen, new Point(base.RenderSize.Width - num, 0.0), new Point(base.RenderSize.Width - num, base.RenderSize.Height));
                }
                if (DoubleUtil.GreaterThan(borderThickness.Top, 0.0))
                {


                    pen = new Pen();
                    pen.Brush = TopBorderBrush;
                    if (useLayoutRounding)
                    {
                        pen.Thickness = UlementEx.RoundLayoutValue(borderThickness.Top, DoubleUtil.DpiScaleY);
                    }
                    else
                    {
                        pen.Thickness = borderThickness.Top;
                    }
                    if (borderBrush != null)
                    {
                        if (borderBrush.IsFrozen)
                        {
                            pen.Freeze();
                        }
                    }


                    double num = pen.Thickness * 0.5;
                    dc.DrawLine(pen, new Point(0.0, num), new Point(base.RenderSize.Width, num));
                }
                if (DoubleUtil.GreaterThan(borderThickness.Bottom, 0.0))
                {


                    pen = new Pen();
                    pen.Brush = BottomBorderBrush;
                    if (useLayoutRounding)
                    {
                        pen.Thickness = UlementEx.RoundLayoutValue(borderThickness.Bottom, DoubleUtil.DpiScaleY);
                    }
                    else
                    {
                        pen.Thickness = borderThickness.Bottom;
                    }
                    if (borderBrush != null)
                    {
                        if (borderBrush.IsFrozen)
                        {
                            pen.Freeze();
                        }
                    }

                    double num = pen.Thickness * 0.5;
                    dc.DrawLine(pen, new Point(0.0, base.RenderSize.Height - num), new Point(base.RenderSize.Width, base.RenderSize.Height - num));
                }


            }

        }

        private class DoubleUtil
        {

            public static double DpiScaleX
            {
                get
                {
                    int dx = 0;
                    int dy = 0;
                    GetDPI(out dx, out dy);
                    if (dx != 96)
                    {
                        return (double)dx / 96.0;
                    }
                    return 1.0;
                }
            }

            public static double DpiScaleY
            {
                get
                {
                    int dx = 0;
                    int dy = 0;
                    GetDPI(out dx, out dy);
                    if (dy != 96)
                    {
                        return (double)dy / 96.0;
                    }
                    return 1.0;
                }
            }

            public static void GetDPI(out int dpix, out int dpiy)
            {
                dpix = 0;
                dpiy = 0;
                using (System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_DesktopMonitor"))
                {
                    using (System.Management.ManagementObjectCollection moc = mc.GetInstances())
                    {

                        foreach (System.Management.ManagementObject each in moc)
                        {
                            dpix = int.Parse((each.Properties["PixelsPerXLogicalInch"].Value.ToString()));
                            dpiy = int.Parse((each.Properties["PixelsPerYLogicalInch"].Value.ToString()));
                        }
                    }
                }
            }
            public static bool GreaterThan(double value1, double value2)
            {
                return value1 > value2 && !DoubleUtil.AreClose(value1, value2);
            }

            public static bool AreClose(double value1, double value2)
            {
                if (value1 == value2)
                {
                    return true;
                }
                double num = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.2204460492503131E-16;
                double num2 = value1 - value2;
                return -num < num2 && num > num2;
            }

            public static bool IsZero(double value)
            {
                return Math.Abs(value) < 2.2204460492503131E-15;
            }

            [StructLayout(LayoutKind.Explicit)]
            private struct NanUnion
            {
                [FieldOffset(0)]
                internal double DoubleValue;
                [FieldOffset(0)]
                internal ulong UintValue;
            }

            public static bool IsNaN(double value)
            {
                DoubleUtil.NanUnion nanUnion = default(DoubleUtil.NanUnion);
                nanUnion.DoubleValue = value;
                ulong num = nanUnion.UintValue & 18442240474082181120uL;
                ulong num2 = nanUnion.UintValue & 4503599627370495uL;
                return (num == 9218868437227405312uL || num == 18442240474082181120uL) && num2 != 0uL;
            }



        }

        private static class UlementEx
        {
            public static double RoundLayoutValue(double value, double dpiScale)
            {
                double num;
                if (!DoubleUtil.AreClose(dpiScale, 1.0))
                {
                    num = Math.Round(value * dpiScale) / dpiScale;
                    if (DoubleUtil.IsNaN(num) || double.IsInfinity(num) || DoubleUtil.AreClose(num, 1.7976931348623157E+308))
                    {
                        num = value;
                    }
                }
                else
                {
                    num = Math.Round(value);
                }
                return num;
            }
        }
    }
}
