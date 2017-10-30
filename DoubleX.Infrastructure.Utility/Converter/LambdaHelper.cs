using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    ///Lambda表达式辅助操作类
    /// </summary>
    public class LambdaHelper
    {
        //ref:http://blog.csdn.net/yl2isoft/article/details/53196092

        //创建lambda表达式：p=>p.propertyName == propertyValue
        //ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "p");//创建参数p
        //MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
        //ConstantExpression constant = Expression.Constant(propertyValue);//创建常数
        //return Expression.Lambda<Func<T, bool>>(Expression.Equal(member, constant), parameter);

        /// <summary>
        /// 动态生成实体查询查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="select"></param>
        /// <param name="value"></param>
        /// <param name="predicateType"></param>
        /// <returns></returns>
        public static Expression<Func<TEntity, bool>> GetCondition<TEntity>(Expression<Func<TEntity, dynamic>> select, object value, EnumConditionPredicateType
       predicateType = EnumConditionPredicateType.等于)
        {
            return GetCondition<TEntity, TEntity>(select, value, predicateType);
        }

        /// <summary>
        /// 动态生成实体查询查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TEntity2"></typeparam>
        /// <param name="select"></param>
        /// <param name="value"></param>
        /// <param name="predicateType"></param>
        /// <returns></returns>
        public static Expression<Func<TEntity, bool>> GetCondition<TEntity, TEntity2>(Expression<Func<TEntity2, dynamic>> select, object value, EnumConditionPredicateType
            predicateType = EnumConditionPredicateType.等于)
        {
            var proInfo = TypesHelper.GetPropertyInfo<TEntity2>(select);
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var member = Expression.PropertyOrField(parameter, proInfo.Name);

            ConstantExpression constant = Expression.Constant(value);

            Expression body = null;
            switch (predicateType)
            {
                case EnumConditionPredicateType.等于:
                default:
                    body = Expression.Equal(member, constant);
                    break;
                case EnumConditionPredicateType.不等于:
                    body = Expression.NotEqual(member, constant);
                    break;
                case EnumConditionPredicateType.大于:
                    body = Expression.GreaterThan(member, constant);
                    break;
                case EnumConditionPredicateType.大于等于:
                    body = Expression.GreaterThanOrEqual(member, constant);
                    break;
                case EnumConditionPredicateType.小于:
                    body = Expression.LessThan(member, constant);
                    break;
                case EnumConditionPredicateType.小于等于:
                    body = Expression.LessThanOrEqual(member, constant);
                    break;
                case EnumConditionPredicateType.包含:
                    //(1)制强是值类型是否支持Contains,即：method 是否正确
                    //MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    //ConstantExpression constant = Expression.Constant(propertyValue, typeof(string));
                    //body = Expression.Call(member, method, constant);
                    //(2)eg2
                    //        var propertyInfo = ReflectionHelper.GetPropertyInfo<TEntity>(select);
                    //        var parameter = Expression.Parameter(typeof(TEntity), "x");
                    //        var left = Expression.Constant(ids);
                    //        var predicate = left.Call("Contains", parameter.Property(propertyInfo.Name));
                    body = constant.Call("Contains", member);
                    break;
                case EnumConditionPredicateType.不包含:
                    body = Expression.Not(constant.Call("Contains", member));
                    break;
            }

            return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
        }
    }

    /// <summary>
    /// 条件判断类型（等于，大于，小于，包含，相似(like)啥的）
    /// </summary>
    public enum EnumConditionPredicateType
    {
        等于,
        不等于,
        大于,
        大于等于,
        小于,
        小于等于,
        包含,
        不包含
    }
}
