// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace BlazorApp.Client.Helper;

public static class QueryableExtensions
{
    public static IQueryable<T> Apply<T>(this IQueryable<T> source, SortingCollection<T> sortings)
        => sortings.Apply(source);

    public static IQueryable<T> Apply<T>(this IQueryable<T> source, Pagination pagination)
        => pagination.Apply(source);
}