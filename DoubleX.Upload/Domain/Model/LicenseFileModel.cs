using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Upload
{
    /// <summary>
    /// 授权文件
    /// </summary>
    public class LicenseFileModel
    {
        /// <summary>
        /// 产品标识
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// 版本类型(试用Trial/基础Basic/专业Professional)
        /// </summary>
        public string Edition { get; set; }

        /// <summary>
        /// 注册文件用户唯一标识(试用邮箱：demo@demo.com)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 注册手机
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 机器Mac地址
        /// </summary>
        public string Mac { get; set; }

        /// <summary>
        /// 机器CPU地址
        /// </summary>
        public string Cpu { get; set; }

        /// <summary>
        /// 使用次数(0不限制)
        /// </summary>
        public string Times { get; set; }

        /// <summary>
        /// 过期日期(1900-01-01， 不过期)
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 是否试用
        /// </summary>
        public bool IsTrial { get; set; }
    }
}
