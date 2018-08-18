// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;

    public interface IOrderedQuery<T> : IQuery<T>
    {
        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function represented by keySelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <returns>A new query instance containing all specified query parameters.</returns>
        IOrderedQuery<T> ThenBy<TKey>(System.Linq.Expressions.Expression<Func<T, TKey>> keySelector);

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in descending order, according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function represented by keySelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <returns>A new query instance containing all specified query parameters.</returns>
        IOrderedQuery<T> ThenByDescending<TKey>(System.Linq.Expressions.Expression<Func<T, TKey>> keySelector);
    }
}
