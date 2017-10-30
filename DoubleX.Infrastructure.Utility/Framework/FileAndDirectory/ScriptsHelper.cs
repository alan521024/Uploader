using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// 脚本工具类
    /// </summary>
    public class ScriptsHelper
    {
        /// <summary>
        /// 将List[Item]转为Json对象
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="objName">对象定义</param>
        /// <returns>Json内容</returns>
        public static string GetJSONStringByList(List<KeyValuePair<string, string>> list, string objName = null)
        {
            var definitionName = GetDefinitionName(objName);
            if (list == null || (list != null && list.Count == 0))
            {
                return definitionName;
            }

            var keyValueString = list.Select(x =>
            {
                return string.Format("{0}: \"{1}\"", x.Key, x.Value);
            }).ToArray();

            if (keyValueString == null || (keyValueString != null && keyValueString.Length == 0))
            {
                return "";
            }
            return string.Format("{0};{1} = {{{2}}}", definitionName, objName, string.Join(",", keyValueString));

        }

        public static string GetJSONStringByObj<TEntity>(TEntity entity, string objName = null)
        {
            if (string.IsNullOrWhiteSpace(objName))
            {
                objName = entity.GetType().FullName;
            }
            var definitionName = GetDefinitionName(objName);
            if (entity == null)
            {
                return definitionName;
            }

            return string.Format("{0}{1} = {2}", definitionName, objName, JsonHelper.Serialize(entity));

        }

        /// <summary>
        /// 根据objName 生成Javascript 对象属性
        /// eg:util.xxx,生成,var util={},util.xxx={}
        /// </summary>
        /// <param name="objName">javascript 对象名称</param>
        /// <returns>javascript 对象名称</returns>
        public static string GetDefinitionName(string objName)
        {
            if (string.IsNullOrWhiteSpace(objName))
            {
                return "";
            }

            var names = objName.Split('.');
            var objNames = names.Take(names.Length - 1).Select((x, index) =>
            {
                return string.Join(".", names.Take(index + 1).ToArray());
            });
            var strBuild = new StringBuilder();
            strBuild.AppendFormat("var {0} = {0} || {{}};", objNames.First());
            foreach (var item in objNames.Skip(1))
            {
                strBuild.AppendFormat("{0} = {0} || {{}},", item);
            }
            strBuild = strBuild.TrimEnd(",").TrimEnd(";").Append(";");
            return strBuild.ToString();
        }
    }
}
