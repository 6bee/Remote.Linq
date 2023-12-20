// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors;

using Aqua.EnumerableExtensions;
using Aqua.TypeExtensions;
using Aqua.TypeSystem;
using Remote.Linq.DynamicQuery;
using Remote.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MemberTypes = Aqua.TypeSystem.MemberTypes;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = Aqua.TypeSystem.PropertyInfo;

public abstract class VariableQueryArgumentVisitor
{
    internal static T ReplaceNonGenericQueryArgumentsByGenericArguments<T>(T expression, ITypeResolver? typeResolver)
        where T : Expression
        => (T)new NonGenericVariableQueryArgumentVisitor(typeResolver).Run(expression);

    internal static T ReplaceGenericQueryArgumentsByNonGenericArguments<T>(T expression, ITypeResolver? typeResolver)
        where T : Expression
        => (T)new GenericVariableQueryArgumentVisitor(typeResolver).Run(expression);

    protected class GenericVariableQueryArgumentVisitor : RemoteExpressionVisitorBase
    {
        private static readonly System.Reflection.PropertyInfo QueryArgumentValuePropertyInfo = typeof(VariableQueryArgument).GetProperty(nameof(VariableQueryArgument.Value))!;
        private static readonly System.Reflection.PropertyInfo QueryArgumentValueListPropertyInfo = typeof(VariableQueryArgumentList).GetProperty(nameof(VariableQueryArgumentList.Values))!;

        public GenericVariableQueryArgumentVisitor(ITypeResolver? typeResolver)
            : base(typeResolver)
        {
        }

        public Expression Run(Expression expression) => Visit(expression);

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (IsGenericVariableQueryArgument(node.CheckNotNull(), out var valueType))
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

        private bool IsGenericVariableQueryArgument(ConstantExpression expression, [NotNullWhen(true)] out Type? valueType)
        {
            var type = expression.Type?.ResolveType(TypeResolver) ?? expression.Value?.GetType();
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
            if (node.CheckNotNull().Expression?.NodeType is ExpressionType.Constant)
            {
                var member = node.Member;
                if (member.MemberType is MemberTypes.Property &&
                    member.DeclaringType?.IsGenericType is true &&
                    member.DeclaringType?.ResolveType(TypeResolver).GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
                {
                    var instanceExpression = (ConstantExpression)(Visit(node.Expression) ?? throw new InvalidOperationException("Visit must not return null for non null value."));

                    PropertyInfo propertyInfo;
                    if (instanceExpression.Value is VariableQueryArgument)
                    {
                        propertyInfo = new PropertyInfo(QueryArgumentValuePropertyInfo);
                    }
                    else if (instanceExpression.Value is VariableQueryArgumentList)
                    {
                        propertyInfo = new PropertyInfo(QueryArgumentValueListPropertyInfo);
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
        private static readonly MethodInfo CreateVariableQueryArgumentListMethodInfo =
            typeof(NonGenericVariableQueryArgumentVisitor).GetMethodEx(nameof(CreateVariableQueryArgumentList));

        public NonGenericVariableQueryArgumentVisitor(ITypeResolver? typeResolver)
            : base(typeResolver)
        {
        }

        public Expression Run(Expression expression) => Visit(expression);

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.CheckNotNull().Value is VariableQueryArgument nonGenericQueryArgument)
            {
                var type = nonGenericQueryArgument.Type.ResolveType(TypeResolver);
                var value = nonGenericQueryArgument.Value;
                var queryArgument = Activator.CreateInstance(typeof(VariableQueryArgument<>).MakeGenericType(type), new[] { value });
                return new ConstantExpression(queryArgument);
            }

            if (node.Value is VariableQueryArgumentList nonGenericQueryArgumentList)
            {
                var elementType = nonGenericQueryArgumentList.ElementType.ResolveType(TypeResolver);
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
            var member = node.CheckNotNull().Member;
            if (member.MemberType == MemberTypes.Property &&
                (string.Equals(member.DeclaringType?.FullName, typeof(VariableQueryArgument).FullName, StringComparison.Ordinal) ||
                string.Equals(member.DeclaringType?.FullName, typeof(VariableQueryArgumentList).FullName, StringComparison.Ordinal)) &&
                Visit(node.Expression) is ConstantExpression instanceExpression)
            {
                var instanceType = instanceExpression.Type;
                if (instanceType.IsGenericType && instanceType.ResolveType(TypeResolver).GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
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