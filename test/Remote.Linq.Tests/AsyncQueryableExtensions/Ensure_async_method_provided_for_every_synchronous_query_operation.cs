// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.AsyncQueryableExtensions
{
    using Remote.Linq;
    using Remote.Linq.Async;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class Ensure_async_method_provided_for_every_synchronous_query_operation
    {
        private class TOne
        {
        }

        private class TTwo
        {
        }

        private class TThree
        {
        }

        [Theory]
        [MemberData(nameof(QueryableOperations))]
        public void Method_should_have_async_counterpart(MethodInfo method)
        {
            var typeList = new[] { typeof(TOne), typeof(TTwo), typeof(TThree) };
            var genericArgumentTypes = method.IsGenericMethodDefinition
                ? typeList.Take(method.GetGenericArguments().Length).ToArray()
                : Array.Empty<Type>();

            var methodName = $"{method.Name}Async";
            var matchingMethods = typeof(AsyncQueryableExtensions)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(x => string.Equals(x.Name, methodName, StringComparison.Ordinal))
                .ToArray();
            matchingMethods.ShouldNotBeEmpty($"No method found with name '{methodName}'");

            if (genericArgumentTypes.Any())
            {
                matchingMethods = matchingMethods
                    .Where(x => x.IsGenericMethodDefinition && x.GetGenericArguments().Length == genericArgumentTypes.Length)
                    .ToArray();
                matchingMethods.ShouldNotBeEmpty($"No '{methodName}' query operation found with {genericArgumentTypes.Length} generic type arguments.");

                method = method.MakeGenericMethod(genericArgumentTypes);
                matchingMethods = matchingMethods
                    .Select(x => x.MakeGenericMethod(genericArgumentTypes))
                    .ToArray();
            }

            var returnType = typeof(ValueTask<>).MakeGenericType(method.ReturnType);
            matchingMethods = matchingMethods
                .Where(x => x.ReturnType == returnType)
                .ToArray();
            matchingMethods.ShouldNotBeEmpty($"No '{methodName}' query operation found with return type {returnType}");

            var numberOfArguments = method.GetParameters().Length + 1;
            matchingMethods = matchingMethods
                .Where(x => x.GetParameters().Length == numberOfArguments)
                .ToArray();
            matchingMethods.ShouldNotBeEmpty($"No '{methodName}' query operation found with {numberOfArguments} arguments");

            matchingMethods = matchingMethods
                .Where(x =>
                {
                    var parameters = x.GetParameters();
                    if (parameters.Last().ParameterType != typeof(CancellationToken))
                    {
                        return false;
                    }

                    var expectedParameters = method.GetParameters();
                    for (int i = 0; i < expectedParameters.Length; i++)
                    {
                        if (parameters[i].ParameterType != expectedParameters[i].ParameterType)
                        {
                            return false;
                        }
                    }

                    return true;
                })
                .ToArray();
            matchingMethods.ShouldNotBeEmpty($"No '{methodName}' query operation found matching argument list ({string.Join(", ", method.GetParameters().Select(x => x.ParameterType.Name))}, {nameof(CancellationToken)})");

            if (matchingMethods.Length != 1)
            {
                throw new RemoteLinqException($"Implementation error: should have exactly one matching method left at this point: '{methodName}'");
            }
        }

        public static IEnumerable<object[]> QueryableOperations
            => typeof(Queryable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(x => !typeof(IQueryable).IsAssignableFrom(x.ReturnType))
                .Select(x => new[] { x });
    }
}
