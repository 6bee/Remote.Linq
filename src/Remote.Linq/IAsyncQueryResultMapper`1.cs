// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAsyncQueryResultMapper<in TSource>
    {
        /// <summary>
        /// Maps a source value to specified result type.
        /// </summary>
        /// <typeparam name="TResult">The of the resulting value.</typeparam>
        /// <param name="source">Source value to me mapped.</param>
        /// <param name="expression">The query expression for the source value.</param>
        /// <param name="cancellation">Cancellation token for the async operation.</param>
        /// <returns>A <see cref="ValueTask{TResult}"/> representing the mapped source value.</returns>
        ValueTask<TResult> MapResultAsync<TResult>(TSource? source, Expression expression, CancellationToken cancellation = default);
    }
}
