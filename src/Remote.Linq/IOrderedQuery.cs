// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    public interface IOrderedQuery : IQuery
    {
        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <param name="sortExpression">A sort expression to extract a key from each element and define a sort direction.</param>
        /// <returns>A new query instance containing all specified query parameters.</returns>
        IOrderedQuery ThenBy(Expressions.SortExpression sortExpression);

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in descending order, according to a key.
        /// </summary>
        /// <param name="sortExpression">A sort expression to extract a key from each element and define a sort direction.</param>
        /// <returns>A new query instance containing all specified query parameters.</returns>
        IOrderedQuery ThenByDescending(Expressions.SortExpression sortExpression);
    }
}
