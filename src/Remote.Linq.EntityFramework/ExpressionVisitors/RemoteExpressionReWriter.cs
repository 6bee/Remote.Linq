// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.ExpressionVisitors;

using Aqua.TypeSystem;
using Remote.Linq.Expressions;
using Remote.Linq.ExpressionVisitors;
using Remote.Linq.Include;
using System.ComponentModel;
using SystemLinq = System.Linq.Expressions;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class RemoteExpressionReWriter
{
    /// <summary>
    /// Map include query methods for entity framework.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     1:
    ///     Replaces method <see cref="IncludeQueryableExtensions.ThenInclude{T, TPreviousProperty, TProperty}(IIncludableQueryable{T, TPreviousProperty}, SystemLinq.Expression{Func{TPreviousProperty, TProperty}})"/>
    ///     and <see cref="IncludeQueryableExtensions.ThenInclude{T, TPreviousProperty, TProperty}(IIncludableQueryable{T, TPreviousProperty}, SystemLinq.Expression{Func{TPreviousProperty, TProperty}})"/>
    ///     by <see cref="IncludeQueryableExtensions.Include{T}(IQueryable{T}, string)"/> with sub-selects.
    ///   </para>
    ///   <para>
    ///     2:
    ///     Replaces include query methods by entity framework's include methods.
    ///   </para>
    /// </remarks>
    public static Expression MapIncludeQueryMethods(this Expression expression, ITypeResolver? typeResolver)
        => expression
        .ReplaceIncludeQueryMethodsByStringInclude(typeResolver: typeResolver)
        .ReplaceIncludeQueryMethods(typeResolver);

    /// <summary>
    /// Replaces include query methods by entity framework's include methods.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to be examined.</param>
    /// <param name="typeResolver">Type resolver.</param>
    /// <returns>A new expression tree with the EF's version of the include method if any method calls to include was contained in the original expression,
    /// otherwise the original expression tree is returned.</returns>
    private static Expression ReplaceIncludeQueryMethods(this Expression expression, ITypeResolver? typeResolver)
        => new QueryMethodMapper(typeResolver).Run(expression);

    private sealed class QueryMethodMapper(ITypeResolver? typeResolver) : RemoteExpressionVisitorBase(typeResolver)
    {
        internal Expression Run(Expression expression) => Visit(expression);

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (MapRemoteLinqToEntityFrameworkMethod(node.Method) is MethodInfo mappedMethod)
            {
                var arguments = node.Arguments?.Select(Visit).ToArray();
                node = new MethodCallExpression(null, mappedMethod, arguments!);
            }

            return base.VisitMethodCall(node);

            MethodInfo? MapRemoteLinqToEntityFrameworkMethod(MethodInfo source)
            {
                if (source is null)
                {
                    return null;
                }

                if (!string.Equals(source.DeclaringType?.FullName, typeof(IncludeQueryableExtensions).FullName))
                {
                    return null;
                }

                var method = source.ResolveMethod(TypeResolver) ?? throw new TypeResolverException($"Failed to resolve method '{source}'");
                var target = MapMethod(method);
                if (target is null)
                {
                    return null;
                }

                var genericArguments = method.GetGenericArguments();
                return new MethodInfo(target.MakeGenericMethod(genericArguments));

                static System.Reflection.MethodInfo? MapMethod(System.Reflection.MethodInfo source)
                {
                    var method = source.GetGenericMethodDefinition();

                    if (method == IncludeQueryableExtensions.IncludeMethodInfo)
                    {
                        return EntityFrameworkMethodInfos.IncludeMethodInfo;
                    }

                    if (method == IncludeQueryableExtensions.StringIncludeMethodInfo)
                    {
                        return EntityFrameworkMethodInfos.StringIncludeMethodInfo;
                    }

                    return null;
                }
            }
        }
    }
}