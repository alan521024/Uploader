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
    /// FileView.xaml 的交互逻辑
    /// </summary>
    public partial class FileView : DxWindow
    {
        /// <summary>
        /// FTP信息
        /// </summary>
        public FTPClientUtility FtpUtil { get; set; }

        public FileView(FTPClientUtility ftpUtil)
        {
            InitializeComponent();
            FtpUtil = ftpUtil;
            WinInit();
        }

        private void btnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbCurrentDir.Text) ||
                tbCurrentDir.Text == "/" ||
                tbCurrentDir.Text.Trim().ToLower() == FtpUtil.ClientModel.Directory.Trim().ToLower())
                return;

            var pathArr = tbCurrentDir.Text.Split('/');
            if (pathArr.Length > 0)
            {
                string newPath = "";
                for (var i = 0; i < pathArr.Length - 1; i++)
                {
                    if (!string.IsNullOrWhiteSpace(pathArr[i]))
                    {
                        newPath += pathArr[i] + "/";
                    }
                }
                newPath=string.Format("/{0}", newPath).Replace("//", "/").TrimEnd('/').Trim();
                tbCurrentDir.Text = (newPath == "") ? "/" : newPath;
                BindSource(tbCurrentDir.Text);
            }

        }

        private void btnAddFolder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            FtpItem obj = lvSource.SelectedItem as FtpItem;
            if (obj == null || (obj != null && obj.Type == "文件"))
            {
                ControlUtil.ShowMsg("请选择目录");
                return;
            }
        }

        private void btnRenameFolder_Click(object sender, RoutedEventArgs e)
        {
            FtpItem obj = lvSource.SelectedItem as FtpItem;
            if (obj == null || (obj != null && obj.Type == "文件"))
            {
                ControlUtil.ShowMsg("请选择目录");
                return;
            }
        }

        private void btnSetCurrentPath_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lvSource_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FtpItem obj = lvSource.SelectedItem as FtpItem;
            if (obj == null || (obj != null && obj.Type == "文件"))
            {
                return;
            }
            tbCurrentDir.Text = string.Format("{0}/{1}", tbCurrentDir.Text, obj.Name).Replace("//", "/").Trim();
            BindSource(tbCurrentDir.Text);
        }

        private void WinInit()
        {
            if (FtpUtil == null)
                return;
            tbCurrentDir.Text = FtpUtil.ClientModel.Directory;
            BindSource(tbCurrentDir.Text);
        }

        private void BindSource(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            var list = FtpUtil.GetServerList(path);

            ControlUtil.ExcuteAction(this, () =>
            {
                lvSource.ItemsSource = FileStructToFtpItems(path, list);
            });
        }

        private List<FtpItem> FileStructToFtpItems(string path, List<FileStruct> source)
        {
            var list = new List<FtpItem>();
            if (source != null && source.Count() > 0)
            {
                foreach (var item in source)
                {
                    list.Add(new FtpItem()
                    {
                        Name = item.Name,
                        Type = item.IsDirectory ? "文件夹" : "文件",
                        Path = string.Format("{0}/{1}", path, item.Name).Replace("//", "/"),
                        Size = item.IsDirectory ? "" : AppHelper.CountSize(item.Size),
                        ImgPath = getIconPath(item.Name, isDirectory: item.IsDirectory)
                    });
                }
                list = list.OrderByDescending(x => x.Type).ThenBy(x => x.Name).ToList();
            }
            return list;
        }

        string getIconPath(string file, bool isDirectory = false)
        {
            if (isDirectory)
                return "pack://application:,,,/Icons/FolderOff.png";

            string format = System.IO.Path.GetExtension(file);//获取文件格式

            switch (format)
            {
                case ".txt":
                    return "pack://application:,,,/Icons/txt.png";
                case ".exe":
                    return "pack://application:,,,/Icons/exe.png";
                case ".pdf":
                    return "pack://application:,,,/Icons/pdf.png";
                case ".mp3":
                case ".wav":
                case ".au":
                case ".midi":
                case ".wma":
                case ".aac":
                case ".ape":
                case ".cda":
                case ".aiff":
                    return "pack://application:,,,/Icons/music.png";
                case ".avi":
                case ".nvai":
                case ".mp4":
                case ".rmvb":
                case ".mpg":
                    return "pack://application:,,,/Icons/video.png";
                case ".zip":
                case ".iso":
                case ".rar":
                    return "pack://application:,,,/Icons/zip.png";
                case ".docx":
                case ".doc":
                    return "pack://application:,,,/Icons/docx.png";
                case ".xslx":
                case ".xlsx":
                case ".xsl":
                    return "pack://application:,,,/Icons/excel.png";
                case ".png":
                case ".jpg":
                case ".ico":
                    return "pack://application:,,,/Icons/image.png";
                case ".cs":
                case ".sln":
                case ".xaml":
                    return "pack://application:,,,/Icons/code_file.ico";
                case ".dll":
                    return "pack://application:,,,/Icons/dll.png";
                case "":
                    return "pack://application:,,,/Icons/unknown.png";
                default:
                    return "pack://application:,,,/Icons/unknown.png";

            }
        }

    }
}
