// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Denotes an asynchronous query result mapper.
    /// </summary>
    /// <typeparam name="TSource">The type of source data values.</typeparam>
    public interface IAsyncQueryResultMapper<in TSource>
    {
        /// <summary>
        /// Maps a source value to specified result type.
        /// </summary>
        /// <typeparam name="TResult">Target type.</typeparam>
        /// <param name="source">Source value to me mapped.</param>
        /// <param name="expression">The query expression for the source value.</param>
        /// <param name="cancellation">Cancellation token for the async operation.</param>
        /// <returns>A <see cref="ValueTask{TResult}"/> representing the mapped value.</returns>
        ValueTask<TResult> MapResultAsync<TResult>(TSource? source, Expression expression, CancellationToken cancellation = default);
    }
}