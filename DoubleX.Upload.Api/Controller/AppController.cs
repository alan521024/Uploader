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
            returnObj["LastVersion"] = "1.0.0.0";   //服务器最新版本
            returnObj["CurrentVersion"] = version;  //请求软件版本
            returnObj["DownloadUrl"] = "http://www.baidu.com";   //下载地址
            returnObj["Incremental"] = false;                    //是否强制更新
            return ToHttpResponseMessage(returnObj);
        }
    }
}
