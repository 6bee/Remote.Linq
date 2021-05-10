// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Aqua.EnumerableExtensions;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using MemberTypes = Aqua.TypeSystem.MemberTypes;
    using PropertyInfo = Aqua.TypeSystem.PropertyInfo;

    public static class VariableQueryArgumentVisitor
    {
        internal static T ReplaceNonGenericQueryArgumentsByGenericArguments<T>(T expression)
            where T : Expression
            => (T)new NonGenericVariableQueryArgumentVisitor().Run(expression);

        internal static T ReplaceGenericQueryArgumentsByNonGenericArguments<T>(T expression)
            where T : Expression
            => (T)new GenericVariableQueryArgumentVisitor().Run(expression);

        protected class GenericVariableQueryArgumentVisitor : RemoteExpressionVisitorBase
        {
            private static readonly PropertyInfo QueryArgumentValuePropertyInfo = new PropertyInfo(typeof(VariableQueryArgument).GetProperty(nameof(VariableQueryArgument.Value)) !);
            private static readonly PropertyInfo QueryArgumentValueListPropertyInfo = new PropertyInfo(typeof(VariableQueryArgumentList).GetProperty(nameof(VariableQueryArgumentList.Values)) !);

            internal Expression Run(Expression expression) => Visit(expression);

            protected override ConstantExpression VisitConstant(ConstantExpression node)
            {
                if (IsGenericVariableQueryArgument(node.CheckNotNull(nameof(node)), out var valueType))
                {
                    var valueProperty = node.Value?.GetType().GetProperty(nameof(VariableQueryArgument<object>.Value));
                    var value = valueProperty?.GetValue(node.Value);

                    object queryArgument;
                    if (value.IsCollection(out var collection))
                    {
                        var elementType = valueType.GetElementTypeOrThrow();
                        queryArgument = new VariableQueryArgumentList(collection, elementType);
                    }
                    else
                    {
                        queryArgument = new VariableQueryArgument(value, valueProperty?.PropertyType ?? valueType);
                    }

                    return new ConstantExpression(queryArgument);
                }

                return base.VisitConstant(node);
            }

            private static bool IsGenericVariableQueryArgument(ConstantExpression expression, [NotNullWhen(true)] out Type? valueType)
            {
                var type = expression.Type?.ToType() ?? expression.Value?.GetType();
                if (type is not null &&
                    type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
                {
                    valueType = type.GetGenericArguments().Single();
                    return true;
                }

                valueType = null;
                return false;
            }

            protected override Expression VisitMemberAccess(MemberExpression node)
            {
                if (node.CheckNotNull(nameof(node)).Expression?.NodeType == ExpressionType.Constant)
                {
                    var member = node.Member;
                    if (member.MemberType == MemberTypes.Property &&
                        member.DeclaringType?.IsGenericType is true &&
                        member.DeclaringType?.ToType().GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
                    {
                        var instanceExpression = (ConstantExpression)(Visit(node.Expression) ?? throw new InvalidOperationException("Visit must not return null for non null value."));

                        PropertyInfo propertyInfo;
                        if (instanceExpression.Value is VariableQueryArgument)
                        {
                            propertyInfo = QueryArgumentValuePropertyInfo;
                        }
                        else if (instanceExpression.Value is VariableQueryArgumentList)
                        {
                            propertyInfo = QueryArgumentValueListPropertyInfo;
                        }
                        else
                        {
                            throw new RemoteLinqException($"Expected instance expression of type {nameof(VariableQueryArgument)} {nameof(VariableQueryArgumentList)} but got '{instanceExpression.Value?.GetType().FullName ?? "null"}'");
                        }

                        return new MemberExpression(instanceExpression, propertyInfo);
                    }
                }

                return base.VisitMemberAccess(node);
            }
        }

        protected class NonGenericVariableQueryArgumentVisitor : RemoteExpressionVisitorBase
        {
            private static readonly System.Reflection.MethodInfo CreateVariableQueryArgumentListMethodInfo =
                typeof(NonGenericVariableQueryArgumentVisitor).GetMethod(nameof(CreateVariableQueryArgumentList), BindingFlags.Static | BindingFlags.NonPublic) !;

            internal Expression Run(Expression expression) => Visit(expression);

            protected override ConstantExpression VisitConstant(ConstantExpression node)
            {
                if (node.CheckNotNull(nameof(node)).Value is VariableQueryArgument nonGenericQueryArgument)
                {
                    var type = nonGenericQueryArgument.Type.ToType();
                    var value = nonGenericQueryArgument.Value;
                    var queryArgument = Activator.CreateInstance(typeof(VariableQueryArgument<>).MakeGenericType(type), new[] { value });
                    return new ConstantExpression(queryArgument);
                }

                if (node.Value is VariableQueryArgumentList nonGenericQueryArgumentList)
                {
                    var elementType = nonGenericQueryArgumentList.ElementType.ToType();
                    var values = nonGenericQueryArgumentList.Values;
                    var methodInfo = CreateVariableQueryArgumentListMethodInfo.MakeGenericMethod(elementType);
                    var queryArgument = methodInfo.Invoke(null, new object[] { values });
                    return new ConstantExpression(queryArgument);
                }

                return base.VisitConstant(node);
            }

            private static VariableQueryArgument<List<T>> CreateVariableQueryArgumentList<T>(System.Collections.IEnumerable collection)
            {
                var list = collection.Cast<T>().ToList();
                return new VariableQueryArgument<List<T>>(list);
            }

            protected override Expression VisitMemberAccess(MemberExpression node)
            {
                var member = node.CheckNotNull(nameof(node)).Member;
                if (member.MemberType == MemberTypes.Property &&
                    (string.Equals(member.DeclaringType?.FullName, typeof(VariableQueryArgument).FullName, StringComparison.Ordinal) ||
                    string.Equals(member.DeclaringType?.FullName, typeof(VariableQueryArgumentList).FullName, StringComparison.Ordinal)) &&
                    Visit(node.Expression) is ConstantExpression instanceExpression)
                {
                    var instanceType = instanceExpression.Type;
                    if (instanceType.IsGenericType && instanceType.ToType().GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
                    {
                        var valueType = instanceType.GenericArguments!.Single();
                        var valuePropertyInfo = new PropertyInfo(nameof(VariableQueryArgument.Value), valueType, instanceType);

                        return new MemberExpression(instanceExpression, valuePropertyInfo);
                    }
                }

                return base.VisitMemberAccess(node);
            }
        }
    }
}