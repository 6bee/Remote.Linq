// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Linq;

    public interface IRemoteQueryable : IQueryable
    {
        new IRemoteQueryProvider Provider { get; }
    }
}
