using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// 枚举工具类
    /// </summary>
    public static class EnumsHelper
    {
        #region 枚举获取

        /// <summary>
        /// 获取对象枚举项
        /// </summary>
        /// <typeparam name="TEntity">枚举</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>TEntity 返回枚举项</returns>
        public static TEntity Get<TEntity>(object obj)
        {
            if (obj != null)
            {
                return (TEntity)Enum.Parse(typeof(TEntity), obj.ToString());
            }
            return default(TEntity);
        }

        /// <summary>
        /// 获取字符串枚举项
        /// </summary>
        /// <typeparam name="TEntity">枚举</typeparam>
        /// <param name="obj">字符串</param>
        /// <returns>TEntity 返回枚举项</returns>
        public static TEntity Get<TEntity>(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return (TEntity)Enum.Parse(typeof(TEntity), value.Trim());
            }
            return default(TEntity);
        }

        #endregion

        #region 枚举操作(GetByXXX,GetToXXX,GetByXXXXToXXX)


        /// <summary>
        /// 获取枚举文本(选获取Descript/Name)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static string GetText<TEntity>(int value)
        {
            return GetText(typeof(TEntity), value);
        }

        /// <summary>
        /// 获取枚举文本(选获取Descript/Name)
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetText(Type enumType, int value)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", "enumType");
            }
            string text = "";
            try
            {
                Enum item = (Enum)Enum.Parse(enumType, value.ToString());
                text = EnumsHelper.GetDescript(enumType, item);
                if (string.IsNullOrWhiteSpace(text))
                {
                    text = Enum.GetName(enumType, value);
                }
            }
            catch (Exception)
            {
            }
            return text;
        }

        /// <summary>
        /// 获取枚举名称
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static string GetName<TEntity>(int value)
        {
            return GetName(typeof(TEntity), value);
        }

        /// <summary>
        /// 获取枚举名称
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(Type enumType, int value)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", "enumType");
            }
            string name = "";
            try
            {
                name = Enum.GetName(enumType, value);
            }
            catch (Exception)
            {
            }
            return name;
        }

        /// <summary>
        /// 枚举描述
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescript<TEntity>(int value)
        {
            var item = Get<TEntity>(value);
            if (item == null)
                return "";
            MemberInfo member = typeof(TEntity).GetMember(item.ToString()).FirstOrDefault();
            return member != null ? TypesHelper.ToDescription(member) : item.ToString();
        }

        /// <summary>
        /// 枚举描述
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescript(Type enumType, Enum value)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", "enumType");
            }
            MemberInfo member = enumType.GetMember(value.ToString()).FirstOrDefault();
            return member != null ? TypesHelper.ToDescription(member) : value.ToString();
        }

        /// <summary>
        /// 获取枚举字典
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="isTextIsDescript">文本是否为描述</param>
        /// <returns>Dictionary 集合</returns>
        public static Dictionary<int, string> ToDictionary(Type enumType, bool isDescript = false)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", "enumType");
            }

            Dictionary<int, string> enumDic = new Dictionary<int, string>();
            foreach (Enum item in Enum.GetValues(enumType))
            {
                Int32 key = Convert.ToInt32(item);
                string value = Enum.GetName(enumType, item);
                if (isDescript)
                {
                    var descript = item.GetEnumDescription();
                    value = string.IsNullOrWhiteSpace(descript) ? value : descript;
                }
                enumDic.Add(key, value);
            }
            return enumDic;
        }

        #endregion

        #region 枚举扩展

        /// <summary>
        /// 根据枚举项获取文本
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumName(this Enum value, int? defaultValue = null)
        {
            if (value == null)
                throw new ArgumentException("Enum is null");

            if (defaultValue == null)
            {
                return value.ToString();
            }
            return Enum.GetName(value.GetType(), defaultValue.Value);
        }

        /// <summary>
        /// 根据枚举项获取值
        /// </summary>
        /// <typeparam name="TEntity">枚举</typeparam>
        /// <param name="entity">枚举</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>int 返回值</returns>
        public static int GetEnumValue(this Enum value, int defaultValue = 0)
        {
            try
            {
                if (value == null)
                    return defaultValue;
                return IntHelper.Get(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取枚举项上的<see cref="DescriptionAttribute"/>特性的文字描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum value)
        {
            if (value == null)
                return "";

            return GetDescript(value.GetType(), value);
        }

        #endregion

    }
}
