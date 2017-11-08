using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Upload
{
    [Serializable]
    public class ConfigModel
    {
        /// <summary>
        /// 官方网站
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        /// 版本校验Url
        /// </summary>
        public string VersionUrl { get; set; }

        /// <summary>
        /// 购买地址
        /// </summary>
        public string BuyUrl { get; set; }
    }
}
