using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Upload
{
    /// <summary>
    /// 任务设置
    /// </summary>
    public class TaskSettingModel
    {
        /// <summary>
        /// 当前任务上传根目录
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        /// 是否启用请求前操作
        /// </summary>
        public bool IsBefore { get; set; }

        /// <summary>
        /// 是否启用请求后操作
        /// </summary>
        public bool IsAfter { get; set; }

        /// <summary>
        /// 出错是否继续
        /// </summary>
        public bool IsErrorGoOn { get; set; }

        /// <summary>
        /// 文件数据库录径
        /// </summary>
        public string FileDatabasePath { get; set; }

    }
}
