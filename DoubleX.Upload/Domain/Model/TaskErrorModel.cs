using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Upload
{
    /// <summary>
    /// 任务错误信息记录
    /// </summary>
    public class TaskErrorModel
    {
        /// <summary>
        /// 出错文件ID(如有)
        /// </summary>
        public string TaskFileId { get; set; }

        /// <summary>
        /// 出错文件
        /// </summary>
        public string FileFullPath { get; set; }

        /// <summary>
        /// 出错描述
        /// </summary>
        public string MessageText { get; set; }
    }
}
