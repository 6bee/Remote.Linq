// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests.Model
{
    using System.Diagnostics;

    [DebuggerDisplay("{Name}")]
    public class Item
    {
        public string Name { get; set; }
    }
}
