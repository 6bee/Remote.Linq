// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#nullable enable
namespace Remote.Linq.Tests.RemoteQueryableFactory;

using Aqua.Dynamic;
using Aqua.TypeSystem;
using Remote.Linq;
using Remote.Linq.SimpleQuery;
using Remote.Linq.Tests.TestSupport;
using ExpressionTranslator = Remote.Linq.DynamicQuery.ExpressionTranslator;
using IExpressionTranslator = Remote.Linq.DynamicQuery.IExpressionTranslator;
using QueryableResourceDescriptor = Remote.Linq.DynamicQuery.QueryableResourceDescriptor;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

public class When_using_factory_overloads
{
    private class ItemDto
    {
        public int Count { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    private class SourceItem
    {
        public int Count { get; set; }
    }

    private class RecordingTypeInfoProvider : ITypeInfoProvider
    {
        private int _invocationCount;

        public int InvocationCount => _invocationCount;

        public TypeInfo? GetTypeInfo(Type? type, bool? includePropertyInfos = null, bool? setMemberDeclaringTypes = null)
        {
            if (type?.AsTypeInfo() is { } info)
            {
                Interlocked.Increment(ref _invocationCount);
                return info;
            }

            return null;
        }
    }

    private class RecordingPredicate
    {
        private int _invocationCount;

        public RecordingPredicate()
        {
            Predicate = expression =>
            {
                Interlocked.Increment(ref _invocationCount);
                return false;
            };
        }

        public int InvocationCount => _invocationCount;

        public Func<SystemLinq.Expression, bool> Predicate { get; }
    }

    private class TestContext : IExpressionToRemoteLinqContext
    {
        public IDynamicObjectMapper ValueMapper { get; }

        public ITypeInfoProvider TypeInfoProvider { get; } = new TypeInfoProvider();

        public Func<object, bool> NeedsMapping { get; } = _ => false;

        public IExpressionTranslator ExpressionTranslator { get; }

        public Func<SystemLinq.Expression, bool>? CanBeEvaluatedLocally { get; } = null;

        public TestContext()
        {
            ValueMapper = new ExpressionTranslatorContext(null, null, null, null, null).ValueMapper;
            ExpressionTranslator = new ExpressionTranslator(this);
        }
    }

    /// <summary>
    /// Group 1: untyped <see cref="IRemoteQueryable"/> overloads serving <see cref="DynamicObject"/> (overloads 1-4).
    /// </summary>
    [Fact]
    public void Should_execute_untyped_dynamic_object_overloads()
    {
        var factory = RemoteQueryable.Factory;
        var dataProvider = new RecordingSyncProvider<DynamicObject>(new DynamicObject(new[] { new Property("Count", 42) }));
        var descriptorType = typeof(ItemDto);

        // overload 1: untyped DynamicObject with optional context (default mapper)
        var queryable1 = factory.CreateQueryable(descriptorType, dataProvider.DataProvider);
        var result1 = Execute<DynamicObject>(queryable1);
        result1["Count"].ShouldBe(42);
        queryable1.ResourceType.ShouldBe(descriptorType);
        dataProvider.InvocationCount.ShouldBe(1);
        AssertContainsResourceDescriptor(dataProvider.LastExpression, descriptorType);

        // overload 2: untyped DynamicObject with type info provider and local-evaluation predicate
        var typeInfoProvider = new RecordingTypeInfoProvider();
        var predicate = new RecordingPredicate();
        var queryable2 = factory.CreateQueryable(descriptorType, dataProvider.DataProvider, typeInfoProvider, predicate.Predicate);
        var result2 = Execute<DynamicObject>(queryable2);
        result2["Count"].ShouldBe(42);
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        AssertContainsResourceDescriptor(dataProvider.LastExpression, descriptorType);

        // overload 3: untyped DynamicObject with explicit context
        var context = new TestContext();
        var queryable3 = factory.CreateQueryable(descriptorType, dataProvider.DataProvider, context);
        var result3 = Execute<DynamicObject>(queryable3);
        result3["Count"].ShouldBe(42);
        context.NeedsMapping(new object()).ShouldBeFalse();
        dataProvider.InvocationCount.ShouldBe(3);

        // overload 4: untyped DynamicObject with type info provider and explicit mapper
        var mapper = new PassthroughQueryResultMapper<DynamicObject>();
        var queryable4 = factory.CreateQueryable(descriptorType, dataProvider.DataProvider, typeInfoProvider, predicate.Predicate, mapper);
        var result4 = Execute<DynamicObject>(queryable4);
        result4["Count"].ShouldBe(42);
        mapper.InvocationCount.ShouldBe(1);
        mapper.LastExpression.ShouldBeOfType<SystemLinq.ConstantExpression>();
    }

    /// <summary>
    /// Group 2: untyped <see cref="IRemoteQueryable"/> overloads serving <see cref="object"/> (overloads 5-6).
    /// </summary>
    [Fact]
    public void Should_execute_untyped_object_overloads()
    {
        var factory = RemoteQueryable.Factory;
        var item = new SourceItem { Count = 3 };
        var objectProvider = new RecordingSyncProvider<object?>(item);
        var descriptorType = typeof(ItemDto);

        // overload 5: untyped object with optional context (default mapper)
        var queryable5 = factory.CreateQueryable(descriptorType, (Func<RemoteLinq.Expression, object?>)objectProvider.DataProvider);
        var result5 = Execute<SourceItem>(queryable5);
        result5.ShouldBeSameAs(item);
        queryable5.ResourceType.ShouldBe(descriptorType);
        objectProvider.InvocationCount.ShouldBe(1);
        AssertContainsResourceDescriptor(objectProvider.LastExpression, descriptorType);

        // overload 6: untyped object with type info provider and local-evaluation predicate
        var typeInfoProvider = new RecordingTypeInfoProvider();
        var predicate = new RecordingPredicate();
        var queryable6 = factory.CreateQueryable(descriptorType, (Func<RemoteLinq.Expression, object?>)objectProvider.DataProvider, typeInfoProvider, predicate.Predicate);
        var result6 = Execute<SourceItem>(queryable6);
        result6.ShouldBeSameAs(item);
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        AssertContainsResourceDescriptor(objectProvider.LastExpression, descriptorType);
    }

    /// <summary>
    /// Group 3: typed <see cref="IRemoteQueryable{T}"/> <see cref="DynamicObject"/> overloads (7-8) and untyped custom-source overloads (9-12).
    /// </summary>
    [Fact]
    public void Should_execute_custom_source_overloads()
    {
        var factory = RemoteQueryable.Factory;
        var dynamicProvider = new RecordingSyncProvider<DynamicObject>(new DynamicObject(new[] { new Property("Count", 7), new Property("Name", "typed") }));
        var item = new SourceItem { Count = 5 };
        var sourceProvider = new RecordingSyncProvider<SourceItem>(item);
        var descriptorType = typeof(ItemDto);

        // overload 7: typed DynamicObject with optional context (default mapper)
        var queryable7 = factory.CreateQueryable<ItemDto>(dynamicProvider.DataProvider);
        var result7 = Execute<SourceItem>(queryable7);
        result7.Count.ShouldBe(7);
        dynamicProvider.InvocationCount.ShouldBe(1);
        AssertContainsResourceDescriptor(dynamicProvider.LastExpression, descriptorType);

        // overload 8: typed DynamicObject with type info provider and explicit mapper
        var dynamicMapper = new Remote.Linq.DynamicQuery.DynamicResultMapper();
        var queryable8 = factory.CreateQueryable<ItemDto>(dynamicProvider.DataProvider, new RecordingTypeInfoProvider(), new RecordingPredicate().Predicate, dynamicMapper);
        var result8 = Execute<SourceItem>(queryable8);
        result8.Count.ShouldBe(7);
        dynamicProvider.InvocationCount.ShouldBe(2);

        // overload 9: untyped custom source with default context (mapper required)
        var sourceMapper9 = new PassthroughQueryResultMapper<SourceItem>();
        var queryable9 = factory.CreateQueryable<SourceItem>(typeof(ItemDto), (Func<RemoteLinq.Expression, SourceItem?>)sourceProvider.DataProvider, sourceMapper9);
        var result9 = Execute<SourceItem>(queryable9);
        result9.ShouldBeSameAs(item);
        sourceMapper9.InvocationCount.ShouldBe(1);
        sourceMapper9.LastExpression.ShouldBeOfType<SystemLinq.ConstantExpression>();

        // overload 10: untyped custom source with explicit context
        var context = new TestContext();
        var sourceMapper10 = new PassthroughQueryResultMapper<SourceItem>();
        var queryable10 = factory.CreateQueryable<SourceItem>(typeof(ItemDto), (Func<RemoteLinq.Expression, SourceItem?>)sourceProvider.DataProvider, context, sourceMapper10);
        var result10 = Execute<SourceItem>(queryable10);
        result10.ShouldBeSameAs(item);
        sourceMapper10.InvocationCount.ShouldBe(1);
        context.NeedsMapping(new object()).ShouldBeFalse();

        // overload 11: untyped custom source with type info provider
        var typeInfoProvider = new RecordingTypeInfoProvider();
        var sourceMapper11 = new PassthroughQueryResultMapper<SourceItem>();
        var queryable11 = factory.CreateQueryable<SourceItem>(typeof(ItemDto), (Func<RemoteLinq.Expression, SourceItem?>)sourceProvider.DataProvider, typeInfoProvider, sourceMapper11);
        var result11 = Execute<SourceItem>(queryable11);
        result11.ShouldBeSameAs(item);
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);

        // overload 12: untyped custom source with type info provider and local-evaluation predicate
        var predicate = new RecordingPredicate();
        var sourceMapper12 = new PassthroughQueryResultMapper<SourceItem>();
        var queryable12 = factory.CreateQueryable<SourceItem>(typeof(ItemDto), (Func<RemoteLinq.Expression, SourceItem?>)sourceProvider.DataProvider, typeInfoProvider, predicate.Predicate, sourceMapper12);
        var result12 = Execute<SourceItem>(queryable12);
        result12.ShouldBeSameAs(item);
        predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
    }

    /// <summary>
    /// Group 4: typed <see cref="IRemoteQueryable{T}"/> custom-source overloads <c>CreateQueryable&lt;T,TSource&gt;</c> (overloads 13-16).
    /// </summary>
    [Fact]
    public void Should_execute_typed_custom_source_overloads()
    {
        var factory = RemoteQueryable.Factory;
        var sourceItem = new SourceItem { Count = 5 };
        var sourceProvider = new RecordingSyncProvider<SourceItem>(sourceItem);

        // overload 13: typed custom source with default (null) context
        var mapper13 = new PassthroughQueryResultMapper<SourceItem>();
        var queryable13 = factory.CreateQueryable<ItemDto, SourceItem>((Func<RemoteLinq.Expression, SourceItem?>)sourceProvider.DataProvider, mapper13);
        var result13 = Execute<SourceItem>(queryable13);
        result13.ShouldBeSameAs(sourceItem);
        queryable13.ResourceType.ShouldBe(typeof(ItemDto));
        mapper13.InvocationCount.ShouldBe(1);

        // overload 14: typed custom source with explicit context
        var context = new TestContext();
        var mapper14 = new PassthroughQueryResultMapper<SourceItem>();
        var queryable14 = factory.CreateQueryable<ItemDto, SourceItem>((Func<RemoteLinq.Expression, SourceItem?>)sourceProvider.DataProvider, context, mapper14);
        var result14 = Execute<SourceItem>(queryable14);
        result14.ShouldBeSameAs(sourceItem);
        mapper14.InvocationCount.ShouldBe(1);
        context.ExpressionTranslator.ShouldNotBeNull();

        // overload 15: typed custom source with type info provider
        var typeInfoProvider = new RecordingTypeInfoProvider();
        var mapper15 = new PassthroughQueryResultMapper<SourceItem>();
        var queryable15 = factory.CreateQueryable<ItemDto, SourceItem>((Func<RemoteLinq.Expression, SourceItem?>)sourceProvider.DataProvider, typeInfoProvider, mapper15);
        var result15 = Execute<SourceItem>(queryable15);
        result15.ShouldBeSameAs(sourceItem);
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        mapper15.InvocationCount.ShouldBe(1);

        // overload 16: typed custom source with type info provider, local-evaluation predicate, and explicit mapper
        var predicate = new RecordingPredicate();
        var mapper16 = new PassthroughQueryResultMapper<SourceItem>();
        var queryable16 = factory.CreateQueryable<ItemDto, SourceItem>((Func<RemoteLinq.Expression, SourceItem?>)sourceProvider.DataProvider, typeInfoProvider, predicate.Predicate, mapper16);
        var result16 = Execute<SourceItem>(queryable16);
        result16.ShouldBeSameAs(sourceItem);
        predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        mapper16.InvocationCount.ShouldBe(1);
    }

    private static void AssertContainsResourceDescriptor(RemoteLinq.Expression? remoteExpression, Type elementType)
    {
        var constant = remoteExpression.ShouldNotBeNull().ShouldBeOfType<RemoteLinq.ConstantExpression>();
        constant.Value
            .ShouldBeOfType<QueryableResourceDescriptor>()
            .Type
            .ToType()
            .ShouldBe(elementType);
    }

    private static T Execute<T>(IRemoteQueryable queryable)
        => queryable.Execute<T>();
}
