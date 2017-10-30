using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// 连接地址辅助类
    /// </summary>
    public class UrlsHelper
    {
        /// <summary>
        /// 获取当前请求Url(包含端口号&参数)
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentReqeustUrl()
        {
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                return HttpContext.Current.Request.Url.ToString();
            }
            return "";
        }
        /// <summary>
        /// 获取当前请求Url(包含端口号,不包含参数)
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentRequestUrlPath()
        {
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                return HttpContext.Current.Request.Url.AbsolutePath.ToString();
            }
            return "";
        }

        /// <summary>
        /// 获取域名名称
        /// </summary>
        public static string GetDomainName(string url = null)
        {
            url = string.IsNullOrWhiteSpace(url) ? GetCurrentReqeustUrl() : url;
            if (string.IsNullOrWhiteSpace(url))
                return "";

            Regex reg = new Regex(@"(http|https|ftp)://(?<domain>[^(:|/]*)", RegexOptions.IgnoreCase);
            Match m = reg.Match(url);
            return m.Groups["domain"].Value;
        }

        /// <summary>
        /// 获取域名网址(http://www.xxx.com)无‘/’结尾
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetDomainUrl(string url = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                url = GetCurrentReqeustUrl();
            try
            {
                Uri uri = new Uri(url);
                if (uri != null)
                {
                    return string.Format("{0}://{1}", uri.Scheme, uri.Authority);
                }
            }
            catch (Exception ex)
            {
            }
            return "";
        }

        /// <summary>
        /// 获取Url信息
        /// </summary>
        /// <param name="url">Url地址</param>
        /// <param name="path">需合并的路径</param>
        /// <returns>返回Url(http://www.xxx.com or http://www.xxx.com.com/a/b)</returns>
        public static string GetUrl(string url = null, string path = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                url = GetDomainUrl(); //如果根据CurrentUrleg 则获取到为 http://www.baidu.com/account/login 类假，加上path后变成 http://www.baidu.com/account/login/account/login
            try
            {
                Uri uri = new Uri(url);
                if (uri != null)
                {
                    string absPath = uri.AbsolutePath == "/" ? "" : uri.AbsolutePath.TrimEnd('/');
                    path = GetUrlPath(path);
                    return string.Format("{0}://{1}{2}{3}", uri.Scheme, uri.Authority, absPath, path == "/" ? "" : path);
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        /// <summary>
        /// 返回Url中的路径及路径以后数据,以'/'开始,结尾不带'/'
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isAndQueryValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetUrlPath(string path, bool isAndQueryValue = true, string defaultValue = "/")
        {
            if (string.IsNullOrWhiteSpace(path))
                return defaultValue;

            //path中带问号处理
            var queryString = "";
            var queryStringStart = path.IndexOf('?');
            if (queryStringStart > -1)
            {
                queryString = path.Substring(queryStringStart);
                path = path.Substring(0, queryStringStart);
            }

            path = path.Replace("\\", "/");
            while (path.IndexOf("//") > -1)
            {
                path = path.Replace("//", "/");
            }
            if (!path.StartsWith("/"))
            {
                path = string.Format("/{0}", path);
            }
            if (isAndQueryValue)
            {
                path = string.Format("{0}{1}", path.TrimEnd('/'), queryString);
            }
            else
            {
                path = path.TrimEnd('/');
            }
            if (string.IsNullOrWhiteSpace(path))
            {
                path = defaultValue;
            }
            return path;
        }

        /// <summary>
        /// 获取来源页面
        /// </summary>
        public static string GetRefUrl(string key = "_ref", string defaultUrl = "/", string url = null)
        {
            //如果当前请求地址中包含key
            var refUrl = GetQueryValue(key, url);

            if (string.IsNullOrWhiteSpace(refUrl) &&
                HttpContext.Current != null &&
                HttpContext.Current.Request != null &&
                HttpContext.Current.Request.UrlReferrer != null)
            {
                //ajax 访问中ajax的地址元_ref参数，但请求ajax的来源页中包含url
                var sourceRefUrl = GetQueryValue(key, HttpContext.Current.Request.UrlReferrer.ToString());
                if (!VerifyHelper.IsEmpty(sourceRefUrl))
                {
                    refUrl = sourceRefUrl;
                }
                else
                {
                    refUrl = VerifyHelper.IsAjax() ? "" : HttpContext.Current.Request.UrlReferrer.ToString();
                }
            }

            //来源页为空或为当前自己
            if (string.IsNullOrWhiteSpace(refUrl) || GetCurrentReqeustUrl().ToLower().Contains(refUrl))
            {
                refUrl = defaultUrl;
            }
            return refUrl;

            ////如果当前请求为Ajax且来源(ajaxy请求所在页面)的地址含key
            //if (VerifyHelper.IsAjax() && string.IsNullOrWhiteSpace(url) && HttpContext.Current.Request.UrlReferrer != null)
            //{
            //    var item = UrlsHelper.GetQueryList(HttpContext.Current.Request.UrlReferrer.ToString()).FirstOrDefault(x => x.Key.ToLower() == key.ToLower());
            //    if (item != null && !string.IsNullOrWhiteSpace(item.Value))
            //    {
            //        url = item.Value;
            //    }
            //}

            ////如果当前及ajax的请求来源地址中都不包含key
        }

        /// <summary>
        /// 获取Url参数部份
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetQueryString(string url = null)
        {
            url = string.IsNullOrWhiteSpace(url) ? GetCurrentReqeustUrl() : url;
            if (string.IsNullOrWhiteSpace(url))
                return "";

            Uri uri = new Uri(url);
            return string.IsNullOrWhiteSpace(uri.Query) ? "" : Decode(uri.Query.ToString().Trim('?'));
        }

        /// <summary>
        /// 获取当前请求Query的值
        /// </summary>
        public static string GetQueryValue(string key, string url = null)
        {

            if (string.IsNullOrWhiteSpace(key))
                return "";

            string value = GetQueryList(url).Where(x => string.Compare(x.Key, key, true) == 0).FirstOrDefault().Value;

            //安全检测

            return string.IsNullOrWhiteSpace(value) ? "" : value;
        }

        /// <summary>
        /// 获取Url参数部份并转为Lis[key,value]
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetQueryList(string url)
        {
            var queryString = GetQueryString(url);
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return new List<KeyValuePair<string, string>>();
            }
            var queryList = new List<KeyValuePair<string, string>>();
            queryString.Split('&').ToList().ForEach(x =>
            {
                if (!string.IsNullOrWhiteSpace(x))
                {
                    var arr = x.Split('=');
                    queryList.Add(new KeyValuePair<string, string>(arr[0], arr.Length == 2 ? arr[1] : ""));
                }
            });
            return queryList;
        }

        /// <summary>
        /// 附加QueryString参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string AttachQueryValue(string url, string keys, string values)
        {
            if (!VerifyHelper.IsEmpty(keys) && !VerifyHelper.IsEmpty(values))
                return AttachQueryValue(url, keys.Split('|'), values.Split('|'));
            return url;
        }

        /// <summary>
        ///  附加QueryString参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string AttachQueryValue(string url, string[] keys, string[] values)
        {
            if (VerifyHelper.IsEmpty(url))
            {
                url = "";
            }
            if (!VerifyHelper.IsEmpty(keys) && !VerifyHelper.IsEmpty(values) && keys.Length == values.Length)
            {
                var UrlPrefix = url.IndexOf("?") > -1 ? url.Substring(0, url.IndexOf("?")) : url;
                var queryVlues = new List<KeyValuePair<string, string>>();

                if (url.Contains("?"))
                {
                    var arr = url.Substring(url.IndexOf("?") + 1).Split('&');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        var keyVlues = arr[i].Split('=');
                        if (keyVlues.Length > 0)
                        {
                            queryVlues.Add(new KeyValuePair<string, string>(keyVlues[0], keyVlues.Length > 1 ? keyVlues[1] : ""));
                        }
                    }
                }

                for (int i = 0; i < keys.Length; i++)
                {
                    var keyModel = queryVlues.Find(x => VerifyHelper.IsEqualString(x.Key, keys[i]));
                    if (!VerifyHelper.IsNull(keyModel))
                    {
                        queryVlues.Remove(keyModel);
                    }
                    queryVlues.Add(new KeyValuePair<string, string>(keys[i], values[i]));
                }

                StringBuilder buildStr = new StringBuilder();
                buildStr.Append(UrlPrefix);
                for (int i = 0; i < queryVlues.Count; i++)
                {
                    if (i == 0) buildStr.Append("?");
                    buildStr.AppendFormat("{0}={1}&", queryVlues[i].Key, queryVlues[i].Value);
                }

                return StringHelper.GetByBuildString(buildStr, true);
            }
            return url;

        }

        /// <summary>
        /// 编码Url
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Encode(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "";
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// 转码Html
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Decode(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "";
            return HttpUtility.UrlDecode(str);
        }

    }
}
