using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Upload
{
    /// <summary>
    /// 参数信息实体
    /// </summary>
    public class RequestParamModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PType { get; set; }
        public string Descript { get; set; }
        public string DefaultValue { get; set; }
        public bool IsCanDelete { get; set; }
    }
}
