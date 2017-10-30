using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace DoubleX.Upload
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //MessageBox.Show("Error encountered! Please contact support." + Environment.NewLine + e.Exception.Message);
            //Shutdown(1);
            string errorMsg = e.Exception.ToString();
            if (e.Exception.InnerException != null) {
                errorMsg = e.Exception.InnerException.ToString();
            }
            
        }
    }
}
