using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubleX.Upload
{
    public class AfterRequestModel
    {
        public string Id { get; set; }
        public string FileFullPath { get; set; }
        public string FileMD5 { get; set; }
        public string ServerFileFullPath { get; set; }
        public long FileSize { get; set; }
        public string ExtB { get; set; }
        public string ReturnId { get; set; }
    }
}
