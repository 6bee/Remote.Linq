// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Expressions;
using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Remote.Linq
{
    [Serializable]
    [DataContract]
    public class Query : IQuery, IOrderedQuery
    {
        #region Constructor

        /// <summary>
        /// Creates a new query instance.
        /// </summary>
        /// <param name="type">The type to be queried</param>
        public Query(Type type, IEnumerable<LambdaExpression> filterExpressions = null, IEnumerable<SortExpression> sortExpressions = null, int? skip = null, int? take = null)
            : this(new TypeInfo(type), filterExpressions, sortExpressions, skip, take)
        {
        }

        /// <summary>
        /// Creates a new query instance.
        /// </summary>
        /// <param name="typeInfo">The type to be queried</param>
        public Query(TypeInfo typeInfo, IEnumerable<LambdaExpression> filterExpressions = null, IEnumerable<SortExpression> sortExpressions = null, int? skip = null, int? take = null)
        {
            Type = typeInfo;
            FilterExpressions = (filterExpressions ?? new LambdaExpression[0]).ToList().AsReadOnly();
            SortExpressions = (sortExpressions ?? new SortExpression[0]).ToList().AsReadOnly();
            SkipValue = skip;
            TakeValue = take;
        }

        #endregion Constructor

        #region Properties

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo Type { get; private set; }

        public bool HasFilters { get { return FilterExpressions.Count > 0; } }
        public bool HasSorting { get { return SortExpressions.Count > 0; } }
        public bool HasPaging { get { return TakeValue.HasValue; } }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ReadOnlyCollection<LambdaExpression> FilterExpressions { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ReadOnlyCollection<SortExpression> SortExpressions { get; private set; }

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
        /// <param name="predicate">A lambda expression to test each element for a condition.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IQuery Where(LambdaExpression predicate)
        {
            var filterExpressions = FilterExpressions.ToList();
            filterExpressions.Add(predicate);

            var query = new Query(Type, filterExpressions, SortExpressions, SkipValue, TakeValue);
            return query;
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="lambdaExpression">A function to extract a key from an element.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IOrderedQuery OrderBy(SortExpression sortExpression)
        {
            if (sortExpression.SortDirection != SortDirection.Ascending)
            {
                throw new ArgumentException("Expected sort expresson to be ascending.");
            }

            var query = new Query(Type, FilterExpressions, new[] { sortExpression }, SkipValue, TakeValue);
            return query;
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IOrderedQuery OrderByDescending(SortExpression sortExpression)
        {
            if (sortExpression.SortDirection != SortDirection.Descending)
            {
                throw new ArgumentException("Expected sort expresson to be descending.");
            }

            var query = new Query(Type, FilterExpressions, new[] { sortExpression }, SkipValue, TakeValue);
            return query;
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <param name="sortExpression">A sort expression to extract a key from each element and define a sort direction.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        IOrderedQuery IOrderedQuery.ThenBy(SortExpression sortExpression)
        {
            if (!SortExpressions.Any())
            {
                throw new InvalidOperationException("No sorting defined yet, use OrderBy or OrderByDescending first.");
            }
            if (sortExpression.SortDirection != SortDirection.Ascending)
            {
                throw new ArgumentException("Expected sort expresson to be ascending.");
            }

            var sortExpressions = SortExpressions.ToList();
            sortExpressions.Add(sortExpression);

            var query = new Query(Type, FilterExpressions, sortExpressions, SkipValue, TakeValue);
            return query;
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in descending order, according to a key.
        /// </summary>
        /// <param name="sortExpression">A sort expression to extract a key from each element and define a sort direction.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        IOrderedQuery IOrderedQuery.ThenByDescending(SortExpression sortExpression)
        {
            if (!SortExpressions.Any())
            {
                throw new InvalidOperationException("No sorting defined yet, use OrderBy or OrderByDescending first.");
            }
            if (sortExpression.SortDirection != SortDirection.Descending)
            {
                throw new ArgumentException("Expected sort expresson to be descending.");
            }

            var sortExpressions = SortExpressions.ToList();
            sortExpressions.Add(sortExpression);

            var query = new Query(Type, FilterExpressions, sortExpressions, SkipValue, TakeValue);
            return query;
        }

        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IQuery Skip(int count)
        {
            var query = new Query(Type, FilterExpressions, SortExpressions, count, TakeValue);
            return query;
        }

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>A new query instance containing all specified query parameters</returns>
        public IQuery Take(int count)
        {
            var query = new Query(Type, FilterExpressions, SortExpressions, SkipValue, count);
            return query;
        }

        #endregion Linq Methods

        #region Conversion methods

        /// <summary>
        /// Creates a non-generic version of the specified query instance. 
        /// </summary>
        /// <param name="query">The query instance to be converted into a non-generc query object.</param>
        /// <returns>A non-generic version of the specified query instance.</returns>
        public static Query CreateFromGeneric<T>(IQuery<T> query)
        {
            if (ReferenceEquals(null, query)) throw new ArgumentNullException("query");

            var instance = new Query(typeof(T), query.FilterExpressions, query.SortExpressions, query.SkipValue, query.TakeValue);
            return instance;
        }

        #endregion Conversion methods

        public override string ToString()
        {
            var queryParameters = QueryParametersToString();
            return string.Format("Query {0}{1}{2}", Type, string.IsNullOrEmpty(queryParameters) ? null : ": ", queryParameters);
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
