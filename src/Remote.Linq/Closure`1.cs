// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    internal sealed class Closure<T>
    {
        public Closure(T value)
            => Value = value;

        public T Value { get; }
    }
}