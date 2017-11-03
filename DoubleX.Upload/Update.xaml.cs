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
    /// Update.xaml 的交互逻辑
    /// </summary>
    public partial class Update : DxWindow
    {
        /// <summary>
        /// 版本信息
        /// </summary>
        public VersionModel versionModel { get; set; }

        public Update(VersionModel _versionModel)
        {
            InitializeComponent();
            this.versionModel = _versionModel;

            lbCurrentVersion.Content = string.Format("当前版本：{0}", versionModel.CurrentVersion);
            lbLastVersion.Content = string.Format("最新版本：{0}", versionModel.LastVersion);

            tbDownloadUrl.Text = versionModel.DownloadUrl;

            if (versionModel.Incremental) {
                lbNote.Content = "（注：必须更新为最新版本，程序才能正常使用）";
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (versionModel.Incremental)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
