// -----------------------------------------------------------------------
//  <copyright file="AbstractBuilder.cs" company="OSharp开源团队">
//      Copyright (c) 2014 OSharp. All rights reserved.
//  </copyright>
//  <last-editor>郭明锋</last-editor>
//  <last-date>2014:07:05 2:52</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// 对象辅助类
    /// </summary>
    public static class ObjectHelper
    {
        #region 对象操作

        /// <summary>
        /// 将对象[主要是匿名对象]转换为dynamic
        /// </summary>
        public static dynamic ToDynamic(object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            Type type = value.GetType();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
            foreach (PropertyDescriptor property in properties)
            {
                var val = property.GetValue(value);
                if (property.PropertyType.FullName.StartsWith("<>f__AnonymousType"))
                {
                    dynamic dval = ToDynamic(val);
                    expando.Add(property.Name, dval);
                }
                else
                {
                    expando.Add(property.Name, val);
                }
            }
            return expando as ExpandoObject;
        }


        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <typeparam name="T">要创建对象的类型</typeparam>
        /// <param name="nameSpace">类型所在命名空间</param>
        /// <param name="className">类型名</param>
        /// <param name="parameters">构造函数参数</param>
        /// <returns></returns>
        public static T CreateInstance<T>(string nameSpace, string className, object[] parameters)
        {
            try
            {
                string fullName = nameSpace + "." + className;//命名空间.类型名
                object ect = Assembly.GetExecutingAssembly().CreateInstance(fullName, true, System.Reflection.BindingFlags.Default, null, parameters, null, null);//加载程序集，创建程序集里面的 命名空间.类型名 实例
                return (T)ect;//类型转换并返回
            }
            catch
            {
                //发生异常，返回类型的默认值
                return default(T);
            }
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static object GetValue(object obj, string fieldName) {

            if (VerifyHelper.IsNull(obj))
                return null;

            PropertyInfo propertyInfo = obj.GetType().GetProperty(fieldName);
            if (VerifyHelper.IsNull(propertyInfo))
                return null;

            return propertyInfo.GetValue(obj, null);
        }

        /// <summary>
        /// 设置相应属性的值
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="fieldName">属性名</param>
        /// <param name="fieldValue">属性值</param>
        public static void SetValue(object obj, string fieldName, object value)
        {
            if (VerifyHelper.IsNull(obj))
                return;

            PropertyInfo propertyInfo = obj.GetType().GetProperty(fieldName);
            if (VerifyHelper.IsNull(propertyInfo))
                return;

            if (VerifyHelper.IsType(propertyInfo.PropertyType, "System.String"))
            {
                propertyInfo.SetValue(obj, value, null);
                return;
            }
            else if (VerifyHelper.IsType(propertyInfo.PropertyType, "System.Boolean"))
            {
                propertyInfo.SetValue(obj, BoolHelper.Get(value), null);
                return;
            }
            else if (VerifyHelper.IsType(propertyInfo.PropertyType, "System.Int32"))
            {
                propertyInfo.SetValue(obj, IntHelper.Get(value), null);
                return;
            }
            else if (VerifyHelper.IsType(propertyInfo.PropertyType, "System.Decimal"))
            {
                propertyInfo.SetValue(obj, DecimalHelper.Get(value), null);
                return;
            }
            else if (VerifyHelper.IsType(propertyInfo.PropertyType, "System.Guid"))
            {
                propertyInfo.SetValue(obj, GuidHelper.Get(value), null);
                return;
            }
            else if (VerifyHelper.IsType(propertyInfo.PropertyType, "System.Nullable`1[System.DateTime]"))
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                {
                    try
                    {
                        propertyInfo.SetValue(
                            obj,
                            (DateTime?)DateTime.ParseExact(value.ToString(), "yyyy-MM-dd HH:mm:ss", null), null);
                    }
                    catch
                    {
                        propertyInfo.SetValue(obj, (DateTime?)DateTime.ParseExact(value.ToString(), "yyyy-MM-dd", null), null);
                    }
                }
                else
                    propertyInfo.SetValue(obj, null, null);
                return;
            }
            else
            {
                propertyInfo.SetValue(obj, value, null);
            }
        }


        #endregion

        #region 扩展方法

        /// <summary>
        /// Used to simplify and beautify casting an object to a type. 
        /// </summary>
        /// <typeparam name="T">Type to be casted</typeparam>
        /// <param name="obj">Object to cast</param>
        /// <returns>Casted object</returns>
        public static T As<T>(this object obj)
            where T : class
        {
            return (T)obj;
        }

        /// <summary>
        /// Converts given object to a value type using <see cref="Convert.ChangeType(object,System.TypeCode)"/> method.
        /// </summary>
        /// <param name="obj">Object to be converted</param>
        /// <typeparam name="T">Type of the target object</typeparam>
        /// <returns>Converted object</returns>
        public static T To<T>(this object obj)
            where T : struct
        {
            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Check if an item is in a list.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="list">List of items</param>
        /// <typeparam name="T">Type of the items</typeparam>
        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }

        #endregion
    }
}