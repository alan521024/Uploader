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
    /// Task.xaml 的交互逻辑
    /// </summary>
    public partial class TaskView : DxWindow
    {
        /// <summary>
        /// 任务信息
        /// </summary>
        public TaskEntity TaskModel { get; set; }

        /// <summary>
        /// 任务设置
        /// </summary>
        public TaskSettingModel TaskSetting { get; set; }

        //先定义一个常量  
        const int PageSize = 20;


        public TaskView(TaskEntity model)
        {
            InitializeComponent();
            this.TaskModel = model;
            BindTask();
        }

        private void BindTask()
        {
            if (TaskModel == null)
                return;

            this.Title = string.Format("上传记录（{0}）", TaskModel.TaskName);
            TaskSetting = JsonHelper.Deserialize<TaskSettingModel>(TaskModel.SettingJSON);
            if (!VerifyHelper.IsEmpty(TaskSetting.FileDatabasePath) && File.Exists(string.Format("{0}/{1}", AppHelper.DatabasePath,TaskSetting.FileDatabasePath)))
            {
                BindTaskFile(PageSize, 1);
            }
        }


        #region 文件列表

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="number">每个页面显示的记录数</param>
        /// <param name="currentPage">表示当前显示页数  </param>
        private void BindTaskFile(int number, int currentPage)
        {
            List<TaskFileEntity> list = new List<TaskFileEntity>();

            int total = 0;
            DataTable table = SQLiteHelper.SelectPaging(AppHelper.GetTaskFileDatabaseConnectionStr(TaskSetting.FileDatabasePath, AppHelper.DatabasePath),
                "TB_Files", "*", string.Format("TaskId='{0}'", TaskModel.Id), "LastDt", number, currentPage, out total);
            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    var model = AppHelper.GetTaskFileEntityByRow(row);
                    if (model == null) { continue; }
                    list.Add(model);
                }
            }

            int pageTotal = 0;
            if (total % number == 0)
            {
                pageTotal = total / number;
            }
            else
            {
                pageTotal = total / number + 1;
            }

            ControlUtil.ExcuteAction(this, () =>
            {
                tbxPageNum.Text = StringHelper.Get(currentPage);

                tbkTotal.Text = StringHelper.Get(pageTotal);

                tbkCurrentsize.Text = StringHelper.Get(currentPage);

                ControlUtil.DataGridSyncBinding(gridTaskList, list);
            });
        }

        //上一页事件   
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            int currentsize = IntHelper.Get(tbkCurrentsize.Text); //获取当前页数  
            if (currentsize > 1)
            {
                BindTaskFile(PageSize, currentsize - 1);   //调用分页方法  
            }
        }

        //下一页事件  
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            int total = IntHelper.Get(tbkTotal.Text); //总页数  
            int currentsize = IntHelper.Get(tbkCurrentsize.Text); //当前页数  
            if (currentsize < total)
            {
                BindTaskFile(PageSize, currentsize + 1);   //调用分页方法  
            }
        }

        //跳转事件  
        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            int pageNum = IntHelper.Get(tbxPageNum.Text);
            int total = IntHelper.Get(tbkTotal.Text); //总页数  
            if (pageNum >= 1 && pageNum <= total)
            {
                BindTaskFile(PageSize, pageNum);     //调用分页方法  
            }
        }

        #endregion
    }
}
