using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Upload
{
    /// <summary>
    /// 上传文件信息实体
    /// </summary>
    public class TaskFileEntity
    {
        public string Id { get; set; }
        public string TaskId { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string ServerFullPath { get; set; }
        public long UpSize { get; set; }
        public string BeforeResult { get; set; }
        public int Status { get; set; }
        public DateTime LastDt { get; set; }
        public string StatusText
        {
            get
            {
                EnumTaskFileStatus status = EnumTaskFileStatus.默认;
                Enum.TryParse<EnumTaskFileStatus>(Status.ToString(), out status);
                return status.ToString();
            }
        }
    }
}
