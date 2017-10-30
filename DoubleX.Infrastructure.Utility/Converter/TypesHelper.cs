using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// 类型操作辅助类
    /// </summary>
    public static class TypesHelper
    {
        /// <summary>
        /// 由类型的Nullable类型返回实际类型
        /// </summary>
        /// <param name="type"> 要处理的类型对象 </param>
        /// <returns> </returns>
        public static Type GetNonNummableType(Type type)
        {
            if (VerifyHelper.IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        /// <summary>
        /// 通过类型转换器获取Nullable类型的基础类型
        /// </summary>
        /// <param name="type"> 要处理的类型对象 </param>
        /// <returns> </returns>
        public static Type GetUnNullableType(Type type)
        {
            if (VerifyHelper.IsNullableType(type))
            {
                NullableConverter nullableConverter = new NullableConverter(type);
                return nullableConverter.UnderlyingType;
            }
            return type;
        }

        /// <summary>
        /// 获取对象属性值 
        /// </summary>
        public static string GetObjectPropertyValue<TEntity>(TEntity model, string propertyName)
        {
            Type type = typeof(TEntity);
            PropertyInfo property = type.GetProperty(propertyName);
            if (property == null) return "";
            object o = property.GetValue(model, null);
            if (o == null) return "";
            return o.ToString();
        }

        /// <summary>
        /// 获取指定属性信息（非String类型存在装箱与拆箱）
        /// eg:var p = GetPropertyInfo<T>(t => t.Age);//获取指定属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, dynamic>> select)
        {
            var body = select.Body;
            if (body.NodeType == ExpressionType.Convert)
            {
                var o = (body as UnaryExpression).Operand;
                return (o as MemberExpression).Member as PropertyInfo;
            }
            else if (body.NodeType == ExpressionType.MemberAccess)
            {
                return (body as MemberExpression).Member as PropertyInfo;
            }
            return null;
        }

        /// <summary>
        /// 获取指定属性信息（需要明确指定属性类型，但不存在装箱与拆箱）
        /// eg: var ps1 = GetPropertyInfos<T>(t => t);//获取类型所有属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="select"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<T, TR>(Expression<Func<T, TR>> select)
        {
            var body = select.Body;
            if (body.NodeType == ExpressionType.Convert)
            {
                var o = (body as UnaryExpression).Operand;
                return (o as MemberExpression).Member as PropertyInfo;
            }
            else if (body.NodeType == ExpressionType.MemberAccess)
            {
                return (body as MemberExpression).Member as PropertyInfo;
            }
            return null;
        }

        /// <summary>
        /// 获取类型的所有属性信息
        /// eg:var ps2 = GetPropertyInfos<People>(t => new { t.Name, t.Age });//获取部份属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetPropertyInfos<T>(Expression<Func<T, dynamic>> select)
        {
            var body = select.Body;
            if (body.NodeType == ExpressionType.Parameter)
            {
                return (body as ParameterExpression).Type.GetProperties();
            }
            else if (body.NodeType == ExpressionType.New)
            {
                return (body as NewExpression).Members.Select(m => m as PropertyInfo).ToArray();
            }
            return null;
        }


        /// <summary>
        /// 获取类型的Description特性描述信息
        /// </summary>
        /// <param name="type">类型对象</param>
        /// <param name="inherit">是否搜索类型的继承链以查找描述特性</param>
        /// <returns>返回Description特性描述信息，如不存在则返回类型的全名</returns>
        public static string ToDescription(Type type, bool inherit = false)
        {
            if (type == null)
                return "";
            DescriptionAttribute desc = GetAttribute<DescriptionAttribute>(type, inherit);
            return desc == null ? type.FullName : desc.Description;
        }

        /// <summary>
        /// 获取成员元数据的Description特性描述信息
        /// </summary>
        /// <param name="member">成员元数据对象</param>
        /// <param name="inherit">是否搜索成员的继承链以查找描述特性</param>
        /// <returns>返回Description特性描述信息，如不存在则返回成员的名称</returns>
        public static string ToDescription(MemberInfo member, bool inherit = false)
        {
            if (member == null)
                return "";

            DescriptionAttribute desc = GetAttribute<DescriptionAttribute>(member,inherit);
            if (desc != null)
            {
                return desc.Description;
            }
            DisplayNameAttribute displayName = GetAttribute<DisplayNameAttribute>(member, inherit);
            if (displayName != null)
            {
                return displayName.DisplayName;
            }
            DisplayAttribute display = GetAttribute<DisplayAttribute>(member, inherit);
            if (display != null)
            {
                return display.Name;
            }
            return member.Name;
        }


        /// <summary>
        /// 从类型成员获取指定Attribute特性
        /// </summary>
        /// <typeparam name="T">Attribute特性类型</typeparam>
        /// <param name="memberInfo">类型类型成员</param>
        /// <param name="inherit">是否从继承中查找</param>
        /// <returns>存在返回第一个，不存在返回null</returns>
        public static T GetAttribute<T>(MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            if (memberInfo == null)
                return default(T);

            var descripts = memberInfo.GetCustomAttributes(typeof(T), inherit);
            return descripts.FirstOrDefault() as T;
        }

        /// <summary>
        /// 从类型成员获取指定Attribute特性
        /// </summary>
        /// <typeparam name="T">Attribute特性类型</typeparam>
        /// <param name="memberInfo">类型类型成员</param>
        /// <param name="inherit">是否从继承中查找</param>
        /// <returns>返回所有指定Attribute特性的数组</returns>
        public static T[] GetAttributes<T>(MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            if (memberInfo == null)
                return new T[] { };
            return memberInfo.GetCustomAttributes(typeof(T), inherit).Cast<T>().ToArray();
        }
    }
}
