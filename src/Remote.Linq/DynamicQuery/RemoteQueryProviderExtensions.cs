// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.ComponentModel;
    using System.Linq;
    using Expression = System.Linq.Expressions.Expression;
    using MethodInfo = System.Reflection.MethodInfo;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class RemoteQueryProviderExtensions
    {
        public static TResult InvokeAndUnwrap<TResult>(this IRemoteQueryProvider queryProvider, MethodInfo method, Expression expression)
            => method
            .MakeGenericMethod(typeof(IQueryable).IsAssignableFrom(expression.Type) ? typeof(object) : expression.Type)
            .InvokeAndUnwrap<TResult>(queryProvider, expression);
    }
}
