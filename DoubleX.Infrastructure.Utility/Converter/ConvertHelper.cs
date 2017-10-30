using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;

namespace DoubleX.Infrastructure.Utility
{
    public class ConvertHelper
    {
        #region String Get

        /// <summary>
        /// 获取对象字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>字符串</returns>
        public static string GetString(object obj, string defaultValue = "")
        {
            if (obj == null)
                return defaultValue;
            try
            {
                return obj.ToString();
            }
            catch (Exception ex)
            {
                return defaultValue;
            }

        }

        /// <summary>
        /// 获取整数字符串
        /// </summary>
        /// <param name="value">整数值</param>
        /// <param name="format">格式</param>
        /// <returns>字符串</returns>
        public static string GetString(int value, int defaultValue = 0, string format = null)
        {
            return value.ToString(format);
        }

        /// <summary>
        /// 获取小数字符串
        /// </summary>
        /// <param name="value">小数值</param>
        /// <param name="format">格式</param>
        /// <returns>字符串</returns>
        public static string GetString(decimal value, decimal defaultValue = 0, string format = null)
        {
            return value.ToString(format);
        }

        /// <summary>
        /// 获取双精度数字符串
        /// </summary>
        /// <param name="value">双精度数</param>
        /// <param name="format">格式</param>
        /// <returns>字符串</returns>
        public static string GetString(double value, double defaultValue = 0, string format = null)
        {
            return value.ToString(format);
        }

        /// <summary>
        /// 获取单精度数小数字符串
        /// </summary>
        /// <param name="value">单精度数</param>
        /// <param name="format">格式</param>
        /// <returns>字符串</returns>
        public static string GetString(float value, float defaultValue = 0, string format = null)
        {
            return value.ToString(format);
        }

        /// <summary>
        /// 获取日期字符串
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <param name="format">格式</param>
        /// <param name="defaultDateTime">默认值</param>
        /// <returns>返回日期字符串</returns>
        public static string GetString(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss", DateTime? defaultDateTime = null)
        {
            return dateTime.ToString(format);
        }

        /// <summary>
        /// 获取字节数组字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string GetString(byte[] bytes, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;

            if (bytes == null)
                return "";
            if (bytes.Length == 0)
                return "";
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// 获取字节流字符串
        /// </summary>
        /// <param name="stream">字节流</param>
        /// <returns>字符串</returns>
        public static string GetString(Stream stream)
        {
            if (stream != null)
            {
                stream.Seek(0, SeekOrigin.Begin);
                return new StreamReader(stream).ReadToEnd();
            }
            return "";
        }

        /// <summary>
        /// 获取集合字符串(返回a=b&c=d格式)
        /// </summary>
        /// <param name="list">NameValueCollection 数据</param>
        /// <param name="separator">分割符</param>
        /// <returns>字符串</returns>
        public static string GetString(NameValueCollection list, char separator = '&')
        {
            if (list == null || (list != null && list.Count == 0))
                return "";
            StringBuilder build = new StringBuilder();
            foreach (string key in list.Keys)
            {
                build.AppendFormat("{0}={1}{2}", key, list[key], separator);
            }
            return build.Length == 0 ? "" : build.ToString().Trim(separator);
        }

        /// <summary>
        /// 获取数组字符串
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="arrs">数组</param>
        /// <param name="separator">分割符</param>
        /// <returns>字符串</returns>
        public static string GetString<TEntity>(TEntity[] arrs, string separator = ",")
        {
            return string.Join(separator, arrs);
        }

        /// <summary>
        /// 获取字符串数组字符串
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="arrs">数组</param>
        /// <param name="separator">分割符</param>
        /// <returns>字符串</returns>
        public static string GetString(string[] arrs, string separator = ",")
        {
            return string.Join(separator, arrs);
        }

        /// <summary>
        /// 获取集合字符串
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="list"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetString<TEntity>(List<TEntity> list, string separator = ",")
        {
            return string.Join(separator, list);
        }

        /// <summary>
        /// 获取字符串集合字符串
        /// </summary>
        /// <param name="list"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetString(List<string> list, string separator = ",")
        {
            return string.Join(separator, list);
        }

        #endregion

        #region Guid Get

        /// <summary>
        /// 获取对象GUID
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回GUID对象</returns>
        public static Guid Get(object obj, Guid? defaultValue = null)
        {
            if (defaultValue == null)
                defaultValue = Guid.Empty;

            if (obj == null)
                return defaultValue.Value;

            Guid returnValue = defaultValue.Value;
            Guid.TryParse(obj.ToString(), out returnValue);
            return returnValue;
        }

        /// <summary>
        /// 获取字符串GUID
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>返回GUID对象</returns>
        public static Guid Get(string str, Guid? defaultValue = null)
        {
            if (defaultValue == null)
                defaultValue = Guid.Empty;

            if (string.IsNullOrWhiteSpace(str))
                return defaultValue.Value;

            str = str.Trim();

            Guid returnValue = defaultValue.Value;
            Guid.TryParse(str, out returnValue);
            return returnValue;
        }

        #endregion
    }
}
