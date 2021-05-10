// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Collections.Generic;
    using System.Linq;

    public interface IRemoteQueryable<out T> : IRemoteQueryable, IQueryable<T>
    {
        IEnumerable<T> Execute();
    }
}