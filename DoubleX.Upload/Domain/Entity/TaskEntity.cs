using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Upload
{
    /// <summary>
    /// 任务记录实体
    /// </summary>
    public class TaskEntity
    {
        public string Id { get; set; }
        public string TaskName { get; set; }
        public string PathJSON { get; set; }
        public string BeforeJSON { get; set; }
        public string AfterJSON { get; set; }
        public string SettingJSON { get; set; }
        public string DBSettingJSON { get; set; }
        public double FileTotal { get; set; }
        public double SuccessTotal { get; set; }
        public double ErrorTotal { get; set; }
        public string ErrorJSON { get; set; }
        public int Status { get; set; }
        public DateTime CreateDt { get; set; }
        public string StatusText
        {
            get
            {
                EnumTaskStatus status = EnumTaskStatus.默认;
                Enum.TryParse<EnumTaskStatus>(Status.ToString(), out status);
                return status.ToString();
            }
        }
    }
}
