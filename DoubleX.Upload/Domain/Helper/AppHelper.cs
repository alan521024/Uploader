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
    public class AppHelper
    {
        #region 数据库相关

        /// <summary>
        /// 数据库目录
        /// </summary>
        public static string DatabasePath { get { return @"Data\"; } }

        /// <summary>
        /// 任务数据库
        /// </summary>
        /// <returns></returns>
        public static string GetTaskDatabaseConnectionStr()
        {
            return string.Format(@"Data Source={0}database.db;", DatabasePath);
        }


        /// <summary>
        /// 任务文件数据库
        /// </summary>
        /// <param name="destDbPath"></param>
        /// <returns></returns>
        public static string GetTaskFileDatabaseConnectionStr(string destDbPath, string rootPath = null)
        {
            if (!string.IsNullOrWhiteSpace(rootPath))
            {
                destDbPath = string.Format("{0}{1}", rootPath, destDbPath);
            }
            return string.Format(@"Data Source={0};", destDbPath);
        }

        #endregion

        #region DataRow 转实体

        /// <summary>
        /// 获取任务信息实体
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static TaskEntity GetTaskEntityByRow(DataRow row)
        {
            TaskEntity model = null;
            if (row != null)
            {
                model = new TaskEntity();
                model.Id = StringHelper.Get(row["Id"]);
                model.TaskName = StringHelper.Get(row["TaskName"]);
                model.PathJSON = StringHelper.Get(row["PathJSON"]);
                model.BeforeJSON = StringHelper.Get(row["BeforeJSON"]);
                model.AfterJSON = StringHelper.Get(row["AfterJSON"]);
                model.SettingJSON = StringHelper.Get(row["SettingJSON"]);
                model.FileTotal = (double)LongHelper.Get(row["FileTotal"]);
                model.SuccessTotal = (double)LongHelper.Get(row["SuccessTotal"]);
                model.ErrorTotal = (double)LongHelper.Get(row["ErrorTotal"]);
                model.ErrorJSON = StringHelper.Get(row["ErrorJSON"]);
                model.Status = IntHelper.Get(row["Status"]);
                model.CreateDt = DateTimeHelper.Get(row["CreateDt"]);

            }
            return model;
        }

        /// <summary>
        /// 获取上传文件信息实体
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static TaskFileEntity GetTaskFileEntityByRow(DataRow row)
        {
            TaskFileEntity model = null;
            if (row != null)
            {
                model = new TaskFileEntity();
                model.Id = StringHelper.Get(row["Id"]);
                model.TaskId = StringHelper.Get(row["TaskId"]);
                model.FilePath = StringHelper.Get(row["FilePath"]);
                model.FileSize = LongHelper.Get(row["FileSize"]);
                model.FileName = StringHelper.Get(row["FileName"]);
                model.FileExtension = StringHelper.Get(row["FileExtension"]);
                model.ServerFullPath = StringHelper.Get(row["ServerFullPath"]);
                model.UpSize = LongHelper.Get(row["UpSize"]);
                model.BeforeResult = StringHelper.Get(row["BeforeResult"]);
                model.Status = IntHelper.Get(row["Status"]);
                model.LastDt = DateTimeHelper.Get(row["LastDt"]);

                if (string.IsNullOrWhiteSpace(model.ServerFullPath))
                {
                    model.ServerFullPath = model.FileName;
                }
            }
            return model;
        }

        #endregion
    }
}
