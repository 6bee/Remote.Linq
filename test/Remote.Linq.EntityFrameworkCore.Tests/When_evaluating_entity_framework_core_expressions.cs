// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests;

using Aqua.Dynamic;
using Aqua.TypeSystem;
using Microsoft.EntityFrameworkCore;
using Remote.Linq.EntityFrameworkCore.Tests.Model;
using System.Linq.Expressions;

public sealed class When_evaluating_entity_framework_core_expressions : IDisposable
{
    private readonly TestDbContext _context = new();

    public When_evaluating_entity_framework_core_expressions()
    {
        _context.Lookup.AddRange(
            new LookupItem { Key = "1", Value = "One" },
            new LookupItem { Key = "2", Value = "Two" },
            new LookupItem { Key = "3", Value = "Three" });
        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public void Should_not_evaluate_ef_functions_and_should_evaluate_regular_expressions()
    {
        var efFunctions = Expression.Property(null, typeof(EF), nameof(EF.Functions));

        ExpressionEvaluator.CanBeEvaluated(efFunctions).ShouldBeFalse();
        ExpressionEvaluator.CanBeEvaluated(Expression.Constant(1)).ShouldBeTrue();
    }

    [Fact]
    public void Should_combine_local_evaluation_predicate_with_ef_evaluator()
    {
        var context = new EntityFrameworkCoreExpressionTranslatorContext((ITypeInfoProvider)null, _ => false);
        var efFunctions = Expression.Property(null, typeof(EF), nameof(EF.Functions));

        context.CanBeEvaluatedLocally(Expression.Constant(1)).ShouldBeFalse();
        context.CanBeEvaluatedLocally(efFunctions).ShouldBeFalse();
    }

    [Fact]
    public void Should_retain_supplied_type_info_provider_and_combine_evaluation_predicate()
    {
        var typeInfoProvider = new RecordingTypeInfoProvider();
        var context = new EntityFrameworkCoreExpressionTranslatorContext(typeInfoProvider, _ => true);
        var efFunctions = Expression.Property(null, typeof(EF), nameof(EF.Functions));

        context.TypeInfoProvider.ShouldBeSameAs(typeInfoProvider);

        var canBeEvaluated = context.CanBeEvaluatedLocally;
        canBeEvaluated.ShouldNotBeNull();
        canBeEvaluated(Expression.Constant(1)).ShouldBeTrue();
        canBeEvaluated(efFunctions).ShouldBeFalse();

        var rejectingContext = new EntityFrameworkCoreExpressionTranslatorContext(typeInfoProvider, _ => false);
        var rejecting = rejectingContext.CanBeEvaluatedLocally;
        rejecting.ShouldNotBeNull();
        rejecting(Expression.Constant(1)).ShouldBeFalse();
    }

    [Fact]
    public void Should_retain_supplied_type_resolver_and_combine_evaluation_predicate()
    {
        var typeResolver = new RecordingTypeResolver();
        var context = new EntityFrameworkCoreExpressionTranslatorContext(typeResolver, _ => true);
        var efFunctions = Expression.Property(null, typeof(EF), nameof(EF.Functions));

        context.TypeResolver.ShouldBeSameAs(typeResolver);

        var canBeEvaluated = context.CanBeEvaluatedLocally;
        canBeEvaluated.ShouldNotBeNull();
        canBeEvaluated(Expression.Constant(1)).ShouldBeTrue();
        canBeEvaluated(efFunctions).ShouldBeFalse();
    }

    [Fact]
    public void Should_expose_each_dependency_supplied_to_full_constructor()
    {
        var typeResolver = new RecordingTypeResolver();
        var typeInfoProvider = new RecordingTypeInfoProvider();
        var knownTypeProvider = new RecordingKnownTypeProvider(typeof(KnownType));
        var valueMapper = new RecordingValueMapper();

        var context = new EntityFrameworkCoreExpressionTranslatorContext(typeResolver, typeInfoProvider, knownTypeProvider, _ => true, valueMapper);

        context.TypeResolver.ShouldBeSameAs(typeResolver);
        context.TypeInfoProvider.ShouldBeSameAs(typeInfoProvider);
        context.ValueMapper.ShouldBeSameAs(valueMapper);

        context.NeedsMapping(new KnownType()).ShouldBeFalse();
        context.NeedsMapping(new UnknownType()).ShouldBeTrue();
    }

    [Fact]
    public void Should_create_and_execute_query_when_type_info_and_predicate_are_null()
    {
        // Exercises helper null-context forwarding: GetExpressionToRemoteLinqContext returns null when both are null.
        Func<Remote.Linq.Expressions.Expression, DynamicObject> provider = expression => expression.ExecuteWithEntityFrameworkCore(_context);
        var queryable = RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<LookupItem>(provider, (ITypeInfoProvider)null, (Func<Expression, bool>)null);

        queryable.Single(item => item.Key == "2").Value.ShouldBe("Two");
    }

    [Fact]
    public void Should_create_and_execute_query_when_type_info_or_predicate_is_supplied()
    {
        // Exercises helper EF-context creation: GetExpressionToRemoteLinqContext creates an EF translator context.
        Func<Remote.Linq.Expressions.Expression, DynamicObject> provider = expression => expression.ExecuteWithEntityFrameworkCore(_context);
        var queryable = RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<LookupItem>(provider, new RecordingTypeInfoProvider(), _ => true);

        queryable.Single(item => item.Key == "2").Value.ShouldBe("Two");
    }

    [Fact]
    public void Should_create_and_execute_query_when_direct_context_is_null()
    {
        // Exercises default EF-context creation: GetOrCreateContext creates a default EF translator context.
        Func<Remote.Linq.Expressions.Expression, DynamicObject> provider = expression => expression.ExecuteWithEntityFrameworkCore(_context);
        var queryable = RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<LookupItem>(provider, (IExpressionToRemoteLinqContext)null);

        queryable.Single(item => item.Key == "2").Value.ShouldBe("Two");
    }

    private sealed class KnownType;

    private sealed class UnknownType;

    private sealed class RecordingTypeInfoProvider : ITypeInfoProvider
    {
        public TypeInfo GetTypeInfo(Type type, bool? includePropertyInfos, bool? setMemberDeclaringTypes)
            => type?.AsTypeInfo();
    }

    private sealed class RecordingTypeResolver : ITypeResolver
    {
        public Type ResolveType(TypeInfo typeInfo) => typeInfo.ToType();
    }

    private sealed class RecordingKnownTypeProvider : IIsKnownTypeProvider
    {
        private readonly Type _knownType;

        public RecordingKnownTypeProvider(Type knownType)
        {
            _knownType = knownType;
        }

        public bool IsKnownType(Type type) => type == _knownType;
    }

    private sealed class RecordingValueMapper : IDynamicObjectMapper
    {
        public object Map(DynamicObject obj, Type targetType) => obj;

        public DynamicObject MapObject(object obj, Func<Type, bool> setTypeInformation) => null;
    }
}
