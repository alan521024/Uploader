using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// 属性扩展方法
    /// </summary>
    public static class AttributeExtensions
    {
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="type"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }
    }
}
