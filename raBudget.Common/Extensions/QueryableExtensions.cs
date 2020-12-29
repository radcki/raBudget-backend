using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using raBudget.Common.Query;

namespace raBudget.Common.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyGridQueryOptions<T>(this IQueryable<T> source, GridQuery options)
        {
            if (options.DataOrder != null && options.DataOrder.Any())
            {
                return source.ApplyOrder(options.DataOrder).TakePage(options.Page, options.PageSize);
            }
            return source.TakePage(options.Page, options.PageSize);
        }

        public static IOrderedQueryable<T> ApplyOrder<T>
        (
            this IQueryable<T> source,
            List<FieldOrderInfo> orderInfo)
        {
            var firstOrder = orderInfo.First();
            var ordered = firstOrder.Descending
                              ? source.OrderByDescending(firstOrder.FieldName)
                              : source.OrderBy(firstOrder.FieldName);
            if (orderInfo.Count == 0)
            {
                return ordered;
            }

            for (var index = 1; index < orderInfo.Count; index++)
            {
                var fieldOrderInfo = orderInfo[index];
                ordered = fieldOrderInfo.Descending
                              ? ordered.ThenByDescending(fieldOrderInfo.FieldName)
                              : ordered.ThenBy(fieldOrderInfo.FieldName);
            }

            return ordered;
        }

        public static IQueryable<TSource> TakePage<TSource>(this IQueryable<TSource> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static IOrderedQueryable<T> OrderBy<T>
        (
            this IQueryable<T> source,
            string property)
        {
            return ApplyOrder<T>(source, property, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>
        (
            this IQueryable<T> source,
            string property)
        {
            return ApplyOrder<T>(source, property, "OrderByDescending");
        }

        public static IOrderedQueryable<T> ThenBy<T>
        (
            this IOrderedQueryable<T> source,
            string property)
        {
            return ApplyOrder<T>(source, property, "ThenBy");
        }

        public static IOrderedQueryable<T> ThenByDescending<T>
        (
            this IOrderedQueryable<T> source,
            string property)
        {
            return ApplyOrder<T>(source, property, "ThenByDescending");
        }

        public static IOrderedQueryable<T> ApplyOrder<T>
        (
            IQueryable<T> source,
            string property,
            string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                PropertyInfo pi = type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods()
                                             .Single(method => method.Name == methodName
                                                               && method.IsGenericMethodDefinition
                                                               && method.GetGenericArguments().Length == 2
                                                               && method.GetParameters().Length == 2)
                                             .MakeGenericMethod(typeof(T), type)
                                             .Invoke(null, new object[] {source, lambda});
            return (IOrderedQueryable<T>) result;
        }
    }
}