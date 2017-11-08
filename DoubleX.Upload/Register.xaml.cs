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

        #endregion

        private void imgCaptcha_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BindCaptchaCode();
        }

        private void swCaptch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                if (ViewType == "buy")
                {
                    ViewType = "import";
                    grdBuy.Visibility = Visibility.Collapsed;
                    grdImport.Visibility = Visibility.Visible;
                }
                else
                {
                    ViewType = "buy";
                    grdBuy.Visibility = Visibility.Visible;
                    grdImport.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
