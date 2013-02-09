using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Remote.Linq.TestConsoleApplication
{
    public static class QueryExtensions
    {
        public static Remote.Linq.Expressions.Expression TranslateAndPrint<T>(Expression<Func<T, bool>> predicate)
        {
            var query = predicate.ToQueryExpression();
            Console.WriteLine(query);

            var exp = query.ToLinqExpression<T, bool>();
            var query2 = exp.ToQueryExpression();
            Console.WriteLine(query2);

            Console.WriteLine();
            Console.WriteLine();

            return query;
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> list, Expression<Func<T, bool>> predicate)
        {
            var result = list.AsQueryable().Where(predicate).AsEnumerable();
            var query = predicate.ToQueryExpression();
            var expression = query.ToLinqExpression<T, bool>();
            return list.AsQueryable().Where(expression).AsEnumerable();
        }

        public static IEnumerable<T> Sort<T, T2>(this IEnumerable<T> list, Expression<Func<T, T2>> orderby)
        {
            var query = orderby.ToQueryExpression();
            var expression = query.ToLinqExpression<T, T2>();
            return list.AsQueryable().OrderByDescending(expression).AsEnumerable();
        }
    }
}
