// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if NO_ASYNC_FEATURE && !NET35

namespace Remote.Linq.DynamicQuery
{
    using System.Threading.Tasks;

    internal sealed class AsyncObjectResultCaster : IAsyncQueryResultMapper<object>
    {
        public Task<TResult> MapResultAsync<TResult>(object source)
        {
            return Task.Factory.StartNew(() => (TResult)source);
        }
    }
}

#endif