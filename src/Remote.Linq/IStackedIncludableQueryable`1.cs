// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    internal interface IStackedIncludableQueryable<out T> : IRemoteQueryable<T>
    {
        public IRemoteQueryable<T> Parent { get; }

        public string IncludePath { get; }
    }
}
