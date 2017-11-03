using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DoubleX.Upload.Api
{
    public class AppController : BaseController
    {
        /// <summary>
        /// 程序版本
        /// </summary>
        [HttpGet]
        public HttpResponseMessage Version(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return ToHttpResponseMessage("错误错误");

            //返回信息
            JObject returnObj = new JObject();
            returnObj["LastVersion"] = "1.0.0.1";
            returnObj["CurrentVersion"] = version;
            returnObj["DownloadUrl"] = "http://www.baidu.com";
            returnObj["Incremental"] = false;
            return ToHttpResponseMessage(returnObj);
        }
    }
}
