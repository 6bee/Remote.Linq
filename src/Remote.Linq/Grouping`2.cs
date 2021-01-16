// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        public TKey Key { get; set; } = default!;

        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Property serves as serialization contract")]
        public TElement[] Elements { get; set; } = default!;

        public IEnumerator<TElement> GetEnumerator()
        {
            var elements = Elements ?? throw new InvalidOperationException($"{nameof(Elements)} property must not be null");
            return ((IEnumerable<TElement>)elements).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
