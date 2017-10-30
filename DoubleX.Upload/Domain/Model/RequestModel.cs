using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Upload
{
    /// <summary>
    /// 上传前/后请求信息
    /// </summary>
    public class RequestModel
    {
        public bool IsEnable { get; set; }
        public string Url { get; set; }
        public List<RequestParamModel> Params { get; set; }
    }
}
