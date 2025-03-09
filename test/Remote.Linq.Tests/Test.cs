// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#nullable enable
namespace Remote.Linq.Tests;

using global::Newtonsoft.Json;
using Remote.Linq.ExpressionExecution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

public class Test
{
    [Fact]
    public void Run()
    {
        // define an IQueryable which is not an in-memory collection, possibly using a custom implementation of IQueryable<>
        IQueryable<Person> peopleQuery = new MyQueryable<Person>();

        // alternatively you may use a IRemoteQueryable instead of creating your own queryable type
        //// var peopleQuery = Remote.Linq.RemoteQueryable.Factory
        ////    .CreateQueryable<Person>(_ => throw new NotSupportedException("this remote queryable is not meant for direct execution"))
        ////    .AsQueryable();

        peopleQuery = peopleQuery.Where(x => x.Age > 35);
        peopleQuery = peopleQuery.Where(x => x.Name.StartsWith("R"));

        var expression = peopleQuery.Expression;
        Console.WriteLine(expression);

        // use ExpressionTranslator so expression not only gets translated but the queryable (i.e. the data source)
        // gets substituted by an instance of QueryableResourceDescriptor
        var remoteExpression = new Remote.Linq.DynamicQuery.ExpressionTranslator().TranslateExpression(expression);
        var serializedExpression = remoteExpression.ToJson();
        Console.WriteLine(serializedExpression);

        var deserializedExpression = serializedExpression.FromJson<Remote.Linq.Expressions.MethodCallExpression>() ?? throw new Exception("JSON must not be null");
        var remoteList = GetPeople();
        var remoteQuery = remoteList.AsQueryable();
        Func<Type, IQueryable> queryableProvider = type => remoteQuery;

        // By using execute extension method, the QueryableResourceDescriptor gets replaced by the value received via queryableProvider
        // before the expression is eventually executed. The result is then based on the queryable provided (i.e. the remoteQuery)
        // with the deserialized expression applied.
        var result = deserializedExpression.Execute(queryableProvider: queryableProvider);
        var remoteResult = result.GetValues().Count;
        Console.WriteLine($"Remote result: {remoteResult}");

        // Use an overload of Execute to change result type if required.
        var typedResult1 = deserializedExpression.Execute<object>(queryableProvider: queryableProvider);
        var typedResult2 = deserializedExpression.Execute<Person[]>(queryableProvider: queryableProvider);
    }

    public static List<Person> GetPeople()
        => new List<Person>
        {
            new Person("Rachel", 25),
            new Person("Joey", 24),
            new Person("Chandler", 38),
            new Person("Phoebe", 31),
            new Person("Ross", 50),
            new Person("Monica", 60),
        };
}

#pragma warning disable SA1402 // File may only contain a single type
public sealed class MyQueryable<T> : IQueryable<T>, IQueryProvider
{
    public MyQueryable() => Expression = Expression.Constant(this);

    private MyQueryable(Expression expression) => Expression = expression;

    public Expression Expression { get; }

    Type IQueryable.ElementType => typeof(T);

    IQueryProvider IQueryable.Provider => this;

    IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression) => new MyQueryable<TElement>(expression);

    IQueryable IQueryProvider.CreateQuery(Expression expression) => throw new NotSupportedException();

    object? IQueryProvider.Execute(Expression expression) => throw new NotSupportedException();

    TResult IQueryProvider.Execute<TResult>(Expression expression) => throw new NotSupportedException();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
}

public record Person(string Name, int Age);

public static class ExpressionHelper
{
    public static TExpression? DeepCopy<TExpression>(TExpression expression)
        where TExpression : Remote.Linq.Expressions.Expression
    {
        var json = ToJson(expression);
        return FromJson<TExpression>(json);
    }

    public static string ToJson<TExpression>(this TExpression expression)
        where TExpression : Remote.Linq.Expressions.Expression
    {
        var serializerSettings = new JsonSerializerSettings().ConfigureRemoteLinq();
        return JsonConvert.SerializeObject(expression, serializerSettings);
    }

    public static TExpression? FromJson<TExpression>(this string json)
        where TExpression : Remote.Linq.Expressions.Expression
    {
        var serializerSettings = new JsonSerializerSettings().ConfigureRemoteLinq();
        return JsonConvert.DeserializeObject<TExpression>(json, serializerSettings);
    }
}
#pragma warning restore SA1402 // File may only contain a single type