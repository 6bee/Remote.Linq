// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using System.Linq.Expressions;

/// <summary>
/// Denotes a query result mapper.
/// </summary>
/// <typeparam name="TSource">The type of source data values.</typeparam>
public interface IQueryResultMapper<in TSource>
{
    /// <summary>
    /// Maps a source value to specified result type.
    /// </summary>
    /// <typeparam name="TResult">Target type.</typeparam>
    /// <param name="source">Source value to be mapped.</param>
    /// <param name="expression">The query expression for the source value.</param>
    /// <returns>The mapped value.</returns>
    TResult? MapResult<TResult>(TSource? source, Expression expression);
}