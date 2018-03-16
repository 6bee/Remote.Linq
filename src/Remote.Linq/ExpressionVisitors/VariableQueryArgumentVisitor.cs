// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Aqua.TypeSystem;
    using Aqua.TypeSystem.Extensions;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MemberTypes = Aqua.TypeSystem.MemberTypes;
    using PropertyInfo = Aqua.TypeSystem.PropertyInfo;

    public class VariableQueryArgumentVisitor
    {
        internal T ReplaceNonGenericQueryArgumentsByGenericArguments<T>(T expression) where T : Expression
        {
            return (T)new NonGenericVariableQueryArgumentVisitor().ReplaceNonGenericQueryArgumentsByGenericArguments(expression);
        }

        internal T ReplaceGenericQueryArgumentsByNonGenericArguments<T>(T expression) where T : Expression
        {
            return (T)new GenericVariableQueryArgumentVisitor().ReplaceGenericQueryArgumentsByNonGenericArguments(expression);
        }

        protected class GenericVariableQueryArgumentVisitor : RemoteExpressionVisitorBase
        {
            private static readonly PropertyInfo QueryArgumentValuePropertyInfo = new PropertyInfo(typeof(VariableQueryArgument).GetProperty("Value"));
            private static readonly PropertyInfo QueryArgumentValueListPropertyInfo = new PropertyInfo(typeof(VariableQueryArgumentList).GetProperty("Values"));

            internal Expression ReplaceGenericQueryArgumentsByNonGenericArguments(Expression expression)
            {
                return Visit(expression);
            }

            protected override ConstantExpression VisitConstant(ConstantExpression expression)
            {
                if (IsGenericVariableQueryArgument(expression))
                {
                    var valueProperty = expression.Value.GetType().GetProperty("Value");
                    var value = valueProperty.GetValue(expression.Value);

                    object queryArgument;

                    var collection = value as System.Collections.IEnumerable;
                    if (ReferenceEquals(null, collection) || value is string)
                    {
                        queryArgument = new VariableQueryArgument(value, valueProperty.PropertyType);
                    }
                    else
                    {
                        var elementType = TypeHelper.GetElementType(valueProperty.PropertyType);
                        queryArgument = new VariableQueryArgumentList(collection, elementType);
                    }

                    return new ConstantExpression(queryArgument);
                }

                return base.VisitConstant(expression);
            }

            private static bool IsGenericVariableQueryArgument(ConstantExpression expression)
            {
                var type = expression.Type?.Type ?? expression.Value?.GetType();
                return !ReferenceEquals(null, type)
                    && type.IsGenericType()
                    && type.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>);
            }

            protected override Expression VisitMemberAccess(MemberExpression expression)
            {
                if (expression.Expression?.NodeType == ExpressionType.Constant)
                {
                    var member = expression.Member;
                    if (member.MemberType == MemberTypes.Property &&
                        member.DeclaringType.IsGenericType &&
                        member.DeclaringType.Type.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
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
                            throw new Exception(string.Format("Unexpected instance expression: {0}", instanceExpression));
                        }

                        var newMemberExpression = new MemberExpression(instanceExpression, propertyInfo);
                        return newMemberExpression;
                    }
                }

                return base.VisitMemberAccess(expression);
            }
        }

        protected class NonGenericVariableQueryArgumentVisitor : RemoteExpressionVisitorBase
        {
            private static readonly System.Reflection.MethodInfo CreateVariableQueryArgumentListMethodInfo =
                typeof(NonGenericVariableQueryArgumentVisitor).GetMethod(nameof(CreateVariableQueryArgumentList), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            internal Expression ReplaceNonGenericQueryArgumentsByGenericArguments(Expression expression)
            {
                return Visit(expression);
            }

            protected override ConstantExpression VisitConstant(ConstantExpression expression)
            {
                var nonGenericQueryArgument = expression.Value as VariableQueryArgument;
                if (!ReferenceEquals(null, nonGenericQueryArgument))
                {
                    var type = nonGenericQueryArgument.Type.Type;
                    var value = nonGenericQueryArgument.Value;
                    var queryArgument = Activator.CreateInstance(typeof(VariableQueryArgument<>).MakeGenericType(type), new[] { value });
                    return new ConstantExpression(queryArgument);
                }

                var nonGenericQueryArgumentList = expression.Value as VariableQueryArgumentList;
                if (!ReferenceEquals(null, nonGenericQueryArgumentList))
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
                    if (member.DeclaringType.FullName == typeof(VariableQueryArgument).FullName || member.DeclaringType.FullName == typeof(VariableQueryArgumentList).FullName)
                    {
                        var instanceExpression = Visit(expression.Expression) as ConstantExpression;
                        if (!ReferenceEquals(null, instanceExpression))
                        {
                            var instanceType = instanceExpression.Type;
                            if (instanceType.IsGenericType && instanceType.Type.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
                            {
                                var valueType = instanceType.GenericArguments.Single();
                                var valuePropertyInfo = new PropertyInfo("Value", valueType, instanceType);

                                var newMemberExpression = new MemberExpression(instanceExpression, valuePropertyInfo);
                                return newMemberExpression;
                            }
                        }
                    }
                }

                return base.VisitMemberAccess(expression);
            }
        }
    }
}
