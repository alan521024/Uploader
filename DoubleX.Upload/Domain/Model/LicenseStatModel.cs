using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Upload
{
    public class LicenseStatModel
    {
        public string Identification { get; set; }
        public long Count { get; set; }
        public DateTime Create { get; set; }
        public string Mac { get; set; }
        public string Cpu { get; set; }
        public bool Expire { get; set; }
    }
}
