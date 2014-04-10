// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remote.Linq
{
    [Serializable]
    [DataContract]
    public class Query : IQuery, IOrderedQuery
    {
        #region Fields

        [DataMember(Name = "TypeName")]
#if SILVERLIGHT
        protected internal readonly string _typeName;
#else
        protected readonly string _typeName;
#endif

        [DataMember(Name = "FilterExpressions")]
#if SILVERLIGHT
        protected internal readonly List<Expressions.LambdaExpression> _filterExpressions;
#else
        protected readonly List<Expressions.LambdaExpression> _filterExpressions;
#endif

        [DataMember(Name = "SortExpressions")]
#if SILVERLIGHT
        protected internal readonly List<Expressions.SortExpression> _sortExpressions;
#else
        protected readonly List<Expressions.SortExpression> _sortExpressions;
#endif

        [DataMember(Name = "Skip")]
#if SILVERLIGHT
        protected internal int? _skip;
#else
        protected int? _skip;
#endif

        [DataMember(Name = "Take")]
#if SILVERLIGHT
        protected internal int? _take;
#else
        protected int? _take;
#endif

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Creates a new query instance.
        /// </summary>
        /// <param name="type">The type to be queried</param>
        public Query(Type type)
            : this(type, type.FullName)
        {
        }

        /// <summary>
        /// Creates a new query instance.
        /// </summary>
        /// <param name="typeFullName">The full name of the type to be queried</param>
        public Query(string typeFullName)
            : this(null, typeFullName)
        {
        }

        /// <summary>
        /// Creates a new query instance and copies query parameters from an existing one.
        /// </summary>
        /// <param name="parent">The existing query instance to copy the query parameters from.</param>
        protected Query(Query parent)
            : this(parent._type, parent._typeName)
        {
            _filterExpressions.AddRange(parent._filterExpressions);
            _sortExpressions.AddRange(parent._sortExpressions);
            _skip = parent._skip;
            _take = parent._take;
        }

        /// <summary>
        /// Creates a new query instance.
        /// </summary>
        /// <param name="typeFullName">The full name of the type to be queried</param>
        private Query(Type type, string typeFullName)
        {
            _type = type;
            _typeName = typeFullName;
            _filterExpressions = new List<Remote.Linq.Expressions.LambdaExpression>();
            _sortExpressions = new List<Remote.Linq.Expressions.SortExpression>();
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

        public Type Type
        {
            get
            {
                if (ReferenceEquals(null, _type))
                {
                    _type = TypeResolver.Instance.ResolveType(_typeName);
                }
                return _type;
            }
        }
        private Type _type;

        #endregion Properties

        #region Methods

        #region Linq Methods

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A lambda expression to test each element for a condition.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IQuery Where(Expressions.LambdaExpression predicate)
        {
            var query = new Query(this);
            query._filterExpressions.Add(predicate);
            return query;
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="lambdaExpression">A function to extract a key from an element.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IOrderedQuery OrderBy(Expressions.SortExpression sortExpression)
        {
            var query = new Query(this);
            query._sortExpressions.Clear();
            query._sortExpressions.Add(sortExpression);
            return query;
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IOrderedQuery OrderByDescending(Expressions.SortExpression sortExpression)
        {
            var query = new Query(this);
            query._sortExpressions.Clear();
            query._sortExpressions.Add(sortExpression);
            return query;
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <param name="sortExpression">A sort expression to extract a key from each element and define a sort direction.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        IOrderedQuery IOrderedQuery.ThenBy(Expressions.SortExpression sortExpression)
        {
            var query = new Query(this);
            query._sortExpressions.Add(sortExpression);
            return query;
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in descending order, according to a key.
        /// </summary>
        /// <param name="sortExpression">A sort expression to extract a key from each element and define a sort direction.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        IOrderedQuery IOrderedQuery.ThenByDescending(Expressions.SortExpression sortExpression)
        {
            var query = new Query(this);
            query._sortExpressions.Add(sortExpression);
            return query;
        }

        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IQuery Skip(int count)
        {
            var query = new Query(this);
            query._skip = count;
            return query;
        }

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IQuery Take(int count)
        {
            var query = new Query(this);
            query._take = count;
            return query;
        }

        #endregion Linq Methods

        #region Conversion methods

        /// <summary>
        /// Creates a non-generic version of the specified query instance. 
        /// </summary>
        /// <param name="query">The query instance to be converted into a non-generc query object.</param>
        /// <returns>A non-generic version of the specified query instance.</returns>
        public static Query CreateFromGeneric<T>(Query<T> query)
        {
            var instance = new Query(typeof(T));
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
            return string.Format("Query {0}{1}{2}", _typeName, string.IsNullOrEmpty(queryParameters) ? null : ": ", queryParameters);
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
