// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua;
    using Remote.Linq.DynamicQuery;
    using System.Collections.Generic;
    using System.Linq;
    using BindingFlags = System.Reflection.BindingFlags;
    using MethodInfo = System.Reflection.MethodInfo;
    
    internal static class MethodInfos
    {
        internal static class Enumerable
        {
            internal static readonly MethodInfo Cast = typeof(System.Linq.Enumerable)
                .GetMethod("Cast", BindingFlags.Public | BindingFlags.Static);
            
            internal static readonly MethodInfo OfType = typeof(System.Linq.Enumerable)
                .GetMethod("OfType", BindingFlags.Public | BindingFlags.Static);

            internal static readonly MethodInfo ToArray = typeof(System.Linq.Enumerable)
                .GetMethod("ToArray", BindingFlags.Public | BindingFlags.Static);

            internal static readonly MethodInfo ToList = typeof(System.Linq.Enumerable)
                .GetMethod("ToList", BindingFlags.Public | BindingFlags.Static);
            
            internal static readonly MethodInfo Contains = typeof(System.Linq.Enumerable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(m => m.Name == "Contains" && m.GetParameters().Length == 2);
            
            internal static readonly MethodInfo Single = typeof(System.Linq.Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(x => x.Name == "Single")
                .Where(x =>
                {
                    var parameters = x.GetParameters();
                    return parameters.Length == 1
                        && parameters[0].ParameterType.IsGenericType()
                        && parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
                })
                .Single();

            internal static readonly MethodInfo SingleOrDefault = typeof(System.Linq.Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(x => x.Name == "SingleOrDefault")
                .Where(x =>
                {
                    var parameters = x.GetParameters();
                    return parameters.Length == 1
                        && parameters[0].ParameterType.IsGenericType()
                        && parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
                })
                .Single();
        }

        internal static class Expression
        {
            internal static readonly MethodInfo Lambda = typeof(System.Linq.Expressions.Expression)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(m =>
                    m.Name == "Lambda" &&
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[1].ParameterType == typeof(System.Linq.Expressions.ParameterExpression[]));
        }

        internal static class Queryable
        {
            internal static readonly MethodInfo OrderBy = typeof(System.Linq.Queryable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(m =>
                    m.Name == "OrderBy" &&
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 2);

            internal static readonly MethodInfo OrderByDescending = typeof(System.Linq.Queryable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(m =>
                    m.Name == "OrderByDescending" &&
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 2);

            internal static readonly MethodInfo ThenBy = typeof(System.Linq.Queryable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(m =>
                    m.Name == "ThenBy" &&
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 2);

            internal static readonly MethodInfo ThenByDescending = typeof(System.Linq.Queryable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(m =>
                    m.Name == "ThenByDescending" &&
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 2);

            internal static readonly MethodInfo Select = typeof(System.Linq.Queryable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(i => i.Name == "Select")
                .Where(i => i.IsGenericMethod)
                .Single(i =>
                {
                    var parameters = i.GetParameters();
                    if (parameters.Length != 2) return false;
                    var expressionParamType = parameters[1].ParameterType;
                    if (!expressionParamType.IsGenericType()) return false;
                    var genericArguments = expressionParamType.GetGenericArguments().ToArray();
                    if (genericArguments.Count() != 1) return false;
                    if (!genericArguments.Single().IsGenericType()) return false;
                    if (genericArguments.Single().GetGenericArguments().Count() != 2) return false;
                    return true;
                });
        }

        internal static class String
        {
            internal static readonly MethodInfo StartsWith = typeof(string)
                .GetMethod("StartsWith", new[] { typeof(string) });

            internal static readonly MethodInfo EndsWith = typeof(string)
                .GetMethod("EndsWith", new[] { typeof(string) });

            internal static readonly MethodInfo Contains = typeof(string)
                .GetMethod("Contains", new[] { typeof(string) });            
        }

        internal static class QueryFuntion
        {
            internal static readonly MethodInfo Include = typeof(QueryFunctions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.Name == "Include")
                .Where(x => x.IsGenericMethod && x.GetGenericArguments().Length == 1)
                .Where(x =>
                {
                    var parameters = x.GetParameters().ToArray();
                    return parameters.Length == 2
                        && parameters[1].ParameterType == typeof(string);
                })
                .Single();
        }
    }
}
