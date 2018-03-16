// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if !NETCOREAPP1_0
namespace Remote.Linq.EntityFrameworkCore.Tests
{
    using System.Diagnostics;

    [DebuggerDisplay("{Name}")]
    public class Item
    {
        public string Name { get; set; }
    }
}
#endif