// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name

#nullable enable

namespace Remote.Linq.Tests.RemoteQueryableFactory;

using Aqua.Dynamic;
using Aqua.TypeSystem;
using Remote.Linq;
using ExpressionTranslator = Remote.Linq.DynamicQuery.ExpressionTranslator;
using IExpressionTranslator = Remote.Linq.DynamicQuery.IExpressionTranslator;
using SystemLinq = System.Linq.Expressions;

/// <summary>
/// Records invocations of a type info provider.
/// </summary>
internal sealed class RecordingTypeInfoProvider : ITypeInfoProvider
{
    private int _invocationCount;

    /// <summary>
    /// Gets the number of invocations of the type info provider.
    /// </summary>
    public int InvocationCount => _invocationCount;

    /// <inheritdoc/>
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

/// <summary>
/// Records invocations of a local-evaluation predicate.
/// </summary>
internal sealed class RecordingPredicate
{
    private int _invocationCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordingPredicate"/> class.
    /// </summary>
    public RecordingPredicate()
    {
        Predicate = expression =>
        {
            Interlocked.Increment(ref _invocationCount);
            return false;
        };
    }

    /// <summary>
    /// Gets the number of invocations of the predicate.
    /// </summary>
    public int InvocationCount => _invocationCount;

    /// <summary>
    /// Gets the local-evaluation predicate.
    /// </summary>
    public Func<SystemLinq.Expression, bool> Predicate { get; }
}

/// <summary>
/// A deterministic <see cref="IExpressionToRemoteLinqContext"/> that never maps values.
/// </summary>
internal sealed class RecordingTranslationContext : IExpressionToRemoteLinqContext
{
    /// <inheritdoc/>
    public IDynamicObjectMapper ValueMapper { get; }

    /// <inheritdoc/>
    public ITypeInfoProvider TypeInfoProvider { get; } = new TypeInfoProvider();

    /// <inheritdoc/>
    public Func<object, bool> NeedsMapping { get; } = _ => false;

    /// <inheritdoc/>
    public IExpressionTranslator ExpressionTranslator { get; }

    /// <inheritdoc/>
    public Func<SystemLinq.Expression, bool>? CanBeEvaluatedLocally { get; } = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordingTranslationContext"/> class.
    /// </summary>
    public RecordingTranslationContext()
    {
        ValueMapper = new ExpressionTranslatorContext(null, null, null, null, null).ValueMapper;
        ExpressionTranslator = new ExpressionTranslator(this);
    }
}
