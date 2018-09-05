// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        public TKey Key { get; set; }

        public TElement[] Elements { get; set; }

        public IEnumerator<TElement> GetEnumerator()
            => ((IEnumerable<TElement>)Elements).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Elements.GetEnumerator();
    }
}
