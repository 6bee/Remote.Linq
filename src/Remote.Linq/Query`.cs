// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

namespace Remote.Linq
{
    [Serializable]
    [DataContract]
    public class Query<T> : IQuery<T>, IOrderedQuery<T>
    {
        #region Fields

        [NonSerialized]
        private readonly Func<Query<T>, IEnumerable<T>> _dataProvider;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Creates a new query instance
        /// </summary>
        /// <param name="dataProvider">A delegate to be invoked to retrieve the actual data</param>
        public Query(Func<Query<T>, IEnumerable<T>> dataProvider = null, IEnumerable<Remote.Linq.Expressions.LambdaExpression> filterExpressions = null, IEnumerable<Remote.Linq.Expressions.SortExpression> sortExpressions = null, int? skip = null, int? take = null)
        {
            _dataProvider = dataProvider;
            FilterExpressions = (filterExpressions ?? new Remote.Linq.Expressions.LambdaExpression[0]).ToList().AsReadOnly();
            SortExpressions = (sortExpressions ?? new Remote.Linq.Expressions.SortExpression[0]).ToList().AsReadOnly();
            SkipValue = skip;
            TakeValue = take;
        }

        #endregion Constructor

        #region Properties

        public bool HasFilters { get { return FilterExpressions.Count > 0; } }
        public bool HasSorting { get { return SortExpressions.Count > 0; } }
        public bool HasPaging { get { return TakeValue.HasValue; } }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ReadOnlyCollection<Remote.Linq.Expressions.LambdaExpression> FilterExpressions { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ReadOnlyCollection<Remote.Linq.Expressions.SortExpression> SortExpressions { get; private set; }

        [DataMember(Name = "Skip", IsRequired = false, EmitDefaultValue = false)]
        public int? SkipValue { get; private set; }

        [DataMember(Name = "Take", IsRequired = false, EmitDefaultValue = false)]
        public int? TakeValue { get; private set; }

        #endregion Properties

        #region Methods

        #region Linq Methods

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            var filter = predicate.ToRemoteLinqExpression();

            var filterExpressions = FilterExpressions.ToList();
            filterExpressions.Add(filter);

            var query = new Query<T>(_dataProvider, filterExpressions, SortExpressions, SkipValue, TakeValue);
            return query;
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="lambdaExpression">A function to extract a key from an element.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IOrderedQuery<T> OrderBy<TKey>(System.Linq.Expressions.Expression<Func<T, TKey>> keySelector)
        {
            var expression = keySelector.ToRemoteLinqExpression();
            var sortExpression = Remote.Linq.Expressions.Expression.Sort(expression, Remote.Linq.Expressions.SortDirection.Ascending);

            var query = new Query<T>(_dataProvider, FilterExpressions, new[] { sortExpression }, SkipValue, TakeValue);
            return query;
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IOrderedQuery<T> OrderByDescending<TKey>(System.Linq.Expressions.Expression<Func<T, TKey>> keySelector)
        {
            var expression = keySelector.ToRemoteLinqExpression();
            var sortExpression = Remote.Linq.Expressions.Expression.Sort(expression, Remote.Linq.Expressions.SortDirection.Descending);

            var query = new Query<T>(_dataProvider, FilterExpressions, new[] { sortExpression }, SkipValue, TakeValue);
            return query;
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function represented by keySelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        IOrderedQuery<T> IOrderedQuery<T>.ThenBy<TKey>(System.Linq.Expressions.Expression<Func<T, TKey>> keySelector)
        {
            if (!SortExpressions.Any())
            {
                throw new InvalidOperationException("No sorting defined yet, use OrderBy or OrderByDescending first.");
            }

            var expression = keySelector.ToRemoteLinqExpression();
            var sortExpression = Remote.Linq.Expressions.Expression.Sort(expression, Remote.Linq.Expressions.SortDirection.Ascending);

            var sortExpressions = SortExpressions.ToList();
            sortExpressions.Add(sortExpression);

            var query = new Query<T>(_dataProvider, FilterExpressions, sortExpressions, SkipValue, TakeValue);
            return query;
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in descending order, according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function represented by keySelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        IOrderedQuery<T> IOrderedQuery<T>.ThenByDescending<TKey>(System.Linq.Expressions.Expression<Func<T, TKey>> keySelector)
        {
            if (!SortExpressions.Any())
            {
                throw new InvalidOperationException("No sorting defined yet, use OrderBy or OrderByDescending first.");
            }

            var expression = keySelector.ToRemoteLinqExpression();
            var sortExpression = Remote.Linq.Expressions.Expression.Sort(expression, Remote.Linq.Expressions.SortDirection.Descending);

            var sortExpressions = SortExpressions.ToList();
            sortExpressions.Add(sortExpression);

            var query = new Query<T>(_dataProvider, FilterExpressions, sortExpressions, SkipValue, TakeValue);
            return query;
        }

        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IQuery<T> Skip(int count)
        {
            var query = new Query<T>(_dataProvider, FilterExpressions, SortExpressions, count, TakeValue);
            return query;
        }

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IQuery<T> Take(int count)
        {
            var query = new Query<T>(_dataProvider, FilterExpressions, SortExpressions, SkipValue, count);
            return query;
        }

        #endregion Linq Methods

        #region IEnumerable members

        /// <summary>
        /// Enumerating the query actually invokes the data provider to retrieve data
        /// </summary>
        /// <returns>The data retrieved from the data provider.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (ReferenceEquals(null, _dataProvider)) throw new Exception("A query may only be enumerated if a data provider has been specified via constructor parameter.");
            return _dataProvider(this).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable members

        #region Conversion methods

        /// <summary>
        /// Creates a generic version of the specified query instance. 
        /// </summary>
        /// <param name="query">The query instance to be converted into a generc query object.</param>
        /// <param name="dataProvider">A delegate to be invoked to retrieve the actual data</param>
        /// <returns>A generic version of the specified query instance.</returns>
        /// <exception cref="Exception">If the query's type does not match the generic type.</exception>
        public static Query<T> CreateFromNonGeneric(IQuery query, Func<Query<T>, IEnumerable<T>> dataProvider = null, ITypeResolver typeResolver = null)
        {
            if (ReferenceEquals(null, query))
            {
                throw new ArgumentNullException("query");
            }

            var type = (typeResolver ?? TypeResolver.Instance).ResolveType(query.Type);
            if (typeof(T) != type)
            {
                throw new Exception(string.Format("Generic type mismatch: {0} vs. {1}", typeof(T), query.Type));
            }

            var instance = new Query<T>(dataProvider, query.FilterExpressions, query.SortExpressions, query.SkipValue, query.TakeValue);
            return instance;
        }

        #endregion Conversion methods

        public override string ToString()
        {
            var queryParameters = QueryParametersToString();
            return string.Format("Query<{0}>{1}{2}", typeof(T).FullName, string.IsNullOrEmpty(queryParameters) ? null : ": ", queryParameters);
        }

        protected string QueryParametersToString()
        {
            var sb = new StringBuilder();

            var filterExpressions = FilterExpressions;
            if (!ReferenceEquals(null, filterExpressions))
            {
                foreach (var expression in filterExpressions)
                {
                    sb.AppendLine();
                    sb.AppendFormat("\tWhere {0}", expression);
                }
            }

            var sortExpressions = SortExpressions;
            if (!ReferenceEquals(null, sortExpressions))
            {
                foreach (var expression in sortExpressions)
                {
                    sb.AppendLine();
                    sb.AppendFormat("\t{0}", expression);
                }
            }

            var skipValue = SkipValue;
            if (skipValue.HasValue)
            {
                sb.AppendLine();
                sb.AppendFormat("\tSkip {0}", skipValue.Value);
            }

            var takeValue = TakeValue;
            if (takeValue.HasValue)
            {
                sb.AppendLine();
                sb.AppendFormat("\tTake {0}", takeValue.Value);
            }

            var queryParameters = sb.ToString();
            return queryParameters;
        }

        #endregion Methods
    }
}
