using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// StringBuild辅助类
    /// </summary>
    public static class StringBuildHelper
    {
        #region 扩展方法

        /// <summary>
        /// 去除<seealso cref="StringBuilder"/>开头的空格
        /// </summary>
        /// <param name="strinBuild">StringBuilder</param>
        /// <returns>返回修改后的StringBuilder，主要用于链式操作</returns>
        public static StringBuilder TrimStart(this StringBuilder strinBuild)
        {
            return strinBuild.TrimStart(' ');
        }

        /// <summary>
        /// 去除<seealso cref="StringBuilder"/>开头的指定<seealso cref="char"/>
        /// </summary>
        /// <param name="strinBuild"></param>
        /// <param name="c">要去掉的<seealso cref="char"/></param>
        /// <returns></returns>
        public static StringBuilder TrimStart(this StringBuilder strinBuild, char c)
        {
            if (strinBuild == null)
                throw new NullReferenceException("StringBuilder is null");

            if (strinBuild.Length == 0)
                return strinBuild;
            while (c.Equals(strinBuild[0]))
            {
                strinBuild.Remove(0, 1);
            }
            return strinBuild;
        }

        /// <summary>
        /// 去除<seealso cref="StringBuilder"/>开头的指定字符数组
        /// </summary>
        /// <param name="strinBuild"></param>
        /// <param name="cs">要去掉的字符数组</param>
        /// <returns></returns> 
        public static StringBuilder TrimStart(this StringBuilder strinBuild, char[] cs)
        {
            if (strinBuild == null)
                throw new NullReferenceException("StringBuilder is null");
            return strinBuild.TrimStart(new string(cs));
        }
        /// <summary>
        /// 去除<see cref="StringBuilder"/>开头的指定的<seealso cref="string"/>
        /// </summary>
        /// <param name="strinBuild"></param>
        /// <param name="str">要去掉的<seealso cref="string"/></param>
        /// <returns></returns>
        public static StringBuilder TrimStart(this StringBuilder strinBuild, string str)
        {
            if (strinBuild == null)
                throw new NullReferenceException("StringBuilder is null");
            if (string.IsNullOrEmpty(str)
                || strinBuild.Length == 0
                || str.Length > strinBuild.Length)
                return strinBuild;
            while (strinBuild.SubString(0, str.Length).Equals(str))
            {
                strinBuild.Remove(0, str.Length);
                if (str.Length > strinBuild.Length)
                {
                    break;
                }
            }
            return strinBuild;
        }

        /// <summary>
        /// 去除StringBuilder结尾的空格
        /// </summary>
        /// <param name="strinBuild">StringBuilder</param>
        /// <returns>返回修改后的StringBuilder，主要用于链式操作</returns>
        public static StringBuilder TrimEnd(this StringBuilder strinBuild)
        {
            return strinBuild.TrimEnd(' ');
        }

        /// <summary>
        /// 去除<see cref="StringBuilder"/>结尾指定字符
        /// </summary>
        /// <param name="strinBuild"></param>
        /// <param name="c">要去掉的字符</param>
        /// <returns></returns>
        public static StringBuilder TrimEnd(this StringBuilder strinBuild, char chr)
        {
            if (strinBuild == null)
                throw new NullReferenceException("StringBuilder is null");
            if (strinBuild.Length == 0)
                return strinBuild;
            while (chr.Equals(strinBuild[strinBuild.Length - 1]))
            {
                strinBuild.Remove(strinBuild.Length - 1, 1);
            }
            return strinBuild;
        }

        /// <summary>
        /// 去除<see cref="StringBuilder"/>结尾指定字符数组
        /// </summary>
        /// <param name="strinBuild"></param>
        /// <param name="chars">要去除的字符数组</param>
        /// <returns></returns>
        public static StringBuilder TrimEnd(this StringBuilder strinBuild, char[] chars)
        {
            if (strinBuild == null)
                throw new NullReferenceException("StringBuilder is null");

            return strinBuild.TrimEnd(new string(chars));
        }

        /// <summary>
        /// 去除<see cref="StringBuilder"/>结尾指定字符串
        /// </summary>
        /// <param name="strinBuild"></param>
        /// <param name="str">要去除的字符串</param>
        /// <returns></returns>
        public static StringBuilder TrimEnd(this StringBuilder strinBuild, string str)
        {
            if (strinBuild == null)
                throw new NullReferenceException("StringBuilder is null");

            if (string.IsNullOrEmpty(str)
                || strinBuild.Length == 0
                || str.Length > strinBuild.Length)
                return strinBuild;
            while (strinBuild.SubString(strinBuild.Length - str.Length, str.Length).Equals(str))
            {
                strinBuild.Remove(strinBuild.Length - str.Length, str.Length);
                if (strinBuild.Length < str.Length)
                {
                    break;
                }
            }
            return strinBuild;
        }
        
        /// <summary>
        /// 返回<see cref="StringBuilder"/>从起始位置指定长度的字符串
        /// </summary>
        /// <param name="strinBuild"></param>
        /// <param name="start">起始位置</param>
        /// <param name="length">长度</param>
        /// <returns>字符串</returns>
        /// <exception cref="OverflowException">超出字符串索引长度异常</exception>
        public static string SubString(this StringBuilder strinBuild, int start, int length)
        {
            if (strinBuild == null)
                throw new NullReferenceException("StringBuilder is null");

            if (start + length > strinBuild.Length)
                throw new IndexOutOfRangeException("超出字符串索引长度");

            char[] cs = new char[length];
            for (int i = 0; i < length; i++)
            {
                cs[i] = strinBuild[start + i];
            }
            return new string(cs);
        }

        #endregion
    }
}
