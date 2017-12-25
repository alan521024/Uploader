using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubleX.Upload.Api
{
    public class BeforeRequestModel
    {
        public string Id { get; set; }
        public string FileFullPath { get; set; }
        public string FileMD5 { get; set; }
        public string ServerFileFullPath { get; set; }
        public long FileSize { get; set; }
        public string Extension { get; set; }
        public string UpDateTime { get; set; }
        public string ExtA { get; set; }
    }
}
