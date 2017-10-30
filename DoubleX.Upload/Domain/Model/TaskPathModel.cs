using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Upload
{
    /// <summary>
    /// 任务路径信息
    /// </summary>
    public class TaskPathModel
    {
        public int ItemIndex { get; set; }

        /// <summary>
        /// Type:1 文件夹，2 文件
        /// </summary>
        public int ItemType { get; set; }

        public string ItemPath { get; set; }

        public long ItemCount { get; set; }

        public string ItemTypeText { get { return ItemType == 1 ? "文件夹" : "文件"; } }

        /// <summary>
        /// 是否己统计
        /// </summary>
        public bool IsStatistical { get; set; }
    }
}
