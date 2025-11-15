// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution;

using Aqua.Dynamic;
using Aqua.TypeExtensions;
using Microsoft.EntityFrameworkCore;
using System.Security;

public sealed class DefaultEntityFrameworkCoreAsyncStreamExpressionExecutor : EntityFrameworkCoreAsyncStreamExpressionExecutor<DynamicObject>
{
    private readonly IDynamicObjectMapper _mapper;
    private readonly Func<Type, bool> _setTypeInformation;

    [SecuritySafeCritical]
    public DefaultEntityFrameworkCoreAsyncStreamExpressionExecutor(DbContext dbContext, IExpressionFromRemoteLinqContext? context = null, Func<Type, bool>? setTypeInformation = null)
        : this(dbContext.GetQueryableSetProvider(), context, setTypeInformation)
    {
    }

    public DefaultEntityFrameworkCoreAsyncStreamExpressionExecutor(Func<Type, IQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null, Func<Type, bool>? setTypeInformation = null)
        : base(queryableProvider, context ??= new EntityFrameworkCoreExpressionTranslatorContext())
    {
        _mapper = context.ValueMapper;
        _setTypeInformation = setTypeInformation ?? (t => !t.IsAnonymousType());
    }

    protected override async IAsyncEnumerable<DynamicObject> ConvertResult(IAsyncEnumerable<object?> queryResult)
    {
        await foreach (var item in queryResult.CheckNotNull())
        {
            yield return _mapper.MapObject(item, _setTypeInformation)
                ?? DynamicObject.CreateDefault(Context.SystemExpression?.Type);
        }
    }
}