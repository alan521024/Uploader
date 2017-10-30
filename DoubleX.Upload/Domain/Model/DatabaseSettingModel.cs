using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Upload
{
    public class DatabaseSettingModel
    {
        public bool IsEnable { get; set; }
        public string DBType { get; set; }
        public string ConnectionStr { get; set; }
        public string SQL { get; set; }
    }
}
