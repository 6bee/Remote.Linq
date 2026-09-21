// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#nullable enable
namespace Remote.Linq.Tests.ExpressionVisitors.ExpressionTranslator;

using Aqua.Dynamic;
using Remote.Linq.DynamicQuery;
using System.Collections.Generic;
using System.Linq.Expressions;
using RemoteLinq = Remote.Linq.Expressions;

public class When_translating_complex_constants : ExpressionTranslatorTestBase
{
    private class ProbeDto
    {
        public int Count { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    [QueryArgument]
    private class QueryArgumentDto
    {
        public int Count { get; set; }
    }

    [Fact]
    public void Should_translate_type_expression_and_expression_collection_constants()
    {
        // Type constant: translated into a TypeInfo carrying the same type on the remote side.
        // The NoMappingContext disables local evaluation, so the constant keeps its shape.
        var typeRemote = (RemoteLinq.ConstantExpression)Expression.Constant(typeof(int), typeof(Type)).ToRemoteLinqExpression(ExpressionTranslatorContext.NoMappingContext);
        typeRemote.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        typeRemote.Type.ToType().ShouldBe(typeof(Type));
        typeRemote.Value.ShouldNotBeNull().ShouldBeOfType<Aqua.TypeSystem.TypeInfo>().ToType().ShouldBe(typeof(int));

        var (typeOriginal, typeRoundTrip) = BackAndForth(Expression.Constant(typeof(int), typeof(Type)), ExpressionTranslatorContext.NoMappingContext);
        typeOriginal.Value.ShouldBe(typeof(int));
        typeRoundTrip.Value.ShouldBe(typeof(int));

        // Under the default context plain constants are not rewritten by the partial
        // evaluation pass, so the type constant still takes the type-specific branch and the
        // value roundtrips back to the original type.
        var defaultTypeRemote = Expression.Constant(typeof(int), typeof(Type)).ToRemoteLinqExpression();
        defaultTypeRemote.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        var defaultTypeConstant = defaultTypeRemote.ShouldBeOfType<RemoteLinq.ConstantExpression>();
        defaultTypeConstant.Type.ToType().ShouldBe(typeof(Type));
        defaultTypeConstant.Value.ShouldNotBeNull().ShouldBeAssignableTo<Aqua.TypeSystem.TypeInfo>().ToType().ShouldBe(typeof(int));
        Expression.Lambda<Func<Type>>(defaultTypeRemote.ToLinqExpression()).Compile().Invoke().ShouldBe(typeof(int));

        // Expression constant: the inner expression references the lambda parameter and cannot be
        // locally evaluated, so it is translated recursively and retained as a remote expression.
        var p = Expression.Parameter(typeof(int), "p");
        var inner = Expression.Lambda<Func<int, int>>(Expression.Multiply(p, Expression.Constant(2)), p);
        var expressionRemote = (RemoteLinq.ConstantExpression)Expression.Constant(inner).ToRemoteLinqExpression(ExpressionTranslatorContext.NoMappingContext);
        expressionRemote.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        expressionRemote.Type.ToType().ShouldBeAssignableTypeTo<Expression<Func<int, int>>>();
        expressionRemote.Value.ShouldBeOfType<RemoteLinq.LambdaExpression>();

        var (expressionOriginal, expressionRoundTrip) = BackAndForth(Expression.Constant(inner), ExpressionTranslatorContext.NoMappingContext);
        expressionOriginal.Value.ShouldBe(inner);
        expressionRoundTrip.Value.ShouldBeAssignableTo<Expression<Func<int, int>>>();

        // Under the default context the inner expression references its parameter and cannot
        // be locally evaluated, so the constant still holds the recursively translated remote
        // expression and the roundtrip compiles to an equivalent lambda.
        var defaultInnerRemote = Expression.Constant(inner).ToRemoteLinqExpression();
        defaultInnerRemote.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        var defaultInnerConstant = defaultInnerRemote.ShouldBeOfType<RemoteLinq.ConstantExpression>();
        defaultInnerConstant.Type.ToType().ShouldBeAssignableTypeTo<Expression<Func<int, int>>>();
        defaultInnerConstant.Value.ShouldBeOfType<RemoteLinq.LambdaExpression>();
        var defaultInnerSystem = defaultInnerRemote.ToLinqExpression().ShouldBeOfType<ConstantExpression>();
        defaultInnerSystem.Value.ShouldBeAssignableTo<Expression<Func<int, int>>>()
            .With(defaultInnerFunc =>
            {
                defaultInnerFunc.ShouldNotBeNull().Compile().Invoke(9).ShouldBe(inner.Compile().Invoke(9));
            });

        // Expression collection constant: each expression in the collection is translated recursively.
        var p2 = Expression.Parameter(typeof(int), "p2");
        var collection = new Expression[]
        {
            Expression.Lambda<Func<int, int>>(Expression.Multiply(p2, Expression.Constant(3)), p2),
            Expression.Lambda<Func<int, int>>(Expression.Add(p2, Expression.Constant(4)), p2),
        };
        var collectionRemote = (RemoteLinq.ConstantExpression)Expression.Constant(collection).ToRemoteLinqExpression(ExpressionTranslatorContext.NoMappingContext);
        collectionRemote.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        collectionRemote.Type.ToType().ShouldBe(typeof(Expression[]));
        collectionRemote.Value.ShouldBeAssignableTo<RemoteLinq.Expression[]>()
            .With(collectionValues =>
            {
                collectionValues.ShouldNotBeNull();
                collectionValues.Length.ShouldBe(2);
                collectionValues[0].ShouldBeOfType<RemoteLinq.LambdaExpression>();
                collectionValues[1].ShouldBeOfType<RemoteLinq.LambdaExpression>();
            });

        var (collectionOriginal, collectionRoundTrip) = BackAndForth(Expression.Constant(collection), ExpressionTranslatorContext.NoMappingContext);
        collectionOriginal.Value.ShouldBe(collection);
        collectionRoundTrip.Value.ShouldBeAssignableTo<Expression[]>()
            .With(roundTrippedCollection =>
            {
                roundTrippedCollection.ShouldNotBeNull();
                roundTrippedCollection.Length.ShouldBe(2);
                roundTrippedCollection[0].ShouldBeAssignableTo<Expression<Func<int, int>>>();
                roundTrippedCollection[1].ShouldBeAssignableTo<Expression<Func<int, int>>>();
            });

        // Under the default context the self-contained collection constant still takes the
        // expression-collection branch, so each expression is translated recursively and the
        // roundtrip yields an equivalent collection.
        var defaultCollectionRemote = Expression.Constant(collection).ToRemoteLinqExpression();
        defaultCollectionRemote.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        var defaultCollectionConstant = defaultCollectionRemote.ShouldBeOfType<RemoteLinq.ConstantExpression>();
        defaultCollectionConstant.Type.ToType().ShouldBe(typeof(Expression[]));
        defaultCollectionConstant.Value.ShouldBeAssignableTo<RemoteLinq.Expression[]>()
            .With(defaultCollectionValues =>
            {
                defaultCollectionValues.ShouldNotBeNull();
                defaultCollectionValues.Length.ShouldBe(2);
                defaultCollectionValues[0].ShouldBeOfType<RemoteLinq.LambdaExpression>();
                defaultCollectionValues[1].ShouldBeOfType<RemoteLinq.LambdaExpression>();
            });
        var defaultCollectionSystem = defaultCollectionRemote.ToLinqExpression().ShouldBeOfType<ConstantExpression>();
        defaultCollectionSystem.Value.ShouldBeAssignableTo<Expression[]>()
            .With(defaultRoundTrippedCollection =>
            {
                defaultRoundTrippedCollection.ShouldNotBeNull();
                defaultRoundTrippedCollection.Length.ShouldBe(2);
                defaultRoundTrippedCollection[0].ShouldBeAssignableTo<Expression<Func<int, int>>>();
                defaultRoundTrippedCollection[1].ShouldBeAssignableTo<Expression<Func<int, int>>>();
            });
    }

    [Fact]
    public void Should_translate_dynamic_object_and_constant_query_argument_constants()
    {
        // Mapped constant (Default context): unknown POCO values are wrapped into a
        // ConstantQueryArgument holding a DynamicObject representation of the value.
        var p = new ProbeDto { Count = 5, Name = "probe" };

        // Under the default context the POCO constant is unknown to the type provider and is
        // therefore mapped: the remote constant holds a ConstantQueryArgument with the
        // DynamicObject representation of the POCO.
        var mappedRemote = Expression.Constant(p, typeof(ProbeDto)).ToRemoteLinqExpression();
        mappedRemote.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        var mappedConstant = mappedRemote.ShouldBeOfType<RemoteLinq.ConstantExpression>();
        mappedConstant.Type.ToType().ShouldBe(typeof(ProbeDto));
        mappedConstant.Value.ShouldBeOfType<ConstantQueryArgument>()
            .With(arg =>
            {
                arg.Value.ShouldBeAssignableTo<DynamicObject>()
                    .With(d =>
                    {
                        d.Type!.ToType().ShouldBe(typeof(ProbeDto));
                        d.Properties!.Count.ShouldBe(2);
                    });
            });

        // The wrapped value must roundtrip back to a POCO with equal property values.
        var (mappedOriginal, mappedRoundTrip) = BackAndForth((Expression)Expression.Constant(p, typeof(ProbeDto)));
        mappedOriginal.ShouldBeAssignableTo<ConstantExpression>().Value.ShouldBeSameAs(p);
        var mappedResult = Expression.Lambda<Func<ProbeDto>>(mappedRoundTrip).Compile().Invoke();
        mappedResult.Count.ShouldBe(5);
        mappedResult.Name.ShouldBe("probe");

        // Unmapped constant (NoMappingContext): a ConstantQueryArgument value passes through
        // unchanged because mapping is disabled for that context.
        var arg = new ConstantQueryArgument(new DynamicObject(new[]
        {
            ("Count", (object?)7),
            ("Name", "unmapped"),
        }));
        var unmappedRemote = (RemoteLinq.ConstantExpression)Expression.Constant(arg, typeof(ConstantQueryArgument)).ToRemoteLinqExpression(ExpressionTranslatorContext.NoMappingContext);
        unmappedRemote.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        unmappedRemote.Type.ToType().ShouldBe(typeof(ConstantQueryArgument));
        unmappedRemote.Value.ShouldBeSameAs(arg);

        // Without type information on the wrapped DynamicObject the value passes through
        // unchanged in both directions under the NoMappingContext.
        var (unmappedOriginal, unmappedRoundTrip) = BackAndForth(Expression.Constant(arg, typeof(ConstantQueryArgument)), ExpressionTranslatorContext.NoMappingContext);
        unmappedOriginal.Value.ShouldBeSameAs(arg);
        unmappedRoundTrip.Value.ShouldBeSameAs(arg);
    }

    [Fact]
    public void Should_translate_string_constants_with_explicit_type_information()
    {
        // String constant with string type stays a plain constant.
        var plainRemote = (RemoteLinq.ConstantExpression)Expression.Constant("hello", typeof(string)).ToRemoteLinqExpression(ExpressionTranslatorContext.NoMappingContext);
        plainRemote.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        plainRemote.Type.ToType().ShouldBe(typeof(string));
        plainRemote.Value.ShouldBe("hello");

        // Under the default context the plain string constant keeps its shape, so the
        // roundtrip returns the same value.
        var (stringOriginal, stringRoundTrip) = BackAndForth((Expression)Expression.Constant("hello", typeof(string)));
        stringOriginal.ShouldBeAssignableTo<ConstantExpression>().Value.ShouldBe("hello");
        Expression.Lambda<Func<string>>(stringRoundTrip).Compile().Invoke().ShouldBe("hello");

        // The system API forbids constants whose value is not assignable to the declared type,
        // so a string constant with a non-string type can only exist on the remote side. The
        // remote-to-system translator converts the value via the dynamic object mapper for
        // int, DateTime, and enum targets.
        var intStringRemote = new RemoteLinq.ConstantExpression("42", typeof(int));
        intStringRemote.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        intStringRemote.Value.ShouldBe("42");
        intStringRemote.Type.ToType().ShouldBe(typeof(int));

        var intSystem = intStringRemote.ToLinqExpression().ShouldBeOfType<ConstantExpression>();
        intSystem.Type.ShouldBe(typeof(int));
        intSystem.Value.ShouldBe(42);

        var date = new DateTime(2026, 9, 19, 12, 0, 0, DateTimeKind.Local);
        var dateStringRemote = new RemoteLinq.ConstantExpression(date.ToString("o"), typeof(DateTime));
        var dateSystem = dateStringRemote.ToLinqExpression().ShouldBeOfType<ConstantExpression>();
        dateSystem.Type.ShouldBe(typeof(DateTime));
        dateSystem.Value.ShouldBe(date);

        var enumStringRemote = new RemoteLinq.ConstantExpression("Friday", typeof(DayOfWeek));
        var enumSystem = enumStringRemote.ToLinqExpression().ShouldBeOfType<ConstantExpression>();
        enumSystem.Type.ShouldBe(typeof(DayOfWeek));
        enumSystem.Value.ShouldBe(DayOfWeek.Friday);
    }

    [Fact]
    public void Should_reuse_cached_constant_query_argument_for_same_reference()
    {
        // Two constants holding the same POCO reference (same type) within one translation
        // must reuse the cached ConstantQueryArgument instance. The QueryArgument attribute
        // keeps the constants from being locally evaluated, so both reach the mapping branch
        // of the translator and share the cached wrapper.
        var p = new QueryArgumentDto { Count = 5 };
        var c1 = Expression.Constant(p, typeof(QueryArgumentDto));
        var c2 = Expression.Constant(p, typeof(QueryArgumentDto));
        var count = typeof(QueryArgumentDto).GetProperty(nameof(QueryArgumentDto.Count))!;
        var parameter = Expression.Parameter(typeof(QueryArgumentDto), "x");
        var body = Expression.Add(Expression.Property(c1, count), Expression.Property(c2, count));
        var lambda = Expression.Lambda<Func<QueryArgumentDto, int>>(body, parameter);

        var remote = lambda.ToRemoteLinqExpression();
        var constantArguments = new List<ConstantQueryArgument>();
        CollectConstantQueryArguments(remote, constantArguments);
        constantArguments.Count.ShouldBe(2);
        constantArguments[0].ShouldBeSameAs(constantArguments[1]);

        // The roundtripped expression must still evaluate the sum of the shared value.
        var (original, roundTrip) = BackAndForth(lambda);
        original.Compile().Invoke(p).ShouldBe(10);
        roundTrip.Compile().Invoke(p).ShouldBe(10);
    }

    private static void CollectConstantQueryArguments(RemoteLinq.Expression expression, List<ConstantQueryArgument> constantArguments)
    {
        if (expression is RemoteLinq.ConstantExpression constant && constant.Value is ConstantQueryArgument argument)
        {
            constantArguments.Add(argument);
        }

        switch (expression)
        {
            case RemoteLinq.LambdaExpression lambdaExpression:
                CollectConstantQueryArguments(lambdaExpression.Expression, constantArguments);
                break;

            case RemoteLinq.MemberExpression member when member.Expression is not null:
                CollectConstantQueryArguments(member.Expression, constantArguments);
                break;

            case RemoteLinq.BinaryExpression binary:
                CollectConstantQueryArguments(binary.LeftOperand, constantArguments);
                CollectConstantQueryArguments(binary.RightOperand, constantArguments);
                break;
        }
    }
}
