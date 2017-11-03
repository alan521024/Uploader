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
using System.Diagnostics;

namespace DoubleX.Upload
{
    /// <summary>
    /// Register.xaml 的交互逻辑
    /// </summary>
    public partial class Register : DxWindow
    {
        /// <summary>
        /// 授权信息
        /// </summary>
        private LicenseFileModel licenseFileModel { get; set; }

        /// <summary>
        /// 授权统计
        /// </summary>
        private LicenseStatModel licenseStatModel { get; set; }

        public Register()
        {
            InitializeComponent();

            //现有授权信息
            var licPath = string.Format("{0}/data/license.key", AppDomain.CurrentDomain.BaseDirectory).ToLower();
            licenseFileModel = AppHelper.LicenseFileGet(licPath);
            licenseStatModel = AppHelper.LicenseStatGet(licenseFileModel, isAddSelf: false);

            txtEmail.Text = licenseFileModel.Email;
            txtMac.Text = MacHelper.GetMacAddress();
            txtCPU.Text = Win32Helper.GetCpuID();
            if (!licenseFileModel.IsTrial)
            {
                txtPath.Text = "license.key";
            }
        }

        private void btnOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = false;
            dlg.DefaultExt = ".key";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                txtPath.Text = dlg.FileName;
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ControlUtil.ShowMsg("请输入邮箱地址");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPath.Text))
            {
                ControlUtil.ShowMsg("请选择授权文件");
                return;
            }

            string email = txtEmail.Text.Trim(), mac = MacHelper.GetMacAddress(), cpu = Win32Helper.GetCpuID();
            var licPath = txtPath.Text.Trim();

            try
            {
                licenseFileModel = AppHelper.LicenseFileGet(licPath);
                licenseStatModel = AppHelper.LicenseStatGet(licenseFileModel, isAddSelf: false);
                licenseStatModel.Identification = licenseFileModel.Email;

                if (AppHelper.LicenseVerify(licenseFileModel, licenseStatModel))
                {
                    AppHelper.LicenseStatReset(licenseFileModel);
                }
                //替换文件
                var destPath = string.Format("{0}/data/license.key", AppDomain.CurrentDomain.BaseDirectory).ToLower();
                File.Copy(licPath, destPath, true);
                if (MessageBox.Show("授权成功，重启程序生效", "授权提示", MessageBoxButton.OK, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    System.Windows.Forms.Application.Restart();
                    Application.Current.Shutdown();
                    return;
                }
            }
            catch (LicenseException ex)
            {
                ControlUtil.ShowMsg(ExceptionHelper.GetMessage(ex), "授权提示", icon: MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                ControlUtil.ShowMsg(ExceptionHelper.GetMessage(ex), "授权提示", icon: MessageBoxImage.Error);
            }
        }
    }
}
