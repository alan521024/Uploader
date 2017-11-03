using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Upload
{
    public class VersionModel
    {
        public string LastVersion { get; set; }
        public string CurrentVersion { get; set; }
        public string DownloadUrl { get; set; }
        public bool Incremental { get; set; }
    }
}
