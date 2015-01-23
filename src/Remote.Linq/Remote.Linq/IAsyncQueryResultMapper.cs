// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Threading.Tasks;

    public interface IAsyncQueryResultMapper<TSource>
    {
        Task<TResult> MapResultAsync<TResult>(TSource source);
    }
}
