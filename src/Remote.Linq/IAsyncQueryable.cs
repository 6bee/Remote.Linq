// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remote.Linq
{
    internal interface IAsyncQueryable<T> : IQueryable<T>
    {
        Task<IEnumerable<T>> ExecuteAsync();
    }
}
