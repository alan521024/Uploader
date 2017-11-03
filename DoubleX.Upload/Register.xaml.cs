using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using WinForm = System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DoubleX.Infrastructure.Utility;
using DoubleXUI.Controls;

namespace DoubleX.Upload
{
    /// <summary>
    /// Register.xaml 的交互逻辑
    /// </summary>
    public partial class Register : DxWindow
    {
        public Register()
        {
            InitializeComponent();
            txtMac.Text = MacHelper.GetMacAddress();
            txtCPU.Text= Win32Helper.GetCpuID();
        }

        private void btnOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = false;
            //dlg.DefaultExt = ".txt";
            //dlg.Filter = "Text documents (.txt)|*.txt";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                txtPath.Text = dlg.FileName;
            }
        }
    }
}
