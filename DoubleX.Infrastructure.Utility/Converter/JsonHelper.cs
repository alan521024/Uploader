using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// JSON工具类
    /// </summary>
    public static class JsonHelper
    {
        #region JSON操作(Deserialize,Serialize,GetByXXX,GetToXXX,GetByXXXXToXXX)

        /// <summary>
        /// 将字符串序列化成对象
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <param name="str">字符串</param>
        /// <param name="isNewObj">为空时是否new 新对象</param>
        /// <returns></returns>
        public static object Deserialize(string str, Type type = null)
        {
            object returnObj = null;
            if (!string.IsNullOrWhiteSpace(str))
            {
                returnObj = JsonConvert.DeserializeObject(str, type, GetSetting());
            }
            return returnObj;
        }

        /// <summary>
        /// 将对象序列化成对象
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static TEntity Deserialize<TEntity>(dynamic obj)
        {
            if (obj == null)
            {
                return default(TEntity);
            }

            var objType = obj.GetType();
            if (objType.IsValueType || objType.FullName == "Newtonsoft.Json.Linq.JObject" || objType.FullName == "Newtonsoft.Json.Linq.JToken" || objType.FullName == "Newtonsoft.Json.Linq.JArray")
            {
                //Newtonsoft.Json.Linq.JObject
                //Newtonsoft.Json.Linq.JToken
                //Newtonsoft.Json.Linq.JArray
                //.IsValueType
                return Deserialize<TEntity>(StringHelper.Get(obj));
            }

            //引用对像
            return Deserialize<TEntity>(Serialize(obj));
        }

        /// <summary>
        /// 将字符串序列化成对象
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static TEntity Deserialize<TEntity>(string str)
        {
            TEntity returnObj = default(TEntity);
            if (!VerifyHelper.IsEmpty(str))
            {
                returnObj = JsonConvert.DeserializeObject<TEntity>(str, GetSetting());
            }
            return returnObj;
        }


        /// <summary>
        /// 将对象返序列化成字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="formatting">时间格式</param>
        /// <param name="settings">返序列化设置</param>
        /// <returns>json 字符串</returns>
        public static string Serialize(object obj, Formatting? formatting = null)
        {
            string str = "";
            if (obj != null)
            {
                if (formatting == null)
                {
                    str = JsonConvert.SerializeObject(obj, GetSetting());
                }
                else
                {
                    str = JsonConvert.SerializeObject(obj, formatting.Value,  GetSetting());
                }
            }
            return str;
        }

        /// <summary>
        /// 将字符串转为Json字符串(格式：a=a1&b=b1 表单/QueryString)
        /// </summary>
        /// <param name="queryStr">QueryString字符串</param>
        /// <returns>Json字符串</returns>
        public static string GetJsonStrByQueryForm(string queryStr)
        {
            if (string.IsNullOrEmpty(queryStr))
                return "";

            StringBuilder build = new StringBuilder();
            NameValueCollection list = System.Web.HttpUtility.ParseQueryString(queryStr);

            for (int i = 0; i < list.Count; i++)
            {
                build.AppendFormat("\"{0}\":\"{1}\",", list.GetKey(i), list.Get(i));
            }
            if (build.Length > 0)
            {
                return string.Format("{{{0}}}", build.TrimEnd(',').ToString());
            }
            return "";
        }

        /// <summary>
        /// 将字符串转为对象(格式：a=a1&b=b1 表单/QueryString)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryFormStr"></param>
        /// <returns></returns>
        public static TEntity GetJsonStrByQueryFormToEntity<TEntity>(string queryFormStr)
        {
            string jsonStr = GetJsonStrByQueryForm(queryFormStr);
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                return Deserialize<TEntity>(jsonStr);
            }
            return default(TEntity);
        }

        #endregion

        #region 扩展方法

        /// <summary>
        /// 获取JObject的Token Value
        /// </summary>
        /// <param name="obj">JObject对象</param>
        /// <param name="key">值Key</param>
        /// <returns></returns>
        public static string GetJTokenValue(this JObject obj, string key)
        {
            if (!VerifyHelper.IsNull(obj) && !VerifyHelper.IsEmpty(key))
            {
                JToken token = obj.GetValue(key, StringComparison.InvariantCultureIgnoreCase);
                return token == null ? "" : token.ToString();
            }
            return "";
        }

        /// <summary>
        /// 合并两个JObject对象(将Json字符串序列成JObject)(只支持第一级差异对比)
        /// </summary>
        /// <param name="obj">源JObject对象</param>
        /// <param name="jsonStr">json字符串</param>
        /// <returns>合并后的JObject</returns>
        public static JObject BuildJObject(this JObject obj, string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return obj;
            }
            JObject obj2 = Deserialize<JObject>(str);
            if (obj2 == null)
                return obj;
            return BuildJObject(obj, obj2);
        }

        /// <summary>
        /// 合并两个JObject对象(将Json字符串序列成JObject)(只支持第一级差异对比)
        /// </summary>
        /// <param name="obj1">对象1</param>
        /// <param name="obj2">对象2</param>
        /// <returns>合并后的JObject</returns>
        public static JObject BuildJObject(this JObject obj1, JObject obj2)
        {
            if (obj1 == null)
                obj1 = new JObject();
            if (obj2 == null)
                return obj1;

            var newObj = new JObject();

            //设置源
            foreach (JProperty item in obj1.Properties())
            {
                newObj.Add(item);
            }

            //设置目标
            foreach (JProperty item in obj2.Properties())
            {
                var sObj = newObj.Properties().FirstOrDefault(x => x.Name.ToLower() == item.Name.ToLower());
                if (sObj != null)
                {
                    newObj[sObj.Name] = item.Value;
                }
                else
                {
                    newObj.Add(item.Name, item.Value);
                }
            }
            return newObj;
        }

        #endregion

        #region 私有方法

        private static JsonSerializerSettings GetSetting() {
            JsonSerializerSettings jsetting = new JsonSerializerSettings();

            //GUID
            jsetting.Converters.Add(new GuidConverter());
            //因类型判断问题这里不统一注册，在需要GUID序列化的地方增加[JsonConverter(typeof(GuidConverter))]  

            //Bool
            jsetting.Converters.Add(new BoolConverter());

            //时间
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            timeFormat.Culture = CultureInfo.InvariantCulture;
            jsetting.Converters.Add(timeFormat);

            //接口或继承类ref:http://www.cnblogs.com/OpenCoder/p/4524786.html
            //jsetting.TypeNameHandling = TypeNameHandling.Auto;

            //格式
            jsetting.Formatting = Formatting.Indented; //是否带\r\n(Indented 如果带预览查看有结构)

            //空值忽略
            jsetting.NullValueHandling = NullValueHandling.Ignore;

            //空值处理
            //jsetting.ContractResolver = new SpecialContractResolver();

            return jsetting;
        }

        #endregion

    }

}
