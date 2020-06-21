// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Ix.DynamicQuery
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;

    internal interface IRemoteLinqAsyncQueryProvider : IAsyncQueryProvider
    {
        IAsyncEnumerator<T> GetAsyncEnumerator<T>(Expression expression, CancellationToken token);
    }
}
