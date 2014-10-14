// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System.Threading.Tasks;

namespace Remote.Linq.DynamicQuery
{
    internal sealed class AsyncObjectResultCaster : IAsyncQueryResultMapper<object>
    {
        public Task<TResult> MapResultAsync<TResult>(object source)
        {
            return Task.Factory.StartNew(() => (TResult)source);
        }
    }
}
