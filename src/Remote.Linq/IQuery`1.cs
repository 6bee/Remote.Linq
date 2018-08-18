// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
    using System.Collections.Generic;

    public interface IQuery<T> : IEnumerable<T>
    {
        bool HasFilters { get; }

        bool HasSorting { get; }

        bool HasPaging { get; }

        List<Expressions.LambdaExpression> FilterExpressions { get; }

        List<Expressions.SortExpression> SortExpressions { get; }

        int? SkipValue { get; }

        int? TakeValue { get; }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A new query instance containing all specified query parameters.</returns>
        IQuery<T> Where(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>A new query instance containing all specified query parameters.</returns>
        IOrderedQuery<T> OrderBy<TKey>(System.Linq.Expressions.Expression<Func<T, TKey>> keySelector);

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>A new query instance containing all specified query parameters.</returns>
        IOrderedQuery<T> OrderByDescending<TKey>(System.Linq.Expressions.Expression<Func<T, TKey>> keySelector);

        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        /// <returns>A new query instance containing all specified query parameters.</returns>
        IQuery<T> Skip(int count);

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>A new query instance containing all specified query parameters.</returns>
        IQuery<T> Take(int count);
    }
}
