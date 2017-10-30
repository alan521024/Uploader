using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// 集合扩展方法
    /// </summary>
    public static class CollectionExtensions
    {
        #region IEnumerable的扩展
        /// <summary>
        /// 检查集合是否为空
        /// </summary>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count <= 0;
        }

        /// <summary>
        ///添加项到集合(如果该项不存在)
        /// </summary>
        public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source.Contains(item))
            {
                return false;
            }

            source.Add(item);
            return true;
        }

        #endregion

        #region IQueryable的扩展

        /// <summary>
        /// 字段排序
        /// </summary>
        public static IQueryable<T> Sorting<T>(IQueryable<T> source, List<KeyValuePair<string, string>> orderList, string defaultField = "Id")
        {
            if (orderList == null || (orderList != null && orderList.Count() == 0))
            {
                return Sorting<T>(source, defaultField: defaultField);
            }

            for (var i = 0; i < orderList.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(orderList[i].Key))
                {
                    source = Sorting<T>(source,
                        orderList[i].Key,
                        string.IsNullOrWhiteSpace(orderList[i].Value) ? "desc" : orderList[i].Value,
                        defaultField, i > 0);
                }
            }

            return source;
        }

        /// <summary>
        /// 字段排序
        /// </summary>
        public static IQueryable<T> Sorting<T>(IQueryable<T> source, string sortField = null, string sortDir = "Desc", string defaultField = "Id", bool isMutile = false)
        {
            if (string.IsNullOrWhiteSpace(sortField))
            {
                sortField = defaultField;
            }

            string sortingDir = string.Empty;
            if (sortDir.ToUpper().Trim() == "asc")
                sortingDir = isMutile ? "ThenBy" : "OrderBy";
            else if (sortDir.ToUpper().Trim() == "desc")
                sortingDir = isMutile ? "ThenByDescending" : "OrderByDescending";
            else
                sortingDir = isMutile ? "ThenByDescending" : "OrderByDescending";

            var properties = typeof(T).GetProperties();
            PropertyInfo keyPropertie = null;
            if (!string.IsNullOrWhiteSpace(sortField))
            {
                keyPropertie = properties.Where(x => x.Name.ToLower().Trim() == sortField.ToLower().Trim()).FirstOrDefault();
            }

            //属性未找到使用默认第一个属性
            if (keyPropertie == null)
            {
                keyPropertie = properties.FirstOrDefault();
                sortingDir = keyPropertie.Name;
            }

            IQueryable<T> query = source;

            if (keyPropertie != null)
            {
                Type[] types = new Type[2];
                types[0] = typeof(T);
                types[1] = keyPropertie.PropertyType;

                ParameterExpression param = Expression.Parameter(typeof(T), sortField);

                Expression expr = Expression.Call(typeof(Queryable), sortingDir, types, source.Expression, Expression.Lambda(Expression.Property(param, sortField), param));
                query = source.AsQueryable().Provider.CreateQuery<T>(expr);
            }
            return query;
        }

        #endregion
    }
}
