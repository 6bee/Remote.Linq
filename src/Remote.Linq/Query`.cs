// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
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

        [DataMember(Name = "FilterExpressions")]
        protected readonly List<Expressions.LambdaExpression> _filterExpressions;

        [DataMember(Name = "SortExpressions")]
        protected readonly List<Expressions.SortExpression> _sortExpressions;

        [DataMember(Name = "Skip")]
        protected int? _skip;

        [DataMember(Name = "Take")]
        protected int? _take;

        // no serializable function delegate for invocation of actual data provider (typically used on client side)
        [NonSerialized]
        private readonly Func<Query<T>, IEnumerable<T>> _dataProvider;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Creates a new query instance
        /// </summary>
        /// <param name="dataProvider">A delegate to be invoked to retrieve the actual data</param>
        public Query(Func<Query<T>, IEnumerable<T>> dataProvider = null)
        {
            _dataProvider = dataProvider;
            _filterExpressions = new List<Remote.Linq.Expressions.LambdaExpression>();
            _sortExpressions = new List<Remote.Linq.Expressions.SortExpression>();
        }

        protected Query(Query<T> parent)
            : this(parent._dataProvider)
        {
            _filterExpressions.AddRange(parent._filterExpressions);
            _sortExpressions.AddRange(parent._sortExpressions);
            _skip = parent._skip;
            _take = parent._take;
        }

        #endregion Constructor

        #region Properties

        public bool HasFilters { get { return _filterExpressions.Count > 0; } }
        public bool HasSorting { get { return _sortExpressions.Count > 0; } }
        public bool HasPaging { get { return _take.HasValue; } }

        public IEnumerable<Expressions.LambdaExpression> FilterExpressions { get { return _filterExpressions.AsReadOnly(); } }
        public IEnumerable<Expressions.SortExpression> SortExpressions { get { return _sortExpressions.AsReadOnly(); } }
        public int? SkipValue { get { return _skip; } }
        public int? TakeValue { get { return _take; } }

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
            var filter = predicate.ToQueryExpression();

            var query = new Query<T>(this);
            query._filterExpressions.Add(filter);
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
            var sortExpression = keySelector.ToQueryExpression();

            var query = new Query<T>(this);
            query._sortExpressions.Clear();
            query._sortExpressions.Add(Remote.Linq.Expressions.Expression.Sort(sortExpression, Remote.Linq.Expressions.SortDirection.Ascending));
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
            var sortExpression = keySelector.ToQueryExpression();

            var query = new Query<T>(this);
            query._sortExpressions.Clear();
            query._sortExpressions.Add(Remote.Linq.Expressions.Expression.Sort(sortExpression, Remote.Linq.Expressions.SortDirection.Descending));
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
            var sortExpression = keySelector.ToQueryExpression();

            var query = new Query<T>(this);
            query._sortExpressions.Add(Remote.Linq.Expressions.Expression.Sort(sortExpression, Remote.Linq.Expressions.SortDirection.Ascending));
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
            var sortExpression = keySelector.ToQueryExpression();

            var query = new Query<T>(this);
            query._sortExpressions.Add(Remote.Linq.Expressions.Expression.Sort(sortExpression, Remote.Linq.Expressions.SortDirection.Descending));
            return query;
        }

        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IQuery<T> Skip(int count)
        {
            var query = new Query<T>(this);
            query._skip = count;
            return query;
        }

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IQuery<T> Take(int count)
        {
            var query = new Query<T>(this);
            query._take = count;
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
            if (_dataProvider == null) throw new Exception("A query may only be enumerated if a data provider has been specified via constructor parameter.");
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
        /// <returns>A generic version of the specified query instance.</returns>
        /// <exception cref="Exception">If the query's type does not match the generic type.</exception>
        public static Query<T> CreateFromNonGeneric(Query query)
        {
            if (typeof(T) != query.Type) throw new Exception(string.Format("Generic type missmatch: {0} vs. {1}", typeof(T), query.Type));

            var type = typeof(Query<>).MakeGenericType(typeof(T));
            var instance = (Query<T>)Activator.CreateInstance(type, default(Func<Query<T>, IEnumerable<T>>));
            instance._filterExpressions.AddRange(query.FilterExpressions);
            instance._sortExpressions.AddRange(query.SortExpressions);
            instance._skip = query.SkipValue;
            instance._take = query.TakeValue;
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
            foreach (var expression in _filterExpressions)
            {
                sb.AppendLine();
                sb.AppendFormat("\tWhere {0}", expression);
            }
            foreach (var expression in _sortExpressions)
            {
                sb.AppendLine();
                sb.AppendFormat("\t{0}", expression);
            }
            if (_skip.HasValue)
            {
                sb.AppendLine();
                sb.AppendFormat("\tSkip {0}", _skip.Value);
            }
            if (_take.HasValue)
            {
                sb.AppendLine();
                sb.AppendFormat("\tTake {0}", _take.Value);
            }
            var queryParameters = sb.ToString();
            return queryParameters;
        }
        
        #endregion Methods
    }
}
