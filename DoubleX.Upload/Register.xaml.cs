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
using System.Drawing;

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
            ViewPageInit();
            LoadingLicense();
        }

        private void imgCaptcha_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BindCaptchaCode();
        }

        private void swCaptch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                ChangeViewType();
            }
        }

        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ControlUtil.ShowMsg("请输入邮箱地址");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMobile.Text))
            {
                ControlUtil.ShowMsg("请输入手机号码");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                ControlUtil.ShowMsg("请输入验证码");
                return;
            }
            if (txtCode.Text != CaptchaCode)
            {
                ControlUtil.ShowMsg("验证码错误");
                return;
            }

            ChangeViewType();

            var licPath = string.Format("{0}/data/license.key", AppDomain.CurrentDomain.BaseDirectory).ToLower();
            licenseFileModel = AppHelper.LicenseFileGet(licPath);
            var config = AppHelper.GetConfig();
            string buyParam = string.Format("email={0}&mobile={1}&mac={2}&cpu={3}&code={4}&businesser={5}&=edition={6}",
                txtEmail.Text.ToLower(), txtMobile.Text.ToLower(), MacHelper.GetMacAddress(), Win32Helper.GetCpuID(), 
                txtCode.Text, config.Businesser, licenseFileModel.Edition);
            string buyUrl = string.Format("{0}/{1}", config.BuyUrl, UrlsHelper.Encode(buyParam));
            System.Diagnostics.Process.Start("explorer.exe", buyUrl);
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

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail2.Text))
            {
                ControlUtil.ShowMsg("请输入邮箱地址");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMobile2.Text))
            {
                ControlUtil.ShowMsg("请输入手机号码");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPath.Text))
            {
                ControlUtil.ShowMsg("请选择授权文件");
                return;
            }

            string email = txtEmail2.Text.Trim(), mobile=txtMobile2.Text.Trim(), mac = MacHelper.GetMacAddress(), cpu = Win32Helper.GetCpuID();
            var licPath = txtPath.Text.Trim();

            try
            {
                licenseFileModel = AppHelper.LicenseFileGet(licPath);
                licenseStatModel = AppHelper.LicenseStatGet(licenseFileModel, isAddSelf: false);
                licenseStatModel.Identification = licenseFileModel.Email;
                licenseStatModel.Mobile = licenseFileModel.Mobile;

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

        #region 私有变量

        /// <summary>
        /// 授权信息
        /// </summary>
        private LicenseFileModel licenseFileModel { get; set; }

        /// <summary>
        /// 授权统计
        /// </summary>
        private LicenseStatModel licenseStatModel { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        private string CaptchaCode { get; set; }

        /// <summary>
        /// 视图类型
        /// </summary>
        private string ViewType { get; set; }

        #endregion

        #region 辅助方法

        protected void ViewPageInit()
        {
            ViewType = "buy";
            BindCaptchaCode();
        }

        /// <summary>
        /// 加载现有授权信息
        /// </summary>
        protected void LoadingLicense()
        {
            var licPath = string.Format("{0}/data/license.key", AppDomain.CurrentDomain.BaseDirectory).ToLower();
            licenseFileModel = AppHelper.LicenseFileGet(licPath);
            licenseStatModel = AppHelper.LicenseStatGet(licenseFileModel, isAddSelf: false);
        }

        protected void BindCaptchaCode()
        {
            ControlUtil.ExcuteAction(this, () =>
            {
                CaptchaCode = StringRandHelper.Number(4);
                imgCaptcha.Source = CaptchaHelper.CreateBitmapImage(CaptchaCode);
            });
        }

        protected void ChangeViewType()
        {
            if (ViewType == "buy")
            {
                ViewType = "import";
                grdBuy.Visibility = Visibility.Collapsed;
                grdImport.Visibility = Visibility.Visible;
                txtEmail2.Text = txtEmail.Text.ToLower();
                txtMobile2.Text = txtMobile.Text.ToLower();
            }
            else
            {
                ViewType = "buy";
                BindCaptchaCode();
                grdBuy.Visibility = Visibility.Visible;
                grdImport.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

    }
}
