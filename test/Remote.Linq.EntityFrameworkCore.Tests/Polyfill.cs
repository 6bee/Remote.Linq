// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests;

#if NETFRAMEWORK
internal static class Polyfill
{
    extension(ValueTask)
    {
        public static ValueTask<T> FromResult<T>(T result) => new(result);
    }
}
#endif // NETFRAMEWORK
