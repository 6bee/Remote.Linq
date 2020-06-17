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
#nullable disable
        public TKey Key { get; set; }

        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Property serves as serialization contract")]
        public TElement[] Elements { get; set; }

        public IEnumerator<TElement> GetEnumerator() => ((IEnumerable<TElement>)Elements).GetEnumerator();
#nullable restore

        IEnumerator IEnumerable.GetEnumerator() => Elements.GetEnumerator();
    }
}
