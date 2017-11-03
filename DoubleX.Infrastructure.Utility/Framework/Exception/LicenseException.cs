using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Infrastructure.Utility
{
    public class LicenseException : Exception
    {
        public LicenseExceptionType ExceptionType { get; set; }

        public LicenseException(string message) : base(message)
        {
            ExceptionType = LicenseExceptionType.授权未知错误;
        }

        public LicenseException(LicenseExceptionType expType) : base("")
        {
            ExceptionType = expType;
        }

        public LicenseException(string message, LicenseExceptionType expType) : base(message)
        {
            ExceptionType = expType;
        }
    }

    public enum LicenseExceptionType
    {
        授权文件不存在,
        授权文件内容错误,
        授权信息错误,
        授权信息过期,
        授权未知错误,
        授权产品错误,
        授权版本错误,
        授权试用错误,
        授权试用过期,
        授权试用次数超出,
    }
}
