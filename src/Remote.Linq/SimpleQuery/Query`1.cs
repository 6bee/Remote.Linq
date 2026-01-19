// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.SimpleQuery;

using Aqua.EnumerableExtensions;
using Remote.Linq.ExpressionVisitors;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

[Serializable]
[DataContract]
public class Query<T> : IOrderedQuery<T>
{
    [NonSerialized]
    private readonly Func<Query<T>, IEnumerable<T>>? _dataProvider;

    [NonSerialized]
    private readonly Func<LambdaExpression, Expressions.LambdaExpression> _expressionTranslator;

    /// <summary>
    /// Initializes a new instance of the <see cref="Query{T}"/> class.
    /// </summary>
    private Query()
    {
        _expressionTranslator = exp => exp.ToRemoteLinqExpression().ReplaceGenericQueryArgumentsByNonGenericArguments();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Query{T}"/> class.
    /// </summary>
    [SuppressMessage("Blocker Code Smell", "S3427:Method overloads with default parameter values should not overlap ", Justification = "Default constructor needed for deserialization")]
    public Query(Func<Query<T>, IEnumerable<T>>? dataProvider = null, Func<LambdaExpression, Expressions.LambdaExpression>? expressionTranslator = null, IEnumerable<Expressions.LambdaExpression>? filterExpressions = null, IEnumerable<Expressions.SortExpression>? sortExpressions = null, int? skip = null, int? take = null)
    {
        _dataProvider = dataProvider;
        _expressionTranslator = expressionTranslator ?? (exp => exp.ToRemoteLinqExpression().ReplaceGenericQueryArgumentsByNonGenericArguments());
        FilterExpressions = filterExpressions.AsNullIfEmpty()?.ToList();
        SortExpressions = sortExpressions.AsNullIfEmpty()?.ToList();
        SkipValue = skip;
        TakeValue = take;
    }

    public bool HasFilters => FilterExpressions?.Count > 0;

    public bool HasSorting => SortExpressions?.Count > 0;

    public bool HasPaging => TakeValue.HasValue;

    [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
    public List<Expressions.LambdaExpression>? FilterExpressions { get; set; }

    [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
    public List<Expressions.SortExpression>? SortExpressions { get; set; }

    [DataMember(Name = "Skip", Order = 3, IsRequired = false, EmitDefaultValue = false)]
    public int? SkipValue { get; set; }

    [DataMember(Name = "Take", Order = 4, IsRequired = false, EmitDefaultValue = false)]
    public int? TakeValue { get; set; }

    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>A new query instance containing all specified query parameters.</returns>
    public IQuery<T> Where(Expression<Func<T, bool>> predicate)
    {
        var filter = _expressionTranslator(predicate);

        var filterExpressions = FilterExpressions.AsEmptyIfNull().ToList();
        filterExpressions.Add(filter);

        var query = new Query<T>(_dataProvider, _expressionTranslator, filterExpressions, SortExpressions, SkipValue, TakeValue);
        return query;
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to a key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>A new query instance containing all specified query parameters.</returns>
    public IOrderedQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        var expression = _expressionTranslator(keySelector);
        var sortExpression = new Expressions.SortExpression(expression, Expressions.SortDirection.Ascending);

        var query = new Query<T>(_dataProvider, _expressionTranslator, FilterExpressions, new[] { sortExpression }, SkipValue, TakeValue);
        return query;
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order according to a key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>A new query instance containing all specified query parameters.</returns>
    public IOrderedQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        var expression = _expressionTranslator(keySelector);
        var sortExpression = new Expressions.SortExpression(expression, Expressions.SortDirection.Descending);

        var query = new Query<T>(_dataProvider, _expressionTranslator, FilterExpressions, new[] { sortExpression }, SkipValue, TakeValue);
        return query;
    }

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key returned by the function represented by keySelector.</typeparam>
    /// <param name="keySelector">A function to extract a key from each element.</param>
    /// <returns>A new query instance containing all specified query parameters.</returns>
    IOrderedQuery<T> IOrderedQuery<T>.ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        var sortExpressions = SortExpressions?.ToList();
        if (sortExpressions?.Count is null or 0)
        {
            throw new InvalidOperationException($"No sorting defined yet, use {nameof(OrderBy)} or {nameof(OrderByDescending)} first.");
        }

        var expression = _expressionTranslator(keySelector);
        var sortExpression = new Expressions.SortExpression(expression, Expressions.SortDirection.Ascending);

        sortExpressions.Add(sortExpression);

        var query = new Query<T>(_dataProvider, _expressionTranslator, FilterExpressions, sortExpressions, SkipValue, TakeValue);
        return query;
    }

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence in descending order, according to a key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key returned by the function represented by keySelector.</typeparam>
    /// <param name="keySelector">A function to extract a key from each element.</param>
    /// <returns>A new query instance containing all specified query parameters.</returns>
    IOrderedQuery<T> IOrderedQuery<T>.ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        var sortExpressions = SortExpressions?.ToList();
        if (sortExpressions?.Count is null or 0)
        {
            throw new InvalidOperationException("No sorting defined yet, use OrderBy or OrderByDescending first.");
        }

        var expression = _expressionTranslator(keySelector);
        var sortExpression = new Expressions.SortExpression(expression, Expressions.SortDirection.Descending);

        sortExpressions.Add(sortExpression);

        var query = new Query<T>(_dataProvider, _expressionTranslator, FilterExpressions, sortExpressions, SkipValue, TakeValue);
        return query;
    }

    /// <summary>
    /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
    /// </summary>
    /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
    /// <returns>A new query instance containing all specified query parameters.</returns>
    public IQuery<T> Skip(int count)
    {
        var query = new Query<T>(_dataProvider, _expressionTranslator, FilterExpressions, SortExpressions, count, TakeValue);
        return query;
    }

    /// <summary>
    /// Returns a specified number of contiguous elements from the start of a sequence.
    /// </summary>
    /// <param name="count">The number of elements to return.</param>
    /// <returns>A new query instance containing all specified query parameters.</returns>
    public IQuery<T> Take(int count)
    {
        var query = new Query<T>(_dataProvider, _expressionTranslator, FilterExpressions, SortExpressions, SkipValue, count);
        return query;
    }

    /// <summary>
    /// Enumerating the query actually invokes the data provider to retrieve data.
    /// </summary>
    /// <returns>The data retrieved from the data provider.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        if (_dataProvider is null)
        {
            throw new RemoteLinqException("A query may only be enumerated if a data provider has been specified via constructor parameter.");
        }

        return _dataProvider(this).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        var queryParameters = QueryParametersToString();
        return string.Format(
            "Query<{0}>{1}{2}",
            typeof(T).FullName,
            string.IsNullOrEmpty(queryParameters) ? null : ": ",
            queryParameters);
    }

    protected string QueryParametersToString()
    {
        var sb = new StringBuilder();

        var filterExpressions = FilterExpressions;
        if (filterExpressions is not null)
        {
            foreach (var expression in filterExpressions)
            {
                sb.AppendLine();
                sb.AppendFormat("\tWhere {0}", expression);
            }
        }

        var sortExpressions = SortExpressions;
        if (sortExpressions is not null)
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