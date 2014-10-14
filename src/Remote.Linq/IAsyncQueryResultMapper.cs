// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System.Threading.Tasks;

namespace Remote.Linq
{
    public interface IAsyncQueryResultMapper<TSource>
    {
        Task<TResult> MapResultAsync<TResult>(TSource source);
    }
}
