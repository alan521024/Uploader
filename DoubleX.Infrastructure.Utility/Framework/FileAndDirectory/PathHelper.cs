using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// 路径辅助操作
    /// </summary>
    public class PathHelper
    {
        /// <summary>
        /// 获取路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rootPath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string Get(string path, string rootPath = "/", string defaultValue = "/")
        {
            string pathString = string.Format("{0}{1}", rootPath, path);
            pathString = pathString.Replace("\\", "/");
            while (pathString.IndexOf("//") > -1)
            {
                pathString = pathString.Replace("//", "/");
            }
            if (!pathString.StartsWith("/"))
            {
                pathString = string.Format("/{0}", pathString);
            }
            if (pathString.EndsWith("/"))
            {
                pathString = pathString.TrimEnd('/');
            }
            if (string.IsNullOrWhiteSpace(pathString))
            {
                pathString = defaultValue;
            }
            return pathString;
        }

        /// <summary>
        /// 合并路径
        /// </summary>
        /// <param name="rootPath">根路径</param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string Combine(params string[] paths)
        {
            StringBuilder build = new StringBuilder();
            if (paths != null && paths.Length > 0)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    build.AppendFormat("/{0}", paths[i]);
                }
            }
            return Get(build.TrimEnd("/").ToString());
        }

        /// <summary>
        /// 获取路由路径
        /// </summary>
        public static string GetRouteRegistPath(string path)
        {
            if (!VerifyHelper.IsEmpty(path))
            {
                while (path.IndexOf("//") > -1)
                {
                    path = path.Replace("//", "");
                }
                path = path.Trim('/') + "/";
            }
            return "";
        }

    }
}
