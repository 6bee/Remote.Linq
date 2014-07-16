// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Remote.Linq.Dynamic
{
    /// <summary>
    /// Marker type used as a descriptor of <see cref="IQueryable{T}" /> types within linq expressions
    /// </summary>
    /// <remarks>This class is meant for internal use only!</remarks>
    /// <typeparam name="T"></typeparam>
    internal sealed class QueryableDescriptor<T> : QueryableDescriptor, IQueryable<T>
    {
        internal QueryableDescriptor()
            : base(typeof(T))
        {
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
