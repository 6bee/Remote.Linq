// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BindingFlags = System.Reflection.BindingFlags;

namespace Remote.Linq.Dynamic
{
    internal static class SystemExpressionReWriter
    {
        internal static Expression ReplaceAnonymousTypes(this Expression expression)
        {
            return new AnonymousTypesReplacer().ReWrite(expression);
        }

        private sealed class AnonymousTypesReplacer : ExpressionVisitorBase
        {
            private static readonly NewExpression NewDynamicObjectExpression = Expression.New(typeof(DynamicObject));
            private static readonly System.Reflection.ConstructorInfo _keyValuePairContructorInfo = typeof(KeyValuePair<string, object>).GetConstructor(new[] { typeof(string), typeof(object) });
            private static readonly System.Reflection.ConstructorInfo _dynamicObjectContructorInfo = typeof(DynamicObject).GetConstructor(new[] { typeof(IEnumerable<KeyValuePair<string, object>>) });
            private static readonly System.Reflection.MethodInfo DynamicObjectAddMethod = typeof(DynamicObject).GetMethod("Add");
            private static readonly System.Reflection.MethodInfo DynamicObjectGetMethod = typeof(DynamicObject).GetMethod("Get");

            private readonly Dictionary<ParameterExpression, ParameterExpression> _parameterMap = new Dictionary<ParameterExpression, ParameterExpression>();

            public Expression ReWrite(Expression expression)
            {
                Expression projection;
                if (IsProjection(expression))
                {
                    projection = expression;
                }
                else
                {
                    var elementType = TypeHelper.GetElementType(expression.Type);
                    var projectionMethod = MethodInfos.Queryable.Select.MakeGenericMethod(elementType, elementType);
                    var parameter = Expression.Parameter(elementType, "x");
                    var lambda = Expression.Lambda(parameter, parameter);
                    //var quote = Expression.MakeUnary(ExpressionType.Quote, lambda, elementType);
                    projection = Expression.Call(null, projectionMethod, expression, lambda);
                }

                return Visit(projection);
            }

            protected override Expression VisitMemberAccess(MemberExpression m)
            {
                if (m.Member.DeclaringType.IsAnonymousType() || (m.Member.DeclaringType.IsGenericType() && m.Member.DeclaringType.GetGenericArguments().Any(x => x.IsAnonymousType())))
                {
                    var name = m.Member.Name;
                    var instance = Visit(m.Expression);
                    if (instance.Type == typeof(DynamicObject))
                    {
                        var method = DynamicObjectGetMethod;
                        var memberAccessCallExpression = Expression.Call(instance, method, Expression.Constant(name));
                        var type = ReplaceAnonymousType(m.Type);
                        if (type == typeof(object))
                        {
                            return memberAccessCallExpression;
                        }
                        else
                        {
                            var conversionExpression = Expression.Convert(memberAccessCallExpression, type);
                            return conversionExpression;
                        }
                    }
                    else
                    {
                        var member = instance.Type.GetMember(name, m.Member.GetMemberType(), (BindingFlags)0xfffffff);
                        if (member.Count() != 1) throw new Exception(string.Format("Failed to bind {2} {0} of type {1}", name, instance.Type, m.Member.GetMemberType().ToString().ToLower()));
                        return Expression.MakeMemberAccess(instance, member.Single());
                    }
                }
                else
                {
                    return base.VisitMemberAccess(m);
                }
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                ParameterExpression parameter;
                if (!_parameterMap.TryGetValue(p, out parameter))
                {
                    var type = ReplaceAnonymousType(p.Type);
                    if (ReferenceEquals(type, p.Type))
                    {
                        parameter = p;
                    }
                    else
                    {
                        parameter = Expression.Parameter(type, p.Name);
                    }
                    _parameterMap[p] = parameter;
                }
                return parameter;
            }

            protected override Expression VisitMethodCall(MethodCallExpression call)
            {
                var instance = Visit(call.Object);
                var arguments = VisitExpressionList(call.Arguments);
                if (!ReferenceEquals(instance, call.Object) || !ReferenceEquals(arguments, call.Arguments))
                {
                    var method = ReplaceAnonymousType(call.Method);
                    return Expression.Call(instance, method, arguments);
                }
                else
                {
                    return call;
                }
            }

            protected override NewExpression VisitNew(NewExpression newExpression)
            {
                if (newExpression.Type.IsAnonymousType())
                {
                    IEnumerable<Expression> arguments;
                    if (newExpression.Members != null && newExpression.Members.Any())
                    {
                        var args = VisitExpressionList(newExpression.Arguments);
                        arguments = args.Select((a, i) =>
                        {
                            var arg = a.Type == typeof(object) ? a : Expression.Convert(a, typeof(object));
                            return (Expression)Expression.New(
                                _keyValuePairContructorInfo,
                                Expression.Constant(GetMemberName(newExpression.Members[i])),
                                arg);
                        });
                    }
                    else
                    {
                        throw new NotImplementedException("unecpected case");
                    }
                    var argsExpression = Expression.NewArrayInit(typeof(KeyValuePair<string, object>), arguments);
                    var x = Expression.New(_dynamicObjectContructorInfo, argsExpression);
                    return x;
                }
                else
                {
                    return base.VisitNew(newExpression);
                }
            }

            private static string GetMemberName(System.Reflection.MemberInfo memberInfo)
            {
                var name = memberInfo.Name;
                if (memberInfo.GetMemberType() == MemberTypes.Method && name.Length > 4 && name.StartsWith("get_"))
                {
                    name = name.Substring(4, name.Length - 4);
                }
                return name;
            }

            protected override Expression VisitLambda(LambdaExpression lambda)
            {
                var body = Visit(lambda.Body);
                var type = ReplaceAnonymousType(lambda.Type);
                var parameters =
                    from p in lambda.Parameters
                    select Expression.Parameter(ReplaceAnonymousType(p.Type), p.Name);
                return Expression.Lambda(type, body, parameters);
            }


            private System.Reflection.MethodInfo ReplaceAnonymousType(System.Reflection.MethodInfo methodInfo)
            {
                if (methodInfo.IsGenericMethod)
                {
                    var args =
                        from a in methodInfo.GetGenericArguments()
                        select ReplaceAnonymousType(a);
                    return methodInfo.GetGenericMethodDefinition().MakeGenericMethod(args.ToArray());
                }
                else
                {
                    return methodInfo;
                }
            }

            private Type ReplaceAnonymousType(Type type)
            {
                if (type.IsGenericType())
                {
                    if (type.IsAnonymousType())
                    {
                        return typeof(DynamicObject);
                    }
                    else
                    {
                        var args =
                            from a in type.GetGenericArguments()
                            select ReplaceAnonymousType(a);
                        return type.GetGenericTypeDefinition().MakeGenericType(args.ToArray());
                    }
                }
                else
                {
                    return type;
                }
            }

            private static bool IsProjection(Expression expression)
            {
                var call = expression as MethodCallExpression;
                if (call != null)
                {
                    var method = call.Method;
                    if (method.DeclaringType == typeof(System.Linq.Queryable) && method.Name == "Select")
                    {
                        return true;
                    }
                }
                return false;
            }

            //private static Expression CreateProjectionToDynamicObject(Expression sourceExpression, Type resultType, IEnumerable<ParameterExpression> parameters, IEnumerable<KeyValuePair<Expression, Expression>> initMap)
            //{
            //    var inits =
            //        from i in initMap
            //        select Expression.ElementInit(DynamicObjectAddMethod, i.Key, i.Value);
            //    var listInit = Expression.ListInit(NewDynamicObjectExpression, inits);
            //    var listInitExpression = Expression.Lambda(listInit, parameters.ToArray());
            //    var projectionMethod = QueryableSelectMethodInfo.MakeGenericMethod(resultType, typeof(DynamicObject));
            //    var exp = Expression.Call(projectionMethod, sourceExpression, listInitExpression);
            //    return exp;
            //}
        }
    }
}
