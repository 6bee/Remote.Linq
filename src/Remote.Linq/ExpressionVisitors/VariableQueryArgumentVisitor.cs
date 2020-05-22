// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Aqua.Extensions;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using MemberTypes = Aqua.TypeSystem.MemberTypes;
    using PropertyInfo = Aqua.TypeSystem.PropertyInfo;

    public class VariableQueryArgumentVisitor
    {
        internal T ReplaceNonGenericQueryArgumentsByGenericArguments<T>(T expression) where T : Expression
            => (T)new NonGenericVariableQueryArgumentVisitor().ReplaceNonGenericQueryArgumentsByGenericArguments(expression);

        internal T ReplaceGenericQueryArgumentsByNonGenericArguments<T>(T expression) where T : Expression
            => (T)new GenericVariableQueryArgumentVisitor().ReplaceGenericQueryArgumentsByNonGenericArguments(expression);

        protected class GenericVariableQueryArgumentVisitor : RemoteExpressionVisitorBase
        {
            private static readonly PropertyInfo QueryArgumentValuePropertyInfo = new PropertyInfo(typeof(VariableQueryArgument).GetProperty(nameof(VariableQueryArgument.Value)));
            private static readonly PropertyInfo QueryArgumentValueListPropertyInfo = new PropertyInfo(typeof(VariableQueryArgumentList).GetProperty(nameof(VariableQueryArgumentList.Values)));

            internal Expression ReplaceGenericQueryArgumentsByNonGenericArguments(Expression expression) => Visit(expression);

            protected override ConstantExpression VisitConstant(ConstantExpression expression)
            {
                if (IsGenericVariableQueryArgument(expression, out var valueType))
                {
                    var valueProperty = expression.Value?.GetType().GetProperty(nameof(VariableQueryArgument<object>.Value));
                    var value = valueProperty?.GetValue(expression.Value);

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

                return base.VisitConstant(expression);
            }

            private static bool IsGenericVariableQueryArgument(ConstantExpression expression, [NotNullWhen(true)] out Type? valueType)
            {
                var type = expression.Type?.Type ?? expression.Value?.GetType();
                if (type != null &&
                    type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
                {
                    valueType = type.GetGenericArguments().Single();
                    return true;
                }

                valueType = null;
                return false;
            }

            protected override Expression VisitMemberAccess(MemberExpression expression)
            {
                if (expression.Expression?.NodeType == ExpressionType.Constant)
                {
                    var member = expression.Member;
                    if (member.MemberType == MemberTypes.Property &&
                        member.DeclaringType?.IsGenericType == true &&
                        member.DeclaringType?.Type.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
                    {
                        var instanceExpression = (ConstantExpression)Visit(expression.Expression);

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
                            throw new Exception($"Unexpected instance expression: {instanceExpression}");
                        }

                        return new MemberExpression(instanceExpression, propertyInfo);
                    }
                }

                return base.VisitMemberAccess(expression);
            }
        }

        protected class NonGenericVariableQueryArgumentVisitor : RemoteExpressionVisitorBase
        {
            private static readonly System.Reflection.MethodInfo CreateVariableQueryArgumentListMethodInfo =
                typeof(NonGenericVariableQueryArgumentVisitor).GetMethod(nameof(CreateVariableQueryArgumentList), BindingFlags.Static | BindingFlags.NonPublic);

            internal Expression ReplaceNonGenericQueryArgumentsByGenericArguments(Expression expression) => Visit(expression);

            protected override ConstantExpression VisitConstant(ConstantExpression expression)
            {
                if (expression.Value is VariableQueryArgument nonGenericQueryArgument)
                {
                    var type = nonGenericQueryArgument.Type.Type;
                    var value = nonGenericQueryArgument.Value;
                    var queryArgument = Activator.CreateInstance(typeof(VariableQueryArgument<>).MakeGenericType(type), new[] { value });
                    return new ConstantExpression(queryArgument);
                }

                if (expression.Value is VariableQueryArgumentList nonGenericQueryArgumentList)
                {
                    var elementType = nonGenericQueryArgumentList.ElementType.Type;
                    var values = nonGenericQueryArgumentList.Values;
                    var methodInfo = CreateVariableQueryArgumentListMethodInfo.MakeGenericMethod(elementType);
                    var queryArgument = methodInfo.Invoke(null, new object[] { values });
                    return new ConstantExpression(queryArgument);
                }

                return base.VisitConstant(expression);
            }

            private static VariableQueryArgument<List<T>> CreateVariableQueryArgumentList<T>(System.Collections.IEnumerable collection)
            {
                var list = collection.Cast<T>().ToList();
                return new VariableQueryArgument<List<T>>(list);
            }

            protected override Expression VisitMemberAccess(MemberExpression expression)
            {
                var member = expression.Member;
                if (member.MemberType == MemberTypes.Property)
                {
                    if (string.Equals(member.DeclaringType?.FullName, typeof(VariableQueryArgument).FullName) ||
                        string.Equals(member.DeclaringType?.FullName, typeof(VariableQueryArgumentList).FullName))
                    {
                        if (Visit(expression.Expression) is ConstantExpression instanceExpression)
                        {
                            var instanceType = instanceExpression.Type;
                            if (instanceType.IsGenericType && instanceType.Type.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
                            {
                                var valueType = instanceType.GenericArguments.Single();
                                var valuePropertyInfo = new PropertyInfo(nameof(VariableQueryArgument.Value), valueType, instanceType);

                                return new MemberExpression(instanceExpression, valuePropertyInfo);
                            }
                        }
                    }
                }

                return base.VisitMemberAccess(expression);
            }
        }
    }
}
