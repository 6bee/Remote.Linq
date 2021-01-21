// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.SimpleAsyncStreamProtocol
{
    public readonly struct AsyncQuery<T> : IQuery<T>
    {
        public T Request { get; init; }
    }
}
