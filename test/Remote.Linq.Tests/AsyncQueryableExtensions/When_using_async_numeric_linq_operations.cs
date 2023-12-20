// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.AsyncQueryableExtensions;

using Aqua.TypeExtensions;
using Remote.Linq.Async;
using Remote.Linq.TestSupport;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class When_using_async_numeric_linq_operations
{
    [Theory]
    [MemberData(nameof(TestSetup))]
    public async Task Should_perform_async(IScenario scenario, string method, bool withSelector)
    {
        var resut = await scenario.PerformAsyc($"{method}Async", withSelector);
        var expectedResult = scenario.Perform(method, withSelector);
        resut.ShouldBe(expectedResult, $"Method {method}Async{(withSelector ? " with selector" : null)}");
    }

    public static IEnumerable<object[]> TestSetup
    {
        get
        {
            IScenario[] scenarios =
            {
                new ScenarioFor<int>(x => 2 * x, 0, 102, -5),
                new ScenarioFor<int?>(x => 2 * x, null, 102, -5),

                new ScenarioFor<long>(x => 2 * x, 0, 102, -5),
                new ScenarioFor<long?>(x => 2 * x, null, 102, -5),

                new ScenarioFor<float>(x => 2 * x, 0, 1.02f, -5),
                new ScenarioFor<float?>(x => 2 * x, null, 1.02f, -5),

                new ScenarioFor<decimal>(x => 2 * x, 0, 1.02m, -5),
                new ScenarioFor<decimal?>(x => 2 * x, null, 1.02m, -5),

                new ScenarioFor<double>(x => 2 * x, 0, 1.02d, -5),
                new ScenarioFor<double?>(x => 2 * x, null, 1.02d, -5),
            };
            string[] methods = { "Sum", "Average" };
            bool[] withSelectorFlags = { false, true };

            return
                from scenario in scenarios
                from method in methods
                from withSelector in withSelectorFlags
                select new object[] { scenario, method, withSelector };
        }
    }

    public interface IScenario
    {
        ValueTask<object> PerformAsyc(string methodName, bool withSelector);

        object Perform(string methodName, bool withSelector);
    }

    private sealed class ScenarioFor<T> : IScenario
    {
        private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;

        public ScenarioFor(Expression<Func<T, T>> selector, params T[] testData)
        {
            Selector = selector;
            Source = testData;
        }

        private Expression<Func<T, T>> Selector { get; }

        private IEnumerable<T> Source { get; }

        private IQueryable<T> Queryable => Source.AsAsyncRemoteQueryable();

        public async ValueTask<object> PerformAsyc(string methodName, bool withSelector)
        {
            var genericArguments = new List<Type>();
            var parameterTypes = new List<Type> { typeof(IQueryable<T>), typeof(CancellationToken) };
            var parameters = new List<object> { Queryable, default(CancellationToken) };

            var method = typeof(AsyncQueryableExtensions).GetMethods(PublicStatic)
                .Where(x => string.Equals(x.Name, methodName, StringComparison.Ordinal))
                .Where(x => withSelector == (x.GetParameters().Length == 3))
                .Select(x => withSelector ? x.MakeGenericMethod(typeof(T)) : x)
                .Where(x => withSelector
                    ? x.GetParameters().Skip(1).First().ParameterType == typeof(Expression<Func<T, T>>)
                    : x.GetParameters().First().ParameterType == typeof(IQueryable<T>))
                .Single();
            if (withSelector)
            {
                parameters.Insert(1, Selector);
            }

            var task = method.Invoke(null, parameters.ToArray());
            if (task.GetType().Implements(typeof(ValueTask<>), out var args))
            {
                return await TestHelper.GetValueTaskResultAsync(task, args[0]);
            }

            throw new NotSupportedException("Async operation result must be a ValueTask<>");
        }

        public object Perform(string methodName, bool withSelector)
        {
            var genericArguments = new List<Type>();
            var parameterTypes = new List<Type> { typeof(IEnumerable<T>) };
            var parameters = new List<object> { Source };

            var method = typeof(Enumerable).GetMethods(PublicStatic)
                .Where(x => string.Equals(x.Name, methodName, StringComparison.Ordinal))
                .Where(x => withSelector == (x.GetParameters().Length == 2))
                .Select(x => withSelector ? x.MakeGenericMethod(typeof(T)) : x)
                .Where(x => withSelector
                    ? x.GetParameters().Skip(1).First().ParameterType == typeof(Func<T, T>)
                    : x.GetParameters().First().ParameterType == typeof(IEnumerable<T>))
                .Single();
            if (withSelector)
            {
                parameters.Add(Selector.Compile());
            }

            var result = method.Invoke(null, parameters.ToArray());
            return result;
        }
    }
}