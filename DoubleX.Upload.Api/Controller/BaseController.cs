using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DoubleX.Upload.Api
{
    public class BaseController : ApiController
    {

        /// <summary>
        /// 将结果将返统一返回消息对象(Webapi使用)
        /// </summary>
        protected static HttpResponseMessage ToHttpResponseMessage(object obj)
        {
            return new HttpResponseMessage { Content = obj == null ? null : new StringContent(JsonConvert.SerializeObject((obj)), System.Text.Encoding.UTF8, "application/json") };
        }
    }
}
