// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.DynamicQuery
{
    using Remote.Linq.TestSupport;
    using System.Collections.Generic;
    using System.Linq;

    internal static class EntityQueryables
    {
        public static IEnumerable<Entity> Enumerable =>
            System.Linq.Enumerable.Range(0, 10)
            .Select(x => new Entity(x));

        public static IQueryable<Entity> AsyncRemoteQueryable =>
            Enumerable
            .AsAsyncRemoteQueryable();

        public static IQueryable<Entity> FilteredAsyncRemoteQueryable =>
            from item in AsyncRemoteQueryable
            where item.Id >= 5 && item.Id <= 7
            select item;

        public static IAsyncRemoteStreamQueryable<Entity> AsyncRemoteStreamQueryable =
            Enumerable
            .AsAsyncRemoteStreamQueryable();

        public static IQueryable<Entity> FilteredAsyncRemoteStreamQueryable =
            from x in AsyncRemoteStreamQueryable
            where x.Id >= 5 && x.Id <= 7
            select x;
    }
}