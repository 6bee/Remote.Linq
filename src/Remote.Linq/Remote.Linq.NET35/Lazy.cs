// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
    using System.Threading;

    // Custom implementation of the corresponding .NET framework class
    internal sealed class Lazy<T>
    {
        private static readonly object _lock = new object();
        private readonly bool _isThreadSafe;
        private readonly Func<T> _valueFactory;
        private bool _isValueCreated = false;
        private T _value;

        public Lazy(Func<T> valueFactory, bool isThreadSafe)
        {
            _valueFactory = valueFactory;
            _isThreadSafe = isThreadSafe;
        }

        public T Value
        {
            get
            {
                if (!_isValueCreated)
                {
                    if (_isThreadSafe)
                    {
                        Monitor.Enter(_lock);
                    }

                    try
                    {
                        if (!_isValueCreated)
                        {
                            _value = _valueFactory();
                            _isValueCreated = true;
                        }
                    }
                    finally
                    {
                        if (_isThreadSafe)
                        {
                            Monitor.Exit(_lock);
                        }
                    }
                }

                return _value;
            }
        }
    }
}
