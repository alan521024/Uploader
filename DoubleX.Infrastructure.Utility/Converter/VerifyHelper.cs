using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// 信息校验类
    /// </summary>
    public class VerifyHelper
    {
        /// <summary>
        /// 判断对象是否为Null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(object obj)
        {
            return obj == null;
        }

        /// <summary>
        /// 判断对象是否Null或空(string null/"",list null/0,arr null/0)
        /// </summary>
        /// <returns></returns>
        public static bool IsEmpty(object obj)
        {
            if (obj == null)
                return true;

            string objType = obj.GetType().FullName;

            if (objType == "System.String")
            {
                return string.IsNullOrWhiteSpace(obj.ToString()) ? true : false;
            }
            else if (objType == "System.DateTime")
            {
                DateTime date = DateTime.MinValue;
                DateTime.TryParse(obj.ToString(), out date);
                return date == DateTime.MinValue || DateTimeHelper.GetEnd(DateTime.MinValue) == date;
            }
            else if (objType == "System.Guid")
            {
                Guid value = Guid.Empty;
                Guid.TryParse(obj.ToString(), out value);
                return value == Guid.Empty;
            }
            else if (objType == "Newtonsoft.Json.Linq.JValue")
            {
                return (obj as JArray).Count == 0;
            }
            else if (objType == "Newtonsoft.Json.Linq.JObject")
            {
                return (obj as JObject).Properties().Count() == 0;
            }
            else if (objType == "System.Int32")
            {
                return IntHelper.Get(obj) == 0;
            }
            else if (obj is ICollection && (obj as ICollection).Count == 0)
            {
                return true;
                //typeof(ICollection).IsAssignableFrom(obj.GetType())
            }
            //0001/1/1 0:00:00

            //System.Web.HttpValueCollection(NameValueCollection)
            //Newtonsoft.Json.Linq.JArray
            return false;
        }

        /// <summary>
        /// 判断是否空字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmpty(string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 判断是否空字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmpty(StringBuilder build)
        {
            return build == null || (build != null && build.Length == 0);
        }

        /// <summary>
        /// 判断是否空Guid（NULL Empty）
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsEmpty(Guid guid)
        {
            return guid == Guid.Empty;
        }

        /// <summary>
        /// 判断集合是否为空
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsEmpty(ICollection list)
        {
            return !(list != null && list.Count > 0);
        }

        /// <summary>
        /// 判断类型是否为Nullable类型
        /// </summary>
        /// <param name="type"> 要处理的类型 </param>
        /// <returns> 是返回True，不是返回False </returns>
        public static bool IsNullableType(Type type)
        {
            if (type == null)
                return false;

            return ((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }


        /// <summary>
        /// 判断类型是否为集合类型
        /// </summary>
        /// <param name="type">要处理的类型</param>
        /// <returns>是返回True，不是返回False</returns>
        public static bool IsEnumerable(Type type)
        {
            if (type == null)
                return false;

            if (type == typeof(string))
            {
                return false;
            }
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// 判断当前泛型类型是否可由指定类型的实例填充
        /// </summary>
        /// <param name="genericType">泛型类型</param>
        /// <param name="type">指定类型</param>
        /// <returns></returns>
        public static bool IsGenericAssignableFrom(Type genericType, Type type, bool typeIsOnlyGenericType = true)
        {
            if (genericType == null)
                throw new NullReferenceException("genericType is null");


            if (type == null)
                throw new NullReferenceException("type is null");

            if (!genericType.IsGenericType)
            {
                throw new ArgumentException("该功能只支持泛型类型的调用，非泛型类型可使用 IsAssignableFrom 方法。");
            }

            //type仅泛型类型
            if (typeIsOnlyGenericType && !type.IsGenericType)
            {
                return false;
            }

            List<Type> allOthers = new List<Type> { type };
            if (genericType.IsInterface)
            {
                allOthers.AddRange(type.GetInterfaces());
            }

            foreach (var other in allOthers)
            {
                Type cur = other;
                while (cur != null)
                {
                    if (cur.IsGenericType)
                    {
                        cur = cur.GetGenericTypeDefinition();
                    }
                    if (cur.IsSubclassOf(genericType) || cur == genericType)
                    {
                        return true;
                    }
                    cur = cur.BaseType;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断当前泛型参数类型(可判断多个泛型参数是否多个类型中的某一个)
        /// </summary>
        /// <param name="genericType"></param>
        /// <param name="argumentTypes"></param>
        /// <returns></returns>
        public static bool IsGenericArgumentsFrom(Type genericType, params Type[] types)
        {
            if (genericType == null || types == null)
                return false;

            if (types.Length == 0)
                return false;

            Type[] argumentTypes = genericType.GetGenericArguments();
            if (argumentTypes.Length == 0)
                return false;

            foreach (var arg in argumentTypes)
            {
                foreach (var t in types)
                {
                    if (t.IsAssignableFrom(arg))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 类型匹配
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static bool IsType(Type type, string typeName)
        {
            if (type.ToString() == typeName)
                return true;
            if (type.ToString() == "System.Object")
                return false;

            return IsType(type.BaseType, typeName);
        }

        /// <summary>
        /// 方法是否是异步
        /// </summary>
        public static bool IsAsync(MethodInfo method)
        {
            if (method == null)
                return false;
            return method.ReturnType == typeof(Task)
                || method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        }

        /// <summary>
        /// 返回当前类型是否是指定基类的派生类
        /// </summary>
        /// <param name="type">当前类型</param>
        /// <param name="baseType">要判断的基类型</param>
        /// <returns></returns>
        public static bool IsBaseOn(Type type, Type baseType)
        {
            if (type.IsGenericTypeDefinition)
            {
                return IsGenericAssignableFrom(baseType, type);
            }
            return baseType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 返回当前类型是否是指定基类的派生类
        /// </summary>
        /// <typeparam name="TBaseType">要判断的基类型</typeparam>
        /// <param name="type">当前类型</param>
        /// <returns></returns>
        public static bool IsBaseOn<TBaseType>(Type type)
        {
            if (type == null)
                return false;
            return IsBaseOn(type, typeof(TBaseType));
        }


        /// <summary>
        /// 判断是否支持Context
        /// </summary>
        public static bool IsSupperHttpContext()
        {
            //上下文无效(多线程，当前为非请求线程 )
            return HttpContext.Current != null;
        }

        /// <summary>
        /// 判断是否支持Cookie
        /// </summary>
        /// <returns></returns>
        public static bool IsSupperHttpCookie()
        {
            //上下文无效(多线程，当前为非请求线程 )
            var context = HttpContext.Current;
            if (context == null)
                return false;

            //判断是否有Cookies(Golable Start 无Cookies)
            return context.Request.Cookies != null;
        }

        /// <summary>
        /// 判断是否Ajax请求(WebForm)
        /// </summary>
        public static bool IsAjax(HttpContext context)
        {
            if (context == null)
                return false;
            return context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        /// <summary>
        /// 判断是否Ajax请求(Mvc)
        /// </summary>
        public static bool IsAjax(HttpContextBase context = null)
        {
            if (context != null)
            {
                return context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            }
            return HttpContext.Current.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        }

        /// <summary>
        /// 判断两个时间日期部份是否相同(date2=null 时默认为当天)
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool IsEqualDate(DateTime date1, DateTime? date2 = null)
        {

            if (date2 == null)
                date2 = DateTime.Now;
            return date1.Date.Equals(date2.Value.Date);
            //EF
            //System.Data.Entity.DbFunctions.DiffDays(date1.StartTime.Value,DateTime.Now) == 0//只获取当天
        }

        /// <summary>
        /// 判断两个字符串是否一致
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool IsEqualString(string str1, string str2, bool ignoreCase = true)
        {
            //, StringComparison.CurrentCultureIgnoreCase
            if (string.IsNullOrWhiteSpace(str1) && string.IsNullOrWhiteSpace(str2))
                return true;
            return String.Compare(str1, str2, ignoreCase) == 0;
        }

        /// <summary>
        /// 检查指定指定类型成员中是否存在指定的Attribute特性
        /// </summary>
        /// <typeparam name="T">要检查的Attribute特性类型</typeparam>
        /// <param name="memberInfo">要检查的类型成员</param>
        /// <param name="inherit">是否从继承中查找</param>
        /// <returns>是否存在</returns>
        public static bool HasAttribute<T>(MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            if (memberInfo == null)
                return false;
            return memberInfo.IsDefined(typeof(T), inherit);
            //return memberInfo.GetCustomAttributes(typeof(T), inherit).Any(m => (m as T) != null);
        }


        /// <summary>
        /// 指示所指定的正则表达式在指定的输入字符串中是否找到了匹配项
        /// </summary>
        /// <param name="value">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <param name="isContains">是否包含，否则全匹配</param>
        /// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false</returns>
        public static bool IsMatch(string value, string pattern, bool isContains = true)
        {
            if (value == null)
            {
                return false;
            }
            return isContains
                ? Regex.IsMatch(value, pattern)
                : Regex.Match(value, pattern).Success;
        }

        /// <summary>
        /// 是否是Unicode字符串
        /// </summary>
        public static bool IsUnicode(string value)
        {
            const string pattern = @"^[\u4E00-\u9FA5\uE815-\uFA29]+$";
            return IsMatch(value, pattern);
        }

        /// <summary>
        /// 是否Url字符串
        /// </summary>
        public static bool IsUrl(string value)
        {
            try
            {
                if (IsEmpty(value) || value.Contains(' '))
                {
                    return false;
                }
                Uri uri = new Uri(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //**=============================格式类校验=======================================**/

        /// <summary>
        /// 判断是否为IP地址
        /// </summary>
        public static bool IsIP(string ipStr)
        {
            if (string.IsNullOrWhiteSpace(ipStr))
                return false;
            return Regex.IsMatch(ipStr, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 验证是否邮箱地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmail(string str)
        {
            const string pattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
            return IsMatch(str, pattern);
        }

        /// <summary>
        /// 验证是否联系电话(座机或手机号码)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsTelphone(string str)
        {
            return IsMobile(str) || IsPhone(str);
        }

        /// <summary>
        /// 验证是否手机号码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsMobile(string str, bool isRestrict = false)
        {
            string pattern = isRestrict ? @"^[1][3-8]\d{9}$" : @"^[1]\d{10}$";
            return IsMatch(str, pattern);
        }

        /// <summary>
        /// 验证是否电话号码(区号[3-4位]-数字[6-8位])
        /// </summary>
        /// <returns></returns>
        public static bool IsPhone(string str)
        {
            return IsMatch(str, @"^(\d{3,4}-)?\d{6,8}$");
        }

        /// <summary>
        /// 是否身份证号，验证如下3种情况：
        /// 1.身份证号码为15位数字；
        /// 2.身份证号码为18位数字；
        /// 3.身份证号码为17位数字+1个字母
        /// </summary>
        public static bool IsIdentityCard(string value)
        {
            if (IsEmpty(value))
                return false;

            if (value.Length != 15 && value.Length != 18)
            {
                return false;
            }
            Regex regex;
            string[] array;
            DateTime time;
            if (value.Length == 15)
            {
                regex = new Regex(@"^(\d{6})(\d{2})(\d{2})(\d{2})(\d{3})_");
                if (!regex.Match(value).Success)
                {
                    return false;
                }
                array = regex.Split(value);
                return DateTime.TryParse(string.Format("{0}-{1}-{2}", "19" + array[2], array[3], array[4]), out time);
            }
            regex = new Regex(@"^(\d{6})(\d{4})(\d{2})(\d{2})(\d{3})([0-9Xx])$");
            if (!regex.Match(value).Success)
            {
                return false;
            }
            array = regex.Split(value);
            if (!DateTime.TryParse(string.Format("{0}-{1}-{2}", array[2], array[3], array[4]), out time))
            {
                return false;
            }
            //校验最后一位
            string[] chars = value.ToCharArray().Select(m => m.ToString()).ToArray();
            int[] weights = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                int num = int.Parse(chars[i]);
                sum = sum + num * weights[i];
            }
            int mod = sum % 11;
            string vCode = "10X98765432";//检验码字符串
            string last = vCode.ToCharArray().ElementAt(mod).ToString();
            return chars.Last().ToUpper() == last;
        }
    }
}
