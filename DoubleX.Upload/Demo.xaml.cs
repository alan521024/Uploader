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
using DoubleXUI.Controls;

namespace DoubleX.Upload
{
    /// <summary>
    /// Demo.xaml 的交互逻辑
    /// </summary>
    public partial class Demo : DxWindow
    {
        public Demo()
        {
            InitializeComponent();
        }

        private void DxButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("DxButton(Default) Click");
        }

        private void DxButton_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("DxButton(Enable) Click");
        }
    }
}
