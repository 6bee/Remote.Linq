// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.RemoteQueryable.QueryTestData
{
    using System;

    [Flags]
    public enum PruductTags
    {
        None = 0,
        BestPrice = 1,
        TopSelling = 2,
        Premium = 4,
    }
}