// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.TypeSystem;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [Serializable]
    [DataContract]
    public class Query : IQuery, IOrderedQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        public Query()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        public Query(Type type, IEnumerable<LambdaExpression> filterExpressions = null, IEnumerable<SortExpression> sortExpressions = null, int? skip = null, int? take = null)
            : this(new TypeInfo(type, false, false), filterExpressions, sortExpressions, skip, take)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        public Query(TypeInfo typeInfo, IEnumerable<LambdaExpression> filterExpressions = null, IEnumerable<SortExpression> sortExpressions = null, int? skip = null, int? take = null)
        {
            Type = typeInfo;
            FilterExpressions = (filterExpressions ?? Enumerable.Empty<LambdaExpression>()).ToList();
            SortExpressions = (sortExpressions ?? Enumerable.Empty<SortExpression>()).ToList();
            SkipValue = skip;
            TakeValue = take;
        }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; }

        public bool HasFilters => FilterExpressions.Count > 0;

        public bool HasSorting => SortExpressions.Count > 0;

        public bool HasPaging => TakeValue.HasValue;

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public List<LambdaExpression> FilterExpressions { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public List<SortExpression> SortExpressions { get; set; }

        [DataMember(Name = "Skip", Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public int? SkipValue { get; set; }

        [DataMember(Name = "Take", Order = 5, IsRequired = false, EmitDefaultValue = false)]
        public int? TakeValue { get; set; }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A lambda expression to test each element for a condition.</param>
        /// <returns>A new query instance containing all specified query parameters.</returns>
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
        /// <returns>A new query instance containing all specified query parameters.</returns>
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
        /// <returns>A new query instance containing all specified query parameters.</returns>
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
        /// <returns>A new query instance containing all specified query parameters.</returns>
        public IQuery Skip(int count)
        {
            var query = new Query(Type, FilterExpressions, SortExpressions, count, TakeValue);
            return query;
        }

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>A new query instance containing all specified query parameters.</returns>
        public IQuery Take(int count)
        {
            var query = new Query(Type, FilterExpressions, SortExpressions, SkipValue, count);
            return query;
        }

        /// <summary>
        /// Creates a non-generic version of the specified query instance.
        /// </summary>
        /// <param name="query">The query instance to be converted into a non-generc query object.</param>
        /// <returns>A non-generic version of the specified query instance.</returns>
        public static Query CreateFromGeneric<T>(IQuery<T> query)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var instance = new Query(typeof(T), query.FilterExpressions, query.SortExpressions, query.SkipValue, query.TakeValue);
            return instance;
        }

        public override string ToString()
        {
            var queryParameters = QueryParametersToString();
            return string.Format("Query {0}{1}{2}", Type, string.IsNullOrEmpty(queryParameters) ? null : ": ", queryParameters);
        }

        protected string QueryParametersToString()
        {
            var sb = new StringBuilder();

            var filterExpressions = FilterExpressions;
            if (!(filterExpressions is null))
            {
                foreach (var expression in filterExpressions)
                {
                    sb.AppendLine();
                    sb.AppendFormat("\tWhere {0}", expression);
                }
            }

            var sortExpressions = SortExpressions;
            if (!(sortExpressions is null))
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
    }
}
