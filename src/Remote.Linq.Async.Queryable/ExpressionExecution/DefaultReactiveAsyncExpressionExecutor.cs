// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution;

using Aqua.Dynamic;
using Aqua.TypeExtensions;
using System;
using System.Linq;

public sealed class DefaultReactiveAsyncExpressionExecutor : InteractiveAsyncExpressionExecutor<DynamicObject?>
{
    private readonly IDynamicObjectMapper _mapper;
    private readonly Func<Type, bool> _setTypeInformation;

    public DefaultReactiveAsyncExpressionExecutor(Func<Type, IAsyncQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null, Func<Type, bool>? setTypeInformation = null)
        : base(queryableProvider, context)
    {
        _mapper = (context ?? new AsyncQueryExpressionTranslatorContext()).ValueMapper
            ?? throw new ArgumentException($"{nameof(IExpressionValueMapperProvider.ValueMapper)} property must not be null.", nameof(context));
        _setTypeInformation = setTypeInformation ?? (t => !t.IsAnonymousType());
    }

    /// <summary>
    /// Converts the query result into a collection of <see cref="DynamicObject"/>.
    /// </summary>
    /// <param name="queryResult">The reult of the query execution.</param>
    /// <returns>The mapped query result.</returns>
    protected override DynamicObject? ConvertResult(object? queryResult)
        => _mapper.MapObject(queryResult, _setTypeInformation);
}