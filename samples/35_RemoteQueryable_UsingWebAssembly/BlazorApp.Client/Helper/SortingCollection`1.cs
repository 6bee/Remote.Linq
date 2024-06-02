// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace BlazorApp.Client.Helper;

using System.Linq.Expressions;

public sealed class SortingCollection<TSource>
{
    private readonly List<ISortExpression> _list;

    public SortingCollection(int maxCount)
    {
        if (maxCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxCount), "Max count must not be smaller than one.");
        }

        _list = new(maxCount);
        MaxCount = maxCount;
    }

    public int MaxCount { get; }

    public void Push<TKey>(Expression<Func<TSource, TKey>> keySelector)
    {
        var match = _list.FirstOrDefault(x => x.HasSameKey(keySelector));
        var direction = match?.Direction is SortDirection.Ascending
            ? SortDirection.Descending
            : SortDirection.Ascending;
        var sortExpression = new SortExpression<TKey>(keySelector, direction);
        Push(sortExpression);
    }

    private void Push(ISortExpression expression)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if (_list[i].HasSameKey(expression))
            {
                _list.RemoveAt(i);
            }
        }

        while (_list.Count >= MaxCount)
        {
            _list.RemoveAt(_list.Count - 1);
        }

        _list.Insert(0, expression);
    }

    public void Clear()
        => _list.Clear();

    public IQueryable<TSource> Apply(IQueryable<TSource> source)
        => _list.Count is 0
        ? source
        : _list[1..].Aggregate(_list[0].OrderBy(source), (q, next) => next.ThenBy(q));

    private interface ISortExpression
    {
        SortDirection Direction { get; }

        IOrderedQueryable<TSource> OrderBy(IQueryable<TSource> source);

        IOrderedQueryable<TSource> ThenBy(IOrderedQueryable<TSource> source);

        bool HasSameKey(ISortExpression other);

        bool HasSameKey(LambdaExpression expression);
    }

    private sealed class SortExpression<TKey> : ISortExpression
    {
        public SortExpression(Expression<Func<TSource, TKey>> keySelector, SortDirection direction)
        {
            KeySelector = keySelector;
            Direction = direction;
        }

        public Expression<Func<TSource, TKey>> KeySelector { get; }

        public SortDirection Direction { get; }

        public bool HasSameKey(ISortExpression other)
            => other is SortExpression<TKey> exp
            && HasSameKey(exp.KeySelector);

        public bool HasSameKey(LambdaExpression expression)
            => expression is Expression<Func<TSource, TKey>> other
            && HasSameKey(other);

        public bool HasSameKey(Expression<Func<TSource, TKey>> other)
            => ((MemberExpression)other.Body).Member
            == ((MemberExpression)KeySelector.Body).Member;

        public IOrderedQueryable<TSource> OrderBy(IQueryable<TSource> source)
            => Direction is SortDirection.Ascending
            ? source.OrderBy(KeySelector)
            : source.OrderByDescending(KeySelector);

        public IOrderedQueryable<TSource> ThenBy(IOrderedQueryable<TSource> source)
            => Direction is SortDirection.Ascending
            ? source.ThenBy(KeySelector)
            : source.ThenByDescending(KeySelector);
    }

    private enum SortDirection
    {
        Ascending,
        Descending,
    }
}