using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// Linq工具类
    /// </summary>
    public class LinqHelper
    {
        #region 条件构建

        /// <summary>
        /// 构建相等查询条件(=)
        /// </summary>
        public static Expression<Func<TEntity, bool>> GetEqualPredicate<TEntity>(Expression<Func<TEntity, bool>> source, Expression<Func<TEntity, dynamic>> select, object obj)
        {
            Expression<Func<TEntity, bool>> condition = null;

            //构建条件  x=>x==obj;

            //构建x(实体信息)
            var parameter = Expression.Parameter(typeof(TEntity), "x");

            //构建=(操作方法)
            MemberExpression member = Expression.Property(parameter, TypesHelper.GetPropertyInfo<TEntity>(select).Name);

            //构建obj(值)
            var constant = Expression.Constant(obj);

            //构建条件
            condition = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(member, constant), parameter);

            return source.And<TEntity>(condition);
        }

        #endregion

        #region 排序处理

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
            if (sortDir.ToLower().Trim() == "asc")
                sortingDir = isMutile ? "ThenBy" : "OrderBy";
            else if (sortDir.ToLower().Trim() == "desc")
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
