// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.SimpleAsyncQueryProtocol
{
    public interface IQuery<T>
    {
        T Request { get; }
    }
}