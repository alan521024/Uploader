using System;
using System.Net.Http;
using System.IO;
using System.Web.Http;
using Newtonsoft.Json.Linq;

namespace DoubleX.Upload.Api
{
    public class UploadController : BaseController
    {
        /// <summary>
        /// 上传前接口
        /// </summary>
        [HttpPost]
        public HttpResponseMessage Before(BeforeRequestModel request)
        {
            if (request == null)
                return ToHttpResponseMessage("false before api 参数错误");


            if (string.IsNullOrWhiteSpace(request.FileFullPath) ||
                string.IsNullOrWhiteSpace(request.FileMD5))
            {
                return ToHttpResponseMessage("false before api 文件信息错误");
            }

            //自定义参数
            if (request.ExtA != "123")
            {
                return ToHttpResponseMessage("false before api 接口演示程序：必须添加传入key为 ExtA, Value：为123 的自定义参数");
            }

            //文件大小
            if (request.FileSize == 0)
            {
                return ToHttpResponseMessage("false before api 空文件不允许上传");
            }

            //使用原文件名称
            string newFileName = Path.GetFileName(request.FileFullPath);

            //重名文件名称(guid)
            //string newFileName = string.Format("{0}{1}",Guid.NewGuid().ToString(), Path.GetExtension(request.FileFullPath));

            //根据时间(/年/月)定义保存路径 eg:/2017/10/文件名
            string newServerPath = string.Format("/{0}/{1}", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));

            //根据MD5定义保存路径 eg:/5A/1D/A/文件名
            //string newServerPath = string.Format("/{0}/{1}/{2}",
            //    request.FileMD5.Substring(0,2),
            //    request.FileMD5.Substring(2, 2),
            //    request.FileMD5.Substring(4, 1));

            //eg:文件数据ID
            string id = Guid.NewGuid().ToString();
            //sql: insert into table(Id,xxx) values(id,xxx);

            //返回信息
            JObject returnObj = new JObject();

            //自定义的返回数据
            returnObj["ReturnId"] = id;
            //returnObj["ReturnName"] = "xxxxx";

            //上传前接口返回系统标签(该标识： 通知客户端文件保存在FTP服务器上的路径)
            //该数据可为空，为空是客户端默认的路径ServerFileFullPath值
            returnObj["_NewServerPath"] = string.Format("{0}/{1}", newServerPath, newFileName);

            return ToHttpResponseMessage(returnObj);
        }

        /// <summary>
        /// 上传后接口
        /// </summary>
        [HttpPost]
        public HttpResponseMessage After(AfterRequestModel request)
        {
            if (request == null)
                return ToHttpResponseMessage("false after api 参数错误");


            if (string.IsNullOrWhiteSpace(request.FileFullPath) ||
                string.IsNullOrWhiteSpace(request.FileMD5))
            {
                return ToHttpResponseMessage("false after api 文件信息错误");
            }

            //自定义固定数据
            if (request.ExtB != "456")
            {
                return ToHttpResponseMessage("false after api 接口演示程序：必须添加传入key为 ExtB, Value：为456 固定值的自定义参数");
            }
            
            //上传文件前处理的数据数Id.值为：本文件代码67行
            if (string.IsNullOrWhiteSpace(request.ReturnId))
            {
                return ToHttpResponseMessage("false after api 接口演示程序, 上传文件前接口http://xxxxxx/api/upload/before 未返回 正确的Id数据");
            }

            //根据上传前插入的数据更新
            //sql: update table1 set stuats='成功' where id=request.ReturnId

            //return ToHttpResponseMessage(false);  //失败返回
            return ToHttpResponseMessage(true);     //成功返回
        }

    }
}
