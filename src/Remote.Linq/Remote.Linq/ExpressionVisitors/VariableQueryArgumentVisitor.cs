// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Remote.Linq.Expressions;
    using Remote.Linq.TypeSystem;
    using System;
    using System.Linq;
    using System.Collections.Generic;

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
                var type = expression.Type;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
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

                    return Expression.Constant(queryArgument);
                }

                return base.VisitConstant(expression);
            }

            protected override Expression VisitMemberAccess(MemberExpression expression)
            {
                var member = expression.Member;
                if (member.MemberType == MemberTypes.Property && member.DeclaringType.IsGenericType && member.DeclaringType.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
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

                return base.VisitMemberAccess(expression);
            }
        }

        protected class NonGenericVariableQueryArgumentVisitor : RemoteExpressionVisitorBase
        {
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
                    return Expression.Constant(queryArgument);
                }

                var nonGenericQueryArgumentList = expression.Value as VariableQueryArgumentList;
                if (!ReferenceEquals(null, nonGenericQueryArgumentList))
                {
                    var elementType = nonGenericQueryArgumentList.ElementType.Type;
                    var values = nonGenericQueryArgumentList.Values;
                    //var listType = typeof(List<>).MakeGenericType(elementType);
                    ////TODO: create typed list
                    //var queryArgument = Activator.CreateInstance(typeof(VariableQueryArgument<>).MakeGenericType(listType), new[] { values });
                    //return Expression.Constant(queryArgument);

                    //var elementTypeParameterExpression = System.Linq.Expressions.Expression.Parameter(typeof(Type), "elementType");
                    //var valuesParameterExpression = System.Linq.Expressions.Expression.Parameter(typeof(System.Collections.IEnumerable), "values");


                    //var castValuesExpression = System.Linq.Expressions.Expression.Call(null, MethodInfos.Enumerable.Cast)
                    //var valuesToListExpression = System.Linq.Expressions.Expression.Call(null, MethodInfos.Enumerable.ToList, castValuesExpression);


                    //var bodyExpression = System.Linq.Expressions.Expression.Constant(null);                
                    //var exp = System.Linq.Expressions.Expression.Lambda<Func<Type, System.Collections.IEnumerable, ConstantExpression>>(bodyExpression, elementTypeParameterExpression, valuesParameterExpression);
                    //var func = exp.Compile();
                    //var result = func(elementType, values);
                    //return result;

                    var methodInfo = CreateVariableQueryArgumentListMethodInfo.MakeGenericMethod(elementType);
                    var queryArgument = methodInfo.Invoke(null, new object[] { values });
                    return Expression.Constant(queryArgument);
                }

                return base.VisitConstant(expression);
            }

            private static readonly System.Reflection.MethodInfo CreateVariableQueryArgumentListMethodInfo = typeof(NonGenericVariableQueryArgumentVisitor).GetMethod("CreateVariableQueryArgumentList", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            private static VariableQueryArgument<List<T>> CreateVariableQueryArgumentList<T>(System.Collections.IEnumerable collection)
            {
                var list = collection.Cast<T>().ToList();
                return new VariableQueryArgument<List<T>>(list);
            }

            //private static readonly System.Linq.Expressions.Expression<Func<Type, System.Collections.IEnumerable, ConstantExpression>> _exp = System.Linq.Expressions.Expression.Lambda<Func<Type, System.Collections.IEnumerable, ConstantExpression>>()

            protected override Expression VisitMemberAccess(MemberExpression expression)
            {
                var member = expression.Member;
                //if (member.MemberType == MemberTypes.Property && member.DeclaringType.FullName == typeof(VariableQueryArgument).FullName)
                if (member.MemberType == MemberTypes.Property)
                {
                    if (member.DeclaringType.FullName == typeof(VariableQueryArgument).FullName || member.DeclaringType.FullName == typeof(VariableQueryArgumentList).FullName)
                    {
                        var instanceExpression = Visit(expression.Expression) as ConstantExpression;
                        if (!ReferenceEquals(null, instanceExpression))
                        {
                            var instanceType = instanceExpression.Type;
                            if (instanceType.IsGenericType && instanceType.GetGenericTypeDefinition() == typeof(VariableQueryArgument<>))
                            {
                                var valuePropertyInfo = new PropertyInfo("Value", instanceType);

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
