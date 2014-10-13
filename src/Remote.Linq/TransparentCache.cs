// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A weak-reference cache that can be hooked-in method calls to serve cached instances 
    /// or transparently create the requested value if not contained in cache
    /// </summary>
    internal class TransparentCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, WeakReference> _cache = new Dictionary<TKey, WeakReference>();
        private readonly int _cleanupDelay;
        private bool _isCleanupScheduled = false;

        /// <summary>
        /// Creates an new instance of <see cref="TransparentCache"/>
        /// </summary>
        /// <param name="cleanupDelay">Number of milliseconds to delay the task to clean-up stale references. Set to -1 to suppress clean-up or 0 to run clean-up synchronously.</param>
        public TransparentCache(int cleanupDelay = 2000)
        {
            if (cleanupDelay < -1)
            {
                throw new ArgumentOutOfRangeException("cleanupDelay", "expected values equal or greater than -1");
            }

            _cleanupDelay = cleanupDelay;
        }

        /// <summary>
        /// Returns the cached value if it's contained in the cache, otherwise it creates and adds the value to the cache.
        /// </summary>
        /// <remarks>
        /// This method also performes a cleanup of stale references according the cleanup-delay specified via cunstructor parameter.
        /// The cleanup task is started only if no other cleanup is pending.
        /// </remarks>
        public TValue GetOrCreate(TKey key, Func<TKey, TValue> factory)
        {
            var value = default(TValue);

            lock (_cache)
            {
                var isReferenceAlive = false;

                // probe cache
                WeakReference weakref;
                if (_cache.TryGetValue(key, out weakref))
                {
                    value = (TValue)weakref.Target;
                    isReferenceAlive = weakref.IsAlive;
                }

                // create value if not found in cache
                if (!isReferenceAlive)
                {
                    value = factory(key);
                    _cache[key] = new WeakReference(value);
                }

                // clean-up stale references from cache
                if (_cleanupDelay == 0)
                {
                    CleanUpStaleReferences();
                }
                else if (_cleanupDelay > 0 && !_isCleanupScheduled)
                {
                    _isCleanupScheduled = true;
#if NET35 || SILVERLIGHT
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(delegate
                    {
                        System.Threading.Thread.Sleep(_cleanupDelay);
                        CleanUpStaleReferences();
                        _isCleanupScheduled = false;
                    }));
#else
                    System.Threading.Tasks.Task.Run(async delegate
                    {
                        await System.Threading.Tasks.Task.Delay(_cleanupDelay);
                        CleanUpStaleReferences();
                        _isCleanupScheduled = false;
                    });
#endif
                }
            }

            return value;
        }

        /// <summary>
        /// Removed cached items with stale references.
        /// </summary>
        protected void CleanUpStaleReferences()
        {
            lock (_cache)
            {
                foreach (var iten in _cache.Where(x => !x.Value.IsAlive).ToArray())
                {
                    _cache.Remove(iten.Key);
                }
            }
        }
    }
}
