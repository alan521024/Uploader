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
using System.Security.Cryptography;
using Microsoft.Win32;
using WinForm = System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DoubleX.Infrastructure.Utility;
using DoubleXUI.Controls;

namespace DoubleX.Upload
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Main : DxWindow
    {
        #region 信息属性

        /// <summary>
        /// FTP信息
        /// </summary>
        private FTPClientUtility ftpUtil;

        /// <summary>
        /// 系统参数(小写)
        /// </summary>
        private string[] systemParam { get { return new string[] { "id", "filefullpath", "filesize", "serverfilefullpath" }; } }

        /// <summary>
        /// 信息参数(FTP发送前)
        /// </summary>
        private List<RequestParamModel> beforeParam { get; set; }

        /// <summary>
        /// 信息参数(FTP发送后)
        /// </summary>
        public List<RequestParamModel> afterParam { get; set; }


        /// <summary>
        /// 任务数据源
        /// </summary>
        private List<TaskPathModel> taskPathSource { get; set; }

        /// <summary>
        /// 日志文本框滚动条是否在最下方
        /// true:文本框竖直滚动条在文本框最下面时，可以在文本框后追加日志
        /// false:当用户拖动文本框竖直滚动条，使其不在最下面时，即用户在查看旧日志，此时不添加新日志，
        /// </summary>
        public bool IsVerticalScrollBarAtBottom
        {
            get
            {
                //bool atBottom = false;

                //this.txtLog.Dispatcher.Invoke((Action)delegate
                //{
                //    //if (this.txtLog.VerticalScrollBarVisibility != ScrollBarVisibility.Visible)
                //    //{
                //    //    atBottom= true;
                //    //    return;
                //    //}
                //    double dVer = this.txtLog.VerticalOffset;       //获取竖直滚动条滚动位置
                //    double dViewport = this.txtLog.ViewportHeight;  //获取竖直可滚动内容高度
                //    double dExtent = this.txtLog.ExtentHeight;      //获取可视区域的高度

                //    if (dVer + dViewport >= dExtent)
                //    {
                //        atBottom = true;
                //    }
                //    else
                //    {
                //        atBottom = false;
                //    }
                //});

                //return atBottom;
                return false;
            }
        }

        /// <summary>
        /// 授权信息
        /// </summary>
        public LicModel RegisterModel { get; set; }

        //上传任务
        private volatile bool uploadIsStop;
        private System.Threading.Thread uploadThread = null;

        #endregion

        public Main()
        {
            InitializeComponent();
            Loading();

            beforeParam = new List<RequestParamModel>();
            afterParam = new List<RequestParamModel>();
            InitPostParams();
            BindTaskPathSource();
            BindTaskList();
        }

        #region FTP连接/断开/浏览/注册

        private void btnConnectOpen_Click(object sender, RoutedEventArgs e)
        {
            ftpUtil = new FTPClientUtility(txtAddress.Text, txtName.Text, txtPassword.Text, IntHelper.Get(txtPort.Text), txtDirectory.Text);
            ControlUtil.ExcuteAction(this, () =>
            {
                SetFTPControlStatus(false);
            });
            try
            {
                WriteLog(string.Format("正在连接FTP：地址：{0} {1}：{2} 登录名：{3}", txtAddress.Text, txtDirectory.Text, txtPort.Text, txtName.Text));
                ftpUtil.Open();
                WriteLog(string.Format("FTP连接成功：地址：{0} {1}：{2}", txtAddress.Text, txtDirectory.Text, txtPort.Text), UILogType.Success);

                ControlUtil.ExcuteAction(this, () =>
                {
                    SetFTPControlStatus(true);
                });
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("FTP连接失败：{0}", ExceptionHelper.GetMessage(ex)), UILogType.Error);

                ControlUtil.ExcuteAction(this, () =>
                {
                    SetFTPControlStatus(false);
                });
            }
        }

        private void btnConnectClose_Click(object sender, RoutedEventArgs e)
        {
            ftpUtil = null;
            ControlUtil.ExcuteAction(this, () =>
            {
                SetFTPControlStatus(false);
            });
            WriteLog(string.Format("FTP连接断开：地址：{0} {1}：{2}", txtAddress.Text, txtDirectory.Text, txtPort.Text), UILogType.Warning);
        }

        private void btnFTPServerView_Click(object sender, RoutedEventArgs e)
        {
            FileView win = new FileView(ftpUtil);
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;// FormStartPosition.CenterParent;
            win.Show();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Register win = new Register();
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;// FormStartPosition.CenterParent;
            win.Show();
        }

        #endregion

        #region 文件/文件夹选择事件

        private void btnOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            tabMain.SelectedIndex = 0;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            //dlg.DefaultExt = ".txt";
            //dlg.Filter = "Text documents (.txt)|*.txt";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                //单文件选择
                //BindTaskPathSource(EnumPathType.文件, dlg.FileName);

                //多文件选择
                foreach (var item in dlg.FileNames)
                {
                    BindTaskPathSource(EnumPathType.文件, item);
                }
            }
        }

        private void btnOpenFolderDialog_Click(object sender, RoutedEventArgs e)
        {
            tabMain.SelectedIndex = 0;
            WinForm.FolderBrowserDialog m_Dialog = new WinForm.FolderBrowserDialog();
            WinForm.DialogResult result = m_Dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            var folderPath = m_Dialog.SelectedPath.ToLower().Trim();
            BindTaskPathSource(EnumPathType.文件夹, folderPath);

            //new Thread(() =>
            //{
            //    var model = taskPathSource.FirstOrDefault(x => x.ItemPath.ToLower() == folderPath);
            //    CalculateTaskPathCount(model, folderPath);
            //    model.IsStatistical = true;
            //    ControlUtil.DataGridSyncBinding(gridTaskPathList, taskPathSource);
            //}).Start();

            Thread thread = new Thread(new ThreadStart(() =>
            {
                var model = taskPathSource.FirstOrDefault(x => x.ItemPath.ToLower() == folderPath);
                CalculateTaskPathCount(model, folderPath);
                model.IsStatistical = true;
                ControlUtil.ExcuteAction(this, () =>
                {
                    ControlUtil.DataGridSyncBinding(gridTaskPathList, taskPathSource);
                });
                System.Windows.Threading.Dispatcher.Run();
            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

        }

        private void gridTaskPathDelete_Click(object sender, RoutedEventArgs e)
        {
            if (gridTaskPathList.SelectedItem != null)
            {
                TaskPathModel current = gridTaskPathList.SelectedItem as TaskPathModel;
                taskPathSource.Remove(taskPathSource.Find(x => x.ItemPath == current.ItemPath));
                ControlUtil.ExcuteAction(this, () =>
                {
                    ControlUtil.DataGridSyncBinding(gridTaskPathList, taskPathSource);
                });
            }
        }


        #endregion

        #region 任务(开始，结束，进度、结果)

        private void btnTaskRunning_Click(object sender, RoutedEventArgs e)
        {
            if (taskPathSource == null || (taskPathSource != null && taskPathSource.Count() == 0))
            {
                ControlUtil.ShowMsg("请选择 文件夹 或 文件");
                return;
            }
            if (taskPathSource.Count(x => !x.IsStatistical) > 0)
            {
                ControlUtil.ShowMsg("正在计算文件夹文件数");
                return;
            }
            if (ftpUtil == null || (ftpUtil != null && !ftpUtil.IsConnection))
            {
                ControlUtil.ShowMsg("FTP信息 未连接 或 连接失败");
                return;
            }

            MessageBoxResult dr = ControlUtil.ShowMsg("任务运行其间，不允许期它操作，是否开始任务？", btn: MessageBoxButton.OKCancel, icon: MessageBoxImage.Question);
            if (dr == MessageBoxResult.OK)
            {
                var beforeModel = GetBeforeModel();
                var afterModel = GetAfterModel();
                var databaseModel = GetDatabaseSettingModel();
                var setting = GetTaskSettingModel();
                string taskId = Guid.NewGuid().ToString().ToLower();
                uploadThread = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        SetCurrentRunningUI();
                        StartNewTask(taskId, beforeModel, afterModel, databaseModel, setting);
                        System.Windows.Threading.Dispatcher.Run();
                    }
                    catch (ThreadAbortException ex)
                    {
                        //移除任务
                        DeleteTask(taskId);
                        ControlUtil.ShowMsg("任务中止");
                    }
                    catch (Exception ex)
                    {
                        string exMsg = ExceptionHelper.GetMessage(ex);

                        //设置任务状态
                        UpdateTaskStatus(taskId, EnumTaskStatus.己中止);

                        //状态栏
                        WriteStatus("上传出错，任务中止");

                        //写日志
                        WriteLog(string.Format("上传出错，任务中止({0}) {1}", taskId, exMsg), UILogType.Error);

                        ControlUtil.ShowMsg(exMsg, "错误", icon: MessageBoxImage.Error);
                    }
                    finally
                    {
                        //同步任务
                        BindTaskList();

                        //设置结束
                        SetCurrentFinishUI();
                    }
                }));
                //uploadThread.SetApartmentState(ApartmentState.STA);
                //uploadThread.IsBackground = true;
                uploadThread.Start();

            }
        }

        private void btnTaskStop_Click(object sender, RoutedEventArgs e)
        {
            StopTask();
            SetCurrentFinishUI();
        }

        #endregion

        #region 上传记录历史记录

        private void btnClearTask_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = System.Windows.MessageBox.Show("是否在清空操作记录(任务+上传)", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.OK)
            {
                StopTask();
                DeleteTask(null);
                SetCurrentFinishUI();
            }
        }

        private void btnClearLog_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = System.Windows.MessageBox.Show("是否在清空操作日志", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.OK)
            {
                ControlUtil.ExcuteAction(this, () =>
                {
                    spLoggin.Children.Clear();
                });
            }
        }

        private void gridTaskList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGrid grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    var model = grid.SelectedItem as TaskEntity;
                    if (model == null)
                        return;

                    TaskView win = new TaskView(model);
                    win.Owner = this;
                    win.WindowStartupLocation = WindowStartupLocation.CenterOwner;// FormStartPosition.CenterParent;
                    win.Show();
                }
            }
        }

        #endregion

        #region 参数设置相关操作

        private void btnConnectionHelper_Click(object sender, RoutedEventArgs e)
        {
            DatabaseHelper win = new DatabaseHelper();
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;// FormStartPosition.CenterParent;
            win.Show();
        }


        private void chkBeforeEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            //chkBeforeErrorContinue.IsChecked = false;
        }

        private void btnPostBeforeAdd_Click(object sender, RoutedEventArgs e)
        {
            if (beforeParam.Count(x => x.Name.ToLower() == "") > 0)
            {
                ControlUtil.ShowMsg("请先设置参数Key不为''后再添加");
                return;
            }

            ControlUtil.ExcuteAction(this, () =>
            {
                beforeParam.Add(new RequestParamModel()
                {
                    Id = Guid.NewGuid(),
                    Name = "",
                    PType = "Value",
                    Descript = "",
                    DefaultValue = "",
                    IsCanDelete = true
                });
                ControlUtil.DataGridSyncBinding(gridRequestBefore, beforeParam);
            });
        }

        private void gridRequestBefore_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            RequestParamModel oldModel = gridRequestBefore.SelectedItem as RequestParamModel;
            TextBox editingElement = e.EditingElement as TextBox;
            string newValue = string.Empty;
            string field = ((Binding)(e.Column as DataGridBoundColumn).Binding).Path.Path;
            if (editingElement != null)
            {
                newValue = editingElement.Text;
            }

            if (newValue != null && oldModel != null)
            {
                //系统参数，还原
                if (systemParam.Contains(oldModel.Name.ToLower()))
                {
                    switch (field.ToLower())
                    {
                        case "name":
                            editingElement.Text = oldModel.Name;
                            break;
                        case "ptype":
                            editingElement.Text = oldModel.PType;
                            break;
                        case "descript":
                            editingElement.Text = oldModel.Descript;
                            break;
                        case "defaultvalue":
                            editingElement.Text = oldModel.DefaultValue;
                            break;
                    }
                    ControlUtil.ShowMsg("系统参数不允许修改");
                    return;
                }

                //重复键，还原
                if (field.ToLower() == "name" && beforeParam.Count(x => x.Name.ToLower() == newValue.ToLower()) > 0)
                {
                    editingElement.Text = oldModel.Name;
                    ControlUtil.ShowMsg("己存在重复信息键Key");
                    return;
                }
            }
        }

        private void gridRequestBeforeDelete_Click(object sender, RoutedEventArgs e)
        {
            if (gridRequestBefore.SelectedItem != null)
            {
                RequestParamModel current = gridRequestBefore.SelectedItem as RequestParamModel;
                if (current != null && systemParam.Contains(current.Name.ToLower()))
                {

                    ControlUtil.ShowMsg("系统参数不允许删除");
                    return;
                }
                else
                {
                    beforeParam.Remove(beforeParam.Find(x => x.Id == current.Id));
                    ControlUtil.ExcuteAction(this, () =>
                    {
                        ControlUtil.DataGridSyncBinding(gridRequestBefore, beforeParam);
                    });
                }
            }
        }



        private void chkAfterEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            //chkAfterErrorContinue.IsChecked = false;
        }

        private void btnPostAfterAdd_Click(object sender, RoutedEventArgs e)
        {
            if (afterParam.Count(x => x.Name.ToLower() == "") > 0)
            {
                ControlUtil.ShowMsg("请先设置参数Key不为''后再添加");
                return;
            }

            ControlUtil.ExcuteAction(this, () =>
            {
                afterParam.Add(new RequestParamModel()
                {
                    Id = Guid.NewGuid(),
                    Name = "",
                    PType = "Return",
                    Descript = "",
                    DefaultValue = "",
                    IsCanDelete = true
                });
                ControlUtil.DataGridSyncBinding(gridRequestAfter, afterParam);
            });
        }

        private void gridRequestAfter_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            RequestParamModel oldModel = gridRequestAfter.SelectedItem as RequestParamModel;
            TextBox editingElement = e.EditingElement as TextBox;
            string newValue = string.Empty;
            string field = ((Binding)(e.Column as DataGridBoundColumn).Binding).Path.Path;
            if (editingElement != null)
            {
                newValue = editingElement.Text;
            }

            if (newValue != null && oldModel != null)
            {
                //系统参数，还原
                if (systemParam.Contains(oldModel.Name.ToLower()))
                {
                    switch (field.ToLower())
                    {
                        case "name":
                            editingElement.Text = oldModel.Name;
                            break;
                        case "ptype":
                            editingElement.Text = oldModel.PType;
                            break;
                        case "descript":
                            editingElement.Text = oldModel.Descript;
                            break;
                        case "defaultvalue":
                            editingElement.Text = oldModel.DefaultValue;
                            break;
                    }
                    ControlUtil.ShowMsg("系统参数不允许修改");
                    return;
                }

                //重复键，还原
                if (field.ToLower() == "name" && afterParam.Count(x => x.Name.ToLower() == newValue.ToLower()) > 0)
                {
                    editingElement.Text = oldModel.Name;
                    ControlUtil.ShowMsg("己存在重复信息键Key");
                    return;
                }
            }
        }

        private void gridRequestAfterDelete_Click(object sender, RoutedEventArgs e)
        {
            if (gridRequestAfter.SelectedItem != null)
            {
                RequestParamModel current = gridRequestAfter.SelectedItem as RequestParamModel;
                if (current != null && systemParam.Contains(current.Name.ToLower()))
                {
                    ControlUtil.ShowMsg("系统参数不允许删除");
                    return;
                }
                else
                {
                    afterParam.Remove(afterParam.Find(x => x.Id == current.Id));
                    ControlUtil.ExcuteAction(this, () =>
                    {
                        ControlUtil.DataGridSyncBinding(gridRequestAfter, afterParam);
                    });
                }
            }
        }

        #endregion

        #region 数据库发布操作

        private void btnScriptExcute_Click(object sender, RoutedEventArgs e)
        {
            var connectionString = txtConnectionStr.Text;
            var sql = txtSql.Text;
            if (VerifyHelper.IsEmpty(connectionString))
            {
                ControlUtil.ShowMsg("连接字符串不能为空");
                return;
            }
            if (VerifyHelper.IsEmpty(sql))
            {
                ControlUtil.ShowMsg("执行SQL语句不能为空");
                return;
            }

            int result = 0;

            try
            {
                if (raSQLserver.IsChecked.Value)
                {
                    result = SQLExecute("sqlserver", connectionString, sql);
                }
                if (raMySql.IsChecked.Value)
                {
                    result = SQLExecute("mysql", connectionString, sql);
                }
                if (raOracle.IsChecked.Value)
                {
                    result = SQLExecute("oracle", connectionString, sql);
                }
                if (raSQLite.IsChecked.Value)
                {
                    result = SQLExecute("sqlite", connectionString, sql);
                }
            }
            catch (Exception ex)
            {
                ControlUtil.ShowMsg(string.Format("出现错误：{0}", ExceptionHelper.GetMessage(ex)));
                return;
            }
            ControlUtil.ShowMsg(string.Format("执行成功：影响行数 {0}", result));

        }

        #endregion


        #region 辅助方法-任务参数

        /// <summary>
        /// 初始参数
        /// </summary>
        private void InitPostParams()
        {
            //请求前数据
            var beforeList = new List<RequestParamModel>();
            beforeList.Add(new RequestParamModel()
            {
                Id = Guid.NewGuid(),
                Name = "Id",
                PType = "Guid",
                Descript = "上传文件的唯一标识",
                DefaultValue = "",
                IsCanDelete = false
            });
            beforeList.Add(new RequestParamModel()
            {
                Id = Guid.NewGuid(),
                Name = "FileFullPath",
                PType = "string",
                Descript = "文件完整路径(含路径+文件名)",
                DefaultValue = "",
                IsCanDelete = false
            });
            beforeList.Add(new RequestParamModel()
            {
                Id = Guid.NewGuid(),
                Name = "FileMD5",
                PType = "string",
                Descript = "文件MD5",
                DefaultValue = "",
                IsCanDelete = false
            });
            beforeList.Add(new RequestParamModel()
            {
                Id = Guid.NewGuid(),
                Name = "ServerFileFullPath",
                PType = "string",
                Descript = "服务器文件完整路径",
                DefaultValue = "",
                IsCanDelete = false
            });
            beforeList.Add(new RequestParamModel()
            {
                Id = Guid.NewGuid(),
                Name = "FileSize",
                PType = "long",
                Descript = "文件大小(KB)",
                DefaultValue = "",
                IsCanDelete = false
            });
            beforeParam = beforeList;
            gridRequestBefore.ItemsSource = beforeList;

            //请求后数据
            var afterList = new List<RequestParamModel>();
            afterList.Add(new RequestParamModel()
            {
                Id = Guid.NewGuid(),
                Name = "Id",
                PType = "Guid",
                Descript = "上传文件的唯一标识",
                DefaultValue = "",
                IsCanDelete = false
            });
            afterList.Add(new RequestParamModel()
            {
                Id = Guid.NewGuid(),
                Name = "FileFullPath",
                PType = "string",
                Descript = "文件完整路径(含路径+文件名)",
                DefaultValue = "",
                IsCanDelete = false
            });
            afterList.Add(new RequestParamModel()
            {
                Id = Guid.NewGuid(),
                Name = "FileMD5",
                PType = "string",
                Descript = "文件MD5",
                DefaultValue = "",
                IsCanDelete = false
            });
            afterList.Add(new RequestParamModel()
            {
                Id = Guid.NewGuid(),
                Name = "ServerFileFullPath",
                PType = "string",
                Descript = "服务器文件完整路径",
                DefaultValue = "",
                IsCanDelete = false
            });
            afterList.Add(new RequestParamModel()
            {
                Id = Guid.NewGuid(),
                Name = "FileSize",
                PType = "long",
                Descript = "文件大小(KB)",
                DefaultValue = "",
                IsCanDelete = false
            });
            afterParam = afterList;
            gridRequestAfter.ItemsSource = afterList;
        }

        /// <summary>
        /// 请求前参数
        /// </summary>
        /// <returns></returns>
        private RequestModel GetBeforeModel()
        {
            RequestModel model = new RequestModel();
            model.IsEnable = chkBeforeEnabled.IsChecked == true ? true : false;
            model.Url = txtBeforeUrl.Text;
            model.Params = beforeParam;
            return model;
        }

        /// <summary>
        /// 请求后参数
        /// </summary>
        /// <returns></returns>
        private RequestModel GetAfterModel()
        {
            RequestModel model = new RequestModel();
            model.IsEnable = chkAfterEnabled.IsChecked == true ? true : false;
            model.Url = txtAfterUrl.Text;
            model.Params = afterParam;
            return model;
        }

        /// <summary>
        /// 数据库配置
        /// </summary>
        /// <returns></returns>
        public DatabaseSettingModel GetDatabaseSettingModel()
        {
            DatabaseSettingModel model = new DatabaseSettingModel();
            model.IsEnable = chkDatabaseEnabled.IsChecked == true ? true : false;
            model.DBType = "";
            if (raSQLserver.IsChecked.Value)
            {
                model.DBType = "sqlserver";
            }
            if (raMySql.IsChecked.Value)
            {
                model.DBType = "mysql";
            }
            if (raSQLite.IsChecked.Value)
            {
                model.DBType = "sqlite";
            }
            model.ConnectionStr = txtConnectionStr.Text;
            model.SQL = txtSql.Text;
            return model;
        }

        /// <summary>
        /// 任务设置配置
        /// </summary>
        /// <returns></returns>
        private TaskSettingModel GetTaskSettingModel()
        {
            TaskSettingModel model = new TaskSettingModel();
            model.IsBefore = (chkBeforeEnabled.IsChecked == true ? true : false);
            model.IsAfter = (chkAfterEnabled.IsChecked == true ? true : false);
            model.IsErrorGoOn = (chkIsErrorGoOn.IsChecked == true ? true : false);
            return model;
        }

        #endregion

        #region 辅助方法-任务操作

        /// <summary>
        /// 设置当前界面运行状态（禁用控件）
        /// </summary>
        private void SetCurrentRunningUI()
        {
            ControlUtil.ExcuteAction(this, () =>
            {
                btnTaskRunning.Visibility = Visibility.Collapsed;
                btnTaskStop.Visibility = Visibility.Visible;
            });
        }

        /// <summary>
        /// 设置当前界面结束状态（启用控件）
        /// </summary>
        private void SetCurrentFinishUI()
        {
            BindTaskList();

            ControlUtil.ExcuteAction(this, () =>
            {
                btnTaskRunning.Visibility = Visibility.Visible;
                btnTaskStop.Visibility = Visibility.Collapsed;
                tbStatus.Text = "未有任务运行....";
            });
        }

        /// <summary>
        ///  开始一个新任务
        /// </summary>
        private void StartNewTask(string taskId, RequestModel beforeModel, RequestModel afterModel, DatabaseSettingModel databaseSettingModel, TaskSettingModel setting)
        {
            //线程信号
            uploadIsStop = false;

            #region 增加任务数据

            WriteLog("正在创建任务");
            WriteStatus("正在创建任务");

            var taskEntity = new TaskEntity();
            taskEntity.Id = taskId;
            taskEntity.TaskName = DateTime.Now.ToString("yyyy-MM-dd HHmmsss");
            taskEntity.PathJSON = JsonHelper.Serialize(taskPathSource);
            taskEntity.BeforeJSON = JsonHelper.Serialize(beforeModel);
            taskEntity.AfterJSON = JsonHelper.Serialize(afterModel);
            taskEntity.SettingJSON = JsonHelper.Serialize(setting);
            taskEntity.DBSettingJSON = JsonHelper.Serialize(databaseSettingModel);
            taskEntity.FileTotal = taskPathSource.Sum(x => x.ItemCount);
            taskEntity.SuccessTotal = 0;
            taskEntity.ErrorTotal = 0;
            taskEntity.ErrorJSON = JsonHelper.Serialize(new List<TaskErrorModel>());
            taskEntity.Status = (int)EnumTaskStatus.未开始;
            taskEntity.CreateDt = DateTime.Now;
            if (InsertTask(taskEntity) == 0)
            {
                WriteLog(string.Format("任务创建失败({0})", taskEntity.TaskName), UILogType.Error);
                return;
            }
            else
            {
                WriteLog(string.Format("任务创建成功({0})", taskEntity.TaskName), UILogType.Success);
            }

            #endregion

            #region 增加待上传文件数据

            WriteLog("正在添加文件数据信息");
            WriteStatus("正在添加文件数据信息");

            string destDbPath = CopyFileLogTempateDB(taskId);
            long optFileTotal = IsertTaskPaths(destDbPath, taskEntity, taskPathSource);

            if (optFileTotal == taskEntity.FileTotal)
            {
                setting.FileDatabasePath = destDbPath.Replace(AppHelper.DatabasePath, "");
                taskEntity.SettingJSON = JsonHelper.Serialize(setting);
                UpdateTaskSetting(taskEntity);
                WriteLog(string.Format("文件数据添加成功 ({0})", taskEntity.TaskName), UILogType.Success);
            }
            else
            {
                WriteLog(string.Format("文件数据添加失败 {0}/{1} ({2})", taskEntity.FileTotal, optFileTotal, taskEntity.TaskName), UILogType.Error);
                return;
            }

            #endregion

            #region 同步任务

            BindTaskList();

            #endregion

            #region 上传文件操作

            WriteLog("正在上传文件");
            WriteStatus("正在上传文件");

            //设置任务状态
            UpdateTaskStatus(taskEntity.Id, EnumTaskStatus.进行中);

            //数据库操作配置
            DatabaseSettingModel databaseModel = null;
            if (!VerifyHelper.IsEmpty(taskEntity.DBSettingJSON))
            {
                databaseModel = JsonConvert.DeserializeObject<DatabaseSettingModel>(taskEntity.DBSettingJSON);
            }
            int currentUploadFileTotal = 0, currentIndex = 0;
            do
            {
                var table = GetUploadFiles(destDbPath, taskEntity);
                if (table != null)
                {
                    currentUploadFileTotal = currentUploadFileTotal + table.Rows.Count;
                }
                foreach (DataRow row in table.Rows)
                {
                    var taskFileEntity = AppHelper.GetTaskFileEntityByRow(row);
                    if (row != null && taskFileEntity != null && !string.IsNullOrWhiteSpace(taskFileEntity.FilePath))
                    {
                        try
                        {
                            //上传文件前调用接口
                            BeforeApiRequest(destDbPath, taskEntity, taskFileEntity);

                            //上传文件
                            ftpUtil.Upload(taskFileEntity.FilePath, taskFileEntity.ServerFullPath, 200, (current) =>
                            {
                                WriteStatus(string.Format("正在上传文件：{0} ({1}/{2})", taskFileEntity.FileName, current, taskFileEntity.FileSize));
                            });

                            //上传文件后调用接口
                            AfterApiRequest(destDbPath, taskEntity, taskFileEntity);

                            //上传文件后数据库执行
                            if (databaseModel != null && databaseModel.IsEnable)
                            {
                                SQLExecute(databaseModel.DBType, databaseModel.ConnectionStr, ReplaceSqlTag(databaseModel.SQL, taskEntity, taskFileEntity));
                            }

                            //设置文件状态
                            UpdateTaskFileStatus(destDbPath, taskEntity, taskFileEntity, EnumTaskFileStatus.完成);

                            //增加任务成功记录
                            UpdateTaskSuccess(taskEntity, taskFileEntity);

                            //同步任务
                            BindTaskList();
                        }
                        catch (ThreadAbortException ex)
                        {
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            //异常消息
                            string exMsg = ExceptionHelper.GetMessage(ex);

                            //写日志
                            WriteLog(string.Format("文件: {0}({1}) 上传出错", taskFileEntity.FileName, taskFileEntity.Id), UILogType.Error);

                            //设置文件状态
                            UpdateTaskFileStatus(destDbPath, taskEntity, taskFileEntity, EnumTaskFileStatus.出错);

                            //只加错误，继续运行
                            UpdateTaskError(taskEntity, taskFileEntity, setting, exMsg);

                            //同步任务
                            BindTaskList();

                            if (!setting.IsErrorGoOn)
                            {
                                //由外部修改状态,并结束
                                throw new Exception(string.Format("出现错误，任务中止 {0}", exMsg));
                            }
                        }
                    }
                    currentIndex++;
                }
            } while (currentUploadFileTotal < taskEntity.FileTotal);

            string overMsg = string.Format("文件上传完成：总数：{0}，成功：{1}，失败：{2}", taskEntity.FileTotal, taskEntity.SuccessTotal, taskEntity.ErrorTotal);

            //设置任务状态
            UpdateTaskStatus(taskEntity.Id, EnumTaskStatus.己完成);

            //状态栏
            WriteStatus(overMsg);

            //写日志
            WriteLog(string.Format("{0} 任务({1}/{2})", overMsg, taskEntity.Id, taskEntity.TaskName), taskEntity.ErrorTotal > 0 ? UILogType.Error : UILogType.Success);

            //同步任务
            BindTaskList();

            //设置结束
            SetCurrentFinishUI();

            #endregion
        }

        /// <summary>
        /// 结束上传
        /// </summary>
        private void StopTask()
        {
            uploadIsStop = true;
            if (uploadThread != null)
            {
                uploadThread.Abort();
                try
                {
                    while (!((uploadThread.ThreadState & ThreadState.Aborted) != 0 || (uploadThread.ThreadState & ThreadState.AbortRequested) != 0))
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("退出");
                }
                finally
                {
                    uploadThread = null;
                }
            }

        }

        #endregion

        #region 辅助方法-接口调用

        /// <summary>
        /// 上传文件前调用接口
        /// </summary>
        /// <param name="dbPath"></param>
        /// <param name="taskEntity"></param>
        /// <param name="taskFileEntity"></param>
        /// <returns></returns>
        private void BeforeApiRequest(string dbPath, TaskEntity taskEntity, TaskFileEntity taskFileEntity)
        {
            var berforeSetting = JsonHelper.Deserialize<RequestModel>(taskEntity.BeforeJSON);
            if (!berforeSetting.IsEnable)
            {
                return;
            }
            string result = GetHttp(berforeSetting.Url, GetHttpPostByRequestParams(taskEntity, taskFileEntity));
            if (!string.IsNullOrWhiteSpace(result))
            {
                //false对象/false字符串
                if (result.ToLower().StartsWith("false") || result.ToLower().StartsWith("\"false"))
                {
                    throw new Exception("api before result is " + result.ToLower());
                }
                //taskFileEntity.BeforeResult = result;
                UpdateApiBeforeResultObj(dbPath, taskFileEntity, result);
            }
            else
            {
                throw new Exception("api before result is empty");
            }
        }

        /// <summary>
        /// 上传文件后调用接口
        /// </summary>
        /// <param name="dbPath"></param>
        /// <param name="taskEntity"></param>
        /// <param name="taskFileEntity"></param>
        /// <returns></returns>
        private void AfterApiRequest(string dbPath, TaskEntity taskEntity, TaskFileEntity taskFileEntity)
        {
            var afterSetting = JsonHelper.Deserialize<RequestModel>(taskEntity.AfterJSON);
            if (!afterSetting.IsEnable)
            {
                return;
            }
            var resultObj = JsonHelper.Deserialize<JObject>(taskFileEntity.BeforeResult);
            string result = GetHttp(afterSetting.Url, GetHttpPostByRequestParams(taskEntity, taskFileEntity, isAfterParam: true, returnJosnObj: resultObj));
            if (!string.IsNullOrWhiteSpace(result))
            {
                //false对象/false字符串
                if (result.ToLower().StartsWith("false") || result.ToLower().StartsWith("\"false"))
                {
                    throw new Exception("api after result is " + result.ToLower());
                }
            }
            else
            {
                throw new Exception("api after result is empty");
            }
        }

        #endregion

        #region 辅助方法-数据库操作

        /// <summary>
        /// 添加新任务（数据）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private int InsertTask(TaskEntity entity)
        {
            if (entity == null) return 0;

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into TB_Task(");
            strSql.Append("Id,TaskName,PathJSON,BeforeJSON,AfterJSON,SettingJSON,DBSettingJSON,FileTotal,SuccessTotal,ErrorTotal,ErrorJSON,Status,CreateDt)");
            strSql.Append(" values (");
            strSql.Append("@Id,@TaskName,@PathJSON,@BeforeJSON,@AfterJSON,@SettingJSON,@DBSettingJSON,@FileTotal,@SuccessTotal,@ErrorTotal,@ErrorJSON,@Status,@CreateDt)");
            SQLiteParameter[] parameters = {
                    new SQLiteParameter("@Id", DbType.String),
                    new SQLiteParameter("@TaskName", DbType.String),
                    new SQLiteParameter("@PathJSON", DbType.String),
                    new SQLiteParameter("@BeforeJSON",DbType.String),
                    new SQLiteParameter("@AfterJSON", DbType.String),
                    new SQLiteParameter("@SettingJSON", DbType.String),
                    new SQLiteParameter("@DBSettingJSON", DbType.String),
                    new SQLiteParameter("@FileTotal", DbType.Double),
                    new SQLiteParameter("@SuccessTotal", DbType.Double),
                    new SQLiteParameter("@ErrorTotal", DbType.Double),
                    new SQLiteParameter("@ErrorJSON", DbType.String),
                    new SQLiteParameter("@Status", DbType.Int32,4),
                    new SQLiteParameter("@CreateDt", DbType.DateTime)
            };
            parameters[0].Value = entity.Id;
            parameters[1].Value = entity.TaskName;
            parameters[2].Value = entity.PathJSON;
            parameters[3].Value = entity.BeforeJSON;
            parameters[4].Value = entity.AfterJSON;
            parameters[5].Value = entity.SettingJSON;
            parameters[6].Value = entity.DBSettingJSON;
            parameters[7].Value = entity.FileTotal;
            parameters[8].Value = entity.SuccessTotal;
            parameters[9].Value = entity.ErrorTotal;
            parameters[10].Value = entity.ErrorJSON;
            parameters[11].Value = entity.Status;
            parameters[12].Value = DateTime.Now;

            return SQLiteHelper.ExecuteNonQuery(AppHelper.GetTaskDatabaseConnectionStr(), strSql.ToString(), CommandType.Text, parameters);
        }

        /// <summary>
        /// 处理上传的文件文件
        /// </summary>
        /// <param name="taskEntity"></param>
        /// <returns></returns>
        private long IsertTaskPaths(string destDbPath, TaskEntity taskEntity, List<TaskPathModel> pathSource)
        {
            long total = 0;
            foreach (var item in pathSource)
            {
                if (item.ItemType == (int)EnumPathType.文件)
                {
                    InsertTaskFile(destDbPath, taskEntity, item.ItemPath, ref total);
                }
                else if (item.ItemType == (int)EnumPathType.文件夹)
                {
                    InsertTaskFolder(destDbPath, taskEntity, item.ItemPath, ref total);
                }
            }
            return total;
        }

        /// <summary>
        /// 插入任务中的文件夹
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="taskEntity"></param>
        /// <param name="folderPath"></param>
        /// <param name="insertFileTotal"></param>
        private void InsertTaskFolder(string destDbPath, TaskEntity taskEntity, string folderPath, ref long insertFileTotal)
        {
            var folders = Directory.GetDirectories(folderPath);
            var files = Directory.GetFiles(folderPath);
            if (files != null)
            {
                foreach (var item in files)
                {
                    InsertTaskFile(destDbPath, taskEntity, item, ref insertFileTotal);
                }
            }
            foreach (var dir in folders)
            {
                InsertTaskFolder(destDbPath, taskEntity, dir, ref insertFileTotal);
            }

        }

        /// <summary>
        /// 插入任务文件数据
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="taskEntity"></param>
        /// <param name="filePath"></param>
        /// <param name="insertFileTotal"></param>
        private void InsertTaskFile(string destDbPath, TaskEntity taskEntity, string filePath, ref long insertFileTotal)
        {
            long currentTotal = insertFileTotal;

            var fileModel = new FileInfo(filePath);

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into TB_Files(");
            strSql.Append("Id,TaskId,FilePath,FileSize,FileName,FileExtension,ServerFullPath,UpSize,BeforeResult,Status,LastDt)");
            strSql.Append(" values (");
            strSql.Append("@Id,@TaskId,@FilePath,@FileSize,@FileName,@FileExtension,@ServerFullPath,@UpSize,@BeforeResult,@Status,@LastDt)");
            SQLiteParameter[] parameters = {
                    new SQLiteParameter("@Id", DbType.String),
                    new SQLiteParameter("@TaskId", DbType.String),
                    new SQLiteParameter("@FilePath", DbType.String),
                    new SQLiteParameter("@FileSize",DbType.Double),
                    new SQLiteParameter("@FileName", DbType.String),
                    new SQLiteParameter("@FileExtension", DbType.String),
                    new SQLiteParameter("@ServerFullPath", DbType.String),
                    new SQLiteParameter("@UpSize", DbType.Double),
                    new SQLiteParameter("@BeforeResult", DbType.String),
                    new SQLiteParameter("@Status", DbType.Int32,4),
                    new SQLiteParameter("@LastDt", DbType.DateTime)
            };
            parameters[0].Value = Guid.NewGuid().ToString().ToLower();
            parameters[1].Value = taskEntity.Id;
            parameters[2].Value = filePath;
            parameters[3].Value = fileModel.Length;
            parameters[4].Value = fileModel.Name;
            parameters[5].Value = fileModel.Extension;
            parameters[6].Value = GetServerFileFullPath(filePath.Replace(fileModel.Name, ""), fileModel.Name);
            parameters[7].Value = 0;
            parameters[8].Value = "";
            parameters[9].Value = (int)EnumTaskFileStatus.待上传;
            parameters[10].Value = DateTime.Now;

            this.Dispatcher.Invoke(new Action(() =>
            {
                WriteStatus(string.Format("正在处理待上传文件数据：{0}/{1}", taskEntity.FileTotal, (currentTotal + 1)));
            }));

            if (SQLiteHelper.ExecuteNonQuery(AppHelper.GetTaskFileDatabaseConnectionStr(destDbPath), strSql.ToString(), CommandType.Text, parameters) == 0)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    WriteLog(string.Format("文件数据添加失败：{0}({1})", filePath, taskEntity.TaskName), UILogType.Error);
                }));
            }
            insertFileTotal++;
        }


        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <returns></returns>
        private bool UpdateTaskStatus(string taskId, EnumTaskStatus status)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update TB_Task set Status=@Status where Id=@Id");
            SQLiteParameter[] parameters = {
                    new SQLiteParameter("@Id", DbType.String),
                    new SQLiteParameter("@Status", DbType.Int32,4)
            };
            parameters[0].Value = taskId;
            parameters[1].Value = (int)status;

            return SQLiteHelper.ExecuteNonQuery(AppHelper.GetTaskDatabaseConnectionStr(), strSql.ToString(), CommandType.Text, parameters) > 0;
        }

        /// <summary>
        /// 更新任务错误
        /// </summary>
        /// <param name="taskEntity"></param>
        /// <param name="taskFieldEntity"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool UpdateTaskError(TaskEntity taskEntity, TaskFileEntity taskFielEntity, TaskSettingModel setting, string msg)
        {
            var taskErrorString = StringHelper.Get(SQLiteHelper.ExecuteScalar(AppHelper.GetTaskDatabaseConnectionStr(), "select ErrorJSON from TB_Task where Id='" + taskEntity.Id + "'", CommandType.Text));
            List<TaskErrorModel> errorList = new List<TaskErrorModel>();
            if (!string.IsNullOrWhiteSpace(taskErrorString))
            {
                errorList = JsonHelper.Deserialize<List<TaskErrorModel>>(taskErrorString);
            }
            errorList.Add(new TaskErrorModel()
            {
                TaskFileId = taskFielEntity.Id,
                FileFullPath = taskFielEntity.FilePath,
                MessageText = msg
            });

            taskEntity.ErrorJSON = JsonHelper.Serialize(errorList);
            taskEntity.ErrorTotal = errorList.Count;

            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update TB_Task set ErrorJSON=@ErrorJSON,ErrorTotal=ErrorTotal+1 where Id=@Id");
            SQLiteParameter[] parameters = {
                    new SQLiteParameter("@Id", DbType.String),
                    new SQLiteParameter("@ErrorJSON", DbType.String)
            };
            parameters[0].Value = taskEntity.Id;
            parameters[1].Value = taskEntity.ErrorJSON;

            return SQLiteHelper.ExecuteNonQuery(AppHelper.GetTaskDatabaseConnectionStr(), strSql.ToString(), CommandType.Text, parameters) > 0;
        }

        /// <summary>
        /// 设置任务成功信息
        /// </summary>
        /// <param name="taskEntity"></param>
        /// <param name="taskFielEntity"></param>
        /// <returns></returns>
        private bool UpdateTaskSuccess(TaskEntity taskEntity, TaskFileEntity taskFielEntity)
        {
            taskEntity.SuccessTotal = taskEntity.SuccessTotal + 1;

            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update TB_Task set SuccessTotal=SuccessTotal+1 where Id=@Id");
            SQLiteParameter[] parameters = {
                    new SQLiteParameter("@Id", DbType.String)
            };
            parameters[0].Value = taskEntity.Id;

            return SQLiteHelper.ExecuteNonQuery(AppHelper.GetTaskDatabaseConnectionStr(), strSql.ToString(), CommandType.Text, parameters) > 0;
        }

        /// <summary>
        /// 更新任务配置
        /// </summary>
        /// <param name="taskEntit"></param>
        /// <returns></returns>
        private bool UpdateTaskSetting(TaskEntity taskEntity)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update TB_Task set SettingJSON=@SettingJSON where Id=@Id");
            SQLiteParameter[] parameters = {
                    new SQLiteParameter("@Id", DbType.String),
                    new SQLiteParameter("@SettingJSON", DbType.String)
            };
            parameters[0].Value = taskEntity.Id;
            parameters[1].Value = taskEntity.SettingJSON;

            return SQLiteHelper.ExecuteNonQuery(AppHelper.GetTaskDatabaseConnectionStr(), strSql.ToString(), CommandType.Text, parameters) > 0;
        }


        /// <summary>
        /// 更新文件请求前接口结果
        /// </summary>
        /// <param name="destDbPath"></param>
        /// <param name="taskFileId"></param>
        /// <param name="beforeJSON"></param>
        /// <returns></returns>
        private bool UpdateApiBeforeResultObj(string destDbPath, TaskFileEntity taskFileEntity, string beforeJSON)
        {
            taskFileEntity.BeforeResult = beforeJSON;
            //更新上传路径：
            ApiBeforeSysModel _sysModel = null;
            try { _sysModel = JsonConvert.DeserializeObject<ApiBeforeSysModel>(beforeJSON); } catch { }
            if (_sysModel != null)
            {
                taskFileEntity.ServerFullPath = _sysModel._NewServerPath;
            }


            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update TB_Files set BeforeResult=@BeforeResult,ServerFullPath=@ServerFullPath where Id=@Id");
            SQLiteParameter[] parameters = {
                    new SQLiteParameter("@Id", DbType.String),
                    new SQLiteParameter("@BeforeResult", DbType.String),
                    new SQLiteParameter("@ServerFullPath", DbType.String)
            };
            parameters[0].Value = taskFileEntity.Id;
            parameters[1].Value = taskFileEntity.BeforeResult;
            parameters[2].Value = taskFileEntity.ServerFullPath;

            return SQLiteHelper.ExecuteNonQuery(AppHelper.GetTaskFileDatabaseConnectionStr(destDbPath), strSql.ToString(), CommandType.Text, parameters) > 0;
        }

        /// <summary>
        /// 更新文件状态
        /// </summary>
        /// <param name="taskEntity"></param>
        /// <param name="taskFieldEntity"></param>
        /// <param name="setting"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool UpdateTaskFileStatus(string destDbPath, TaskEntity taskEntity, TaskFileEntity taskFielEntity, EnumTaskFileStatus status)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Update TB_Files set Status=@Status where Id=@Id");
            SQLiteParameter[] parameters = {
                    new SQLiteParameter("@Id", DbType.String),
                    new SQLiteParameter("@Status", DbType.Int32,4)
            };
            parameters[0].Value = taskFielEntity.Id;
            parameters[1].Value = (int)status;

            return SQLiteHelper.ExecuteNonQuery(AppHelper.GetTaskFileDatabaseConnectionStr(destDbPath), strSql.ToString(), CommandType.Text, parameters) > 0;
        }

        /// <summary>
        /// 清空任务(taskId为空，清空所有)
        /// </summary>
        /// <returns></returns>
        private void DeleteTask(string taskId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from TB_Task ");
            if (!string.IsNullOrWhiteSpace(taskId))
            {
                strSql.AppendFormat(" where Id='{0}'", taskId);
            }
            SQLiteHelper.ExecuteNonQuery(AppHelper.GetTaskDatabaseConnectionStr(), strSql.ToString(), CommandType.Text);

            DeleteFileLogDatabase(taskId);
        }

        /// <summary>
        /// 获取待上传文件列表(每批50条数据)
        /// </summary>
        /// <param name="destDbPath"></param>
        /// <param name="taskEntity"></param>
        /// <returns></returns>
        private DataTable GetUploadFiles(string destDbPath, TaskEntity taskEntity)
        {
            string sql = string.Format("select * from  TB_Files where TaskId='{0}' and Status={1} order by LastDt limit 50 offset 0", taskEntity.Id, (int)EnumTaskFileStatus.待上传);
            DataSet ds = SQLiteHelper.ExecuteDataSet(AppHelper.GetTaskFileDatabaseConnectionStr(destDbPath), sql, CommandType.Text);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <returns></returns>
        private List<TaskEntity> GetTaskList()
        {
            List<TaskEntity> list = new List<TaskEntity>();
            string sql = string.Format("select * from  TB_Task order by CreateDt desc limit 30 offset 0");
            DataSet ds = SQLiteHelper.ExecuteDataSet(AppHelper.GetTaskDatabaseConnectionStr(), sql, CommandType.Text);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var model = AppHelper.GetTaskEntityByRow(row);
                    if (model == null) { continue; }
                    list.Add(model);
                }
            }
            return list;
        }


        /// <summary>
        /// 从模版复制当前任务上传文件库
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        private string CopyFileLogTempateDB(string taskId)
        {
            string sourcePath = string.Format(@"{0}template.db", AppHelper.DatabasePath);
            string destDirPath = string.Format(@"{0}/temp/{1}/", AppHelper.DatabasePath, DateTime.Now.ToString("yyyyMM"));
            if (!Directory.Exists(destDirPath))
            {
                Directory.CreateDirectory(destDirPath);
            }
            string destFilePath = string.Format("{0}{1}.db", destDirPath, taskId);
            File.Copy(sourcePath, destFilePath);
            return destFilePath;
        }

        /// <summary>
        /// 移除任务文件
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        private void DeleteFileLogDatabase(string taskId)
        {
            string tempPath = string.Format(@"{0}/temp/", AppHelper.DatabasePath);
            if (string.IsNullOrWhiteSpace(taskId))
            {
                if (Directory.Exists(tempPath))
                {
                    try
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch { }
                }
                return;
            }

            string fileNme = string.Format("{0}.db", taskId);
            var files = Directory.GetFiles(tempPath, fileNme);
            if (files != null && files.Length > 0 && File.Exists(files[0]))
            {
                try
                {
                    File.Delete(files[0]);
                }
                catch { }

            }
        }

        /// <summary>
        /// 执行数据库操作
        /// </summary>
        /// <param name="type"></param>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        private int SQLExecute(string type, string connection, string sql)
        {
            if (VerifyHelper.IsEmpty(type) || VerifyHelper.IsEmpty(connection) || VerifyHelper.IsEmpty(sql))
            {
                throw new Exception("数据库操作参数错误（连接字符串/脚本）");
            }

            int result = 0;
            type = type.ToLower().Trim();

            if (type == "sqlserver")
            {
                result = SQLServerHelper.ExecuteNonQuery(connection, sql);
            }
            if (type == "mysql")
            {
                result = MySqlHelper.ExecuteNonQuery(connection, sql);
            }
            if (type == "oracle")
            {
                result = OracleHelper.ExecuteNonQuery(connection, sql);
            }
            if (type == "sqlite")
            {
                result = SQLiteHelper.ExecuteNonQuery(connection, sql, CommandType.Text);
            }

            return result;
        }

        #endregion

        #region 辅助方法-工具方法

        /// <summary>
        /// 绑定上传任务路径数据源
        /// </summary>
        /// <param name="pathType">类型(EnumPathType)</param>
        /// <param name="path"></param>
        private void BindTaskPathSource(EnumPathType pathType = EnumPathType.默认, string path = null)
        {
            if (taskPathSource == null) { taskPathSource = new List<TaskPathModel>(); }

            if (pathType != EnumPathType.默认 && !string.IsNullOrEmpty(path))
            {
                path = path.ToLower();
                if (taskPathSource.Count(x => x.ItemPath == path) == 0)
                {
                    taskPathSource.Add(new TaskPathModel { ItemIndex = -1, ItemType = (int)pathType, ItemPath = path, ItemCount = (pathType == EnumPathType.文件 ? 1 : 0), IsStatistical = (pathType == EnumPathType.文件 ? true : false) });
                }
            }

            for (var i = 0; i < taskPathSource.Count; i++)
            {
                if (taskPathSource[i].ItemIndex == -1)
                {
                    taskPathSource[i].ItemIndex = i + 1;
                }
            }

            ControlUtil.ExcuteAction(this, () =>
            {
                ControlUtil.DataGridSyncBinding(gridTaskPathList, taskPathSource);
            });
        }

        private void BindTaskList()
        {
            ControlUtil.ExcuteAction(this, () =>
            {
                ControlUtil.DataGridSyncBinding(gridTaskList, GetTaskList());
            });
        }

        /// <summary>
        /// 计算文件总数
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentFolderPath"></param>
        private void CalculateTaskPathCount(TaskPathModel model, string currentFolderPath)
        {
            var folders = Directory.GetDirectories(currentFolderPath);
            var files = Directory.GetFiles(currentFolderPath);
            if (files != null)
            {
                model.ItemCount += files.Length;
                this.Dispatcher.Invoke(new Action(() =>
                {
                    ControlUtil.DataGridSyncBinding(gridTaskPathList, taskPathSource);
                }));
            }
            foreach (var dir in folders)
            {
                CalculateTaskPathCount(model, dir);
            }
        }

        /// <summary>
        /// 输出日志(界面 + 文本)
        /// </summary>
        /// <param name="msg"></param>
        private void WriteLog(string msg, UILogType logType = UILogType.Default)
        {
            ControlUtil.ExcuteAction(this, () =>
            {
                TextBlock msgBlock = new TextBlock();
                msgBlock.Text = string.Format("{0} {1}", DateTime.Now.ToString("HH:mm:sss"), msg);
                switch (logType)
                {
                    case UILogType.Success:
                        msgBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                        break;
                    case UILogType.Error:
                        msgBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        break;
                    case UILogType.Warning:
                        msgBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFAE00"));
                        break;
                    default:
                        msgBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF34495E"));
                        break;
                }
                spLoggin.Children.Add(msgBlock);
            });

            #region 操作日志，滚动至底部

            //ThreadPool.QueueUserWorkItem(sender =>
            //{
            //    while (true)
            //    {
            //        this.txtLog.Dispatcher.BeginInvoke((Action)delegate
            //        {
            //            this.txtLog.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff\r\n"));
            //            if (IsVerticalScrollBarAtBottom)
            //            {
            //                this.txtLog.ScrollToEnd();
            //            }
            //        });
            //        Thread.Sleep(600);
            //    }
            //});

            #endregion
        }

        /// <summary>
        /// 操作状态
        /// </summary>
        /// <param name="msg"></param>
        private void WriteStatus(string msg, UILogType logType = UILogType.Default)
        {

            ControlUtil.ExcuteAction(this, () =>
            {
                switch (logType)
                {
                    case UILogType.Success:
                        tbStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                        break;
                    case UILogType.Error:
                        tbStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        break;
                    case UILogType.Warning:
                        tbStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFAE00"));
                        break;
                    default:
                        tbStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF34495E"));
                        break;
                }
                tbStatus.Text = msg;
            });
        }

        /// <summary>
        /// 设置FTP连接操作的控件可用与不可用操作 
        /// </summary>
        /// <param name="isSuccess"></param>
        private void SetFTPControlStatus(bool isSuccess = false)
        {
            if (isSuccess)
            {
                txtAddress.IsEnabled = false;
                txtName.IsEnabled = false;
                txtPassword.IsEnabled = false;
                txtPassword.IsEnabled = false;
                txtPort.IsEnabled = false;
                txtDirectory.IsEnabled = false;
                btnConnectOpen.Visibility = Visibility.Collapsed;
                btnConnectClose.Visibility = Visibility.Visible;
                btnFTPServerView.Visibility = Visibility.Visible;
            }
            else
            {
                txtAddress.IsEnabled = true;
                txtName.IsEnabled = true;
                txtPassword.IsEnabled = true;
                txtPassword.IsEnabled = true;
                txtPort.IsEnabled = true;
                txtDirectory.IsEnabled = true;
                btnConnectOpen.Visibility = Visibility.Visible;
                btnConnectClose.Visibility = Visibility.Collapsed;
                btnFTPServerView.Visibility = Visibility.Collapsed;
            }
            ShowRegisterButton(RegisterModel.IsTrial);
        }

        /// <summary>
        /// 获取服务器文件存储路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        private string GetServerFileFullPath(string filePath, string fileName)
        {
            if (ftpUtil == null || (ftpUtil != null && ftpUtil.ClientModel == null)) return fileName;
            return string.Format("{0}{1}", ftpUtil.ClientModel.Directory, fileName);
        }

        /// <summary>
        /// 替换标签
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="taskEntity"></param>
        /// <param name="taskFileEntity"></param>
        /// <returns></returns>
        private string ReplaceSqlTag(string sql, TaskEntity taskEntity, TaskFileEntity taskFileEntity)
        {
            if (VerifyHelper.IsEmpty(sql))
                return sql;
            //{FileFullPath}->文路径径，{FileSize}->文件大小，{ServerFileFullPath}
            return sql.Replace("{FileFullPath}", taskFileEntity.FilePath).Replace("{ServerFileFullPath}", taskFileEntity.ServerFullPath).Replace("{FileSize}", taskFileEntity.FileSize.ToString())
                .Replace("{filefullpath}", taskFileEntity.FilePath).Replace("{serverfilefullpath}", taskFileEntity.FilePath).Replace("{filesize}", taskFileEntity.FileSize.ToString())
                .Replace("{FILEFULLPATH}", taskFileEntity.FilePath).Replace("{SERVERFILEFULLPATH}", taskFileEntity.FilePath).Replace("{FILESIZE}", taskFileEntity.FileSize.ToString());
        }

        /// <summary>
        /// 获取文件MD5
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetFileMD5(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetFileMD5() fail, error:" + ex.Message);
            }
        }

        /// <summary>
        /// 上传信息参数
        /// </summary>
        /// <param name="param"></param>
        /// <param name="returnJosnObj"></param>
        /// <returns></returns>
        private string GetHttpPostByRequestParams(TaskEntity taskEntity, TaskFileEntity taskFileEntity, bool isAfterParam = false, JObject returnJosnObj = null)
        {
            JObject post = new JObject();
            if (taskFileEntity != null && taskEntity != null)
            {
                List<RequestParamModel> param = new List<RequestParamModel>();
                if (!isAfterParam)
                {
                    var berforeSetting = JsonHelper.Deserialize<RequestModel>(taskEntity.BeforeJSON);
                    param = berforeSetting.Params;
                }
                else
                {
                    var afterSetting = JsonHelper.Deserialize<RequestModel>(taskEntity.AfterJSON);
                    param = afterSetting.Params;
                }

                foreach (var item in param)
                {
                    if (item.Name.ToLower() == "id")
                    {
                        post["Id"] = taskFileEntity.Id;
                    }
                    if (item.Name.ToLower() == "filefullpath")
                    {
                        post["FileFullPath"] = taskFileEntity.FilePath;
                    }
                    if (item.Name.ToLower() == "filemd5")
                    {

                        post["FileMD5"] = GetFileMD5(taskFileEntity.FilePath);
                    }
                    if (item.Name.ToLower() == "serverfilefullpath")
                    {
                        post["ServerFileFullPath"] = taskFileEntity.ServerFullPath;
                    }
                    if (item.Name.ToLower() == "filesize")
                    {
                        post["FileSize"] = taskFileEntity.FileSize;
                    }
                    if (item.PType.ToLower() == "value")
                    {
                        post[item.Name] = item.DefaultValue;
                    }
                    if (item.PType.ToLower() == "return" && returnJosnObj != null)
                    {
                        post[item.Name] = returnJosnObj.GetJTokenValue(item.Name);
                    }
                }
            }
            return JsonHelper.Serialize(post);
        }

        /// <summary>
        /// Http请求并返回HttpResult.Html String
        /// </summary>
        /// <param name="url"></param>
        /// <param name="post"></param>
        /// <param name="method"></param>
        /// <param name="encoding"></param>
        /// <param name="contentType"></param>
        /// <param name="heads"></param>
        /// <returns></returns>
        private string GetHttp(string url, string post = null, string method = null, Encoding encoding = null, string contentType = null, WebHeaderCollection heads = null)
        {

            HttpItem item = new HttpItem();

            #region 默认值设置

            if (VerifyHelper.IsEmpty(encoding))
            {
                encoding = Encoding.UTF8;
            }

            if (VerifyHelper.IsEmpty(method))
            {
                method = "POST";
                if (VerifyHelper.IsEmpty(post))
                {
                    method = "GET";
                }
            }

            if (VerifyHelper.IsEmpty(contentType))
            {
                contentType = "application/json";
            }

            //不指定编辑可能导致接收不到数据(中文)
            if (VerifyHelper.IsEqualString(method, "post"))
            {
                item.PostEncoding = encoding;
            }

            #endregion

            item.Postdata = post;
            item.URL = url;
            item.Encoding = encoding;
            item.ContentType = contentType;
            item.Method = method;
            item.Header = heads;

            return GetHttp(item).Html;
        }

        /// <summary>
        /// Http请求并返回HttpResult
        /// </summary>
        private HttpResult GetHttp(HttpItem item)
        {
            if (VerifyHelper.IsNull(item))
            {
                throw new Exception("Http Error");
            }

            var httpResult = new HttpHelper().GetHtml(item);

            #region 请求结果处理

            if (VerifyHelper.IsNull(httpResult))
            {
                throw new Exception("Http Error");
            }

            if (VerifyHelper.IsEmpty(httpResult.Html))
            {
                throw new Exception("Http Error");
            }
            if (httpResult.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Http Error");
            }
            #endregion

            return httpResult;
        }


        #endregion

        #region 辅助方法-授权文件

        private void Loading()
        {
            var licPath = string.Format("{0}/data/license.key", AppDomain.CurrentDomain.BaseDirectory).ToLower();
            var pubKeyPath = string.Format("{0}/data/doublex.key", AppDomain.CurrentDomain.BaseDirectory).ToLower();

            if (!File.Exists(licPath))
            {
                if (MessageBox.Show("未找到授权文件，退出程序", "提示信息", MessageBoxButton.OK, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                    return;
                }
            }

            #region 获取授权文件内容

            //字符编号
            UTF8Encoding enc = new UTF8Encoding();

            //读取注册数据文件
            StreamReader sr = new StreamReader(licPath, UTF8Encoding.UTF8);
            string encrytText = sr.ReadToEnd();
            sr.Close();
            byte[] encrytBytes = System.Convert.FromBase64CharArray(encrytText.ToCharArray(), 0, encrytText.Length);

            //读取公钥
            StreamReader srPublickey = new StreamReader(pubKeyPath, UTF8Encoding.UTF8);
            string publicKey = srPublickey.ReadToEnd();
            srPublickey.Close();

            //用公钥初化始RSACryptoServiceProvider类实例crypt。
            RSACryptoServiceProvider crypt = new RSACryptoServiceProvider();
            crypt.FromXmlString(AESHelper.AESDecrypt(publicKey));

            int keySize = crypt.KeySize / 8;
            byte[] buffer = new byte[keySize];
            MemoryStream msInput = new MemoryStream(encrytBytes);
            MemoryStream msOuput = new MemoryStream();
            int readLen = msInput.Read(buffer, 0, keySize);
            while (readLen > 0)
            {
                byte[] dataToDec = new byte[readLen];
                Array.Copy(buffer, 0, dataToDec, 0, readLen);
                byte[] decData = crypt.Decrypt(dataToDec, false);
                msOuput.Write(decData, 0, decData.Length);
                readLen = msInput.Read(buffer, 0, keySize);
            }

            msInput.Close();
            byte[] result = msOuput.ToArray();    //得到解密结果
            msOuput.Close();
            crypt.Clear();

            string decryptText = enc.GetString(result);
            try
            {
                if (!string.IsNullOrWhiteSpace(decryptText))
                {
                    RegisterModel = JsonHelper.Deserialize<LicModel>(decryptText);
                }
            }
            catch (Exception ex) { };

            #endregion

            SetControler(RegisterModel);
        }

        private void SetControler(LicModel model)
        {
            var currentEdition = VeifyLicModel(model);

            if (model == null || currentEdition == EnumEditionType.Default)
            {
                if (MessageBox.Show("未找到授权文件，退出程序", "提示信息", MessageBoxButton.OK, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                    return;
                }
            }

            //注册按钮
            ShowRegisterButton(model.IsTrial);

            if (currentEdition == EnumEditionType.Default)
            {
                if (MessageBox.Show("授权文件验证失败，退出程序", "提示信息", MessageBoxButton.OK, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                    return;
                }
            }

            if (currentEdition == EnumEditionType.Basic)
            {
                this.LogoPath = "pack://application:,,,/Image/acp-base-logo.png";

                if (model.IsTrial)
                {
                    this.LogoPath = "pack://application:,,,/Image/acp-base-try-logo.png";
                }
                else
                {

                }
            }

            if (currentEdition == EnumEditionType.Professional)
            {
                this.LogoPath = "pack://application:,,,/Image/acp-pro-logo.png";

                if (model.IsTrial)
                {
                    this.LogoPath = "pack://application:,,,/Image/acp-pro-try-logo.png";
                }
                else
                {

                }
            }

            //设置Logo
        }

        private EnumEditionType VeifyLicModel(LicModel model)
        {
            //数据验证
            if (model == null)
                return EnumEditionType.Default;

            //产品验证
            if (model.Product.ToLower() != "doublex.upload")
            {
                return EnumEditionType.Default;
            }

            //版本验证，基础/专业版
            string[] editionArr = { EnumEditionType.Basic.ToString().ToLower(), EnumEditionType.Professional.ToString().ToLower() };
            if (!editionArr.Contains(model.Edition.ToLower()))
            {
                return EnumEditionType.Default; //版本错误
            }

            //试用版数据验证
            if (model.Email.ToLower() == "demo@demo.com" &&
                model.Mac.ToLower() == "xx-xx-xx-xx-xx-xx" && model.Cpu.ToLower() == "xxxxxxxxxxxxxxxx" &&
                model.Times.ToLower() == "0" && model.Date.ToLower() == "1900-01-01")
            {
                model.IsTrial = true;
            }

            //基础版
            if (model.Edition.ToLower() == EnumEditionType.Basic.ToString().ToLower())
            {
                //非试用版，数据验证
                if (!model.IsTrial)
                {

                }
                return EnumEditionType.Basic;
            }

            //专业版
            if (model.Edition.ToLower() == EnumEditionType.Professional.ToString().ToLower())
            {
                //非试用版，数据验证
                if (!model.IsTrial)
                {

                }
                return EnumEditionType.Professional;
            }

            return EnumEditionType.Default;
        }

        private void ShowRegisterButton(bool isTrial)
        {
            if (isTrial)
            {
                this.btnRegister.Visibility = Visibility.Visible;
                this.btnFTPServerView.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.btnRegister.Visibility = Visibility.Collapsed;
                this.btnFTPServerView.Visibility = Visibility.Visible;
            }
        }

        #endregion

    }
}
