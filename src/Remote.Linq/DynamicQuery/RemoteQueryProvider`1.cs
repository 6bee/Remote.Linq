// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using Aqua.TypeExtensions;
using Aqua.TypeSystem;
using System;
using System.Linq;
using Expression = System.Linq.Expressions.Expression;
using MethodInfo = System.Reflection.MethodInfo;

public sealed class RemoteQueryProvider<TSource> : IRemoteQueryProvider
{
    private static readonly MethodInfo _executeMethod = typeof(RemoteQueryProvider<TSource>)
        .GetMethodEx(nameof(Execute), [typeof(MethodInfos.TResult)], typeof(Expression));

    private readonly Func<Expressions.Expression, TSource?> _dataProvider;
    private readonly IQueryResultMapper<TSource> _resultMapper;
    private readonly IExpressionToRemoteLinqContext _context;

    public RemoteQueryProvider(
        Func<Expressions.Expression, TSource?> dataProvider,
        IQueryResultMapper<TSource> resultMapper,
        IExpressionToRemoteLinqContext? context)
    {
        _dataProvider = dataProvider.CheckNotNull();
        _resultMapper = resultMapper.CheckNotNull();
        _context = context ?? ExpressionTranslatorContext.Default;
    }

    /// <inheritdoc/>
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new RemoteQueryable<TElement>(this, expression);

    /// <inheritdoc/>
    public IQueryable CreateQuery(Expression expression)
    {
        var elementType = TypeHelper.GetElementType(expression.Type)
            ?? throw new RemoteLinqException($"Cannot get element type based on expression's type {expression.Type}");
        return new RemoteQueryable(elementType, this, expression);
    }

    /// <inheritdoc/>
    public TResult Execute<TResult>(Expression expression)
    {
        ExpressionHelper.CheckExpressionResultType<TResult>(expression);

        var rlinq = _context.ExpressionTranslator.TranslateExpression(expression);
        var dataRecords = _dataProvider(rlinq);
        return _resultMapper.MapResult<TResult>(dataRecords, expression)!;
    }

    /// <inheritdoc/>
    public object? Execute(Expression expression)
        => this.InvokeAndUnwrap<object?>(_executeMethod, expression);
}