// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    public interface IIncludableRemoteQueryable<out T, out TProperty> : IRemoteQueryable<T>
    {
    }
}
