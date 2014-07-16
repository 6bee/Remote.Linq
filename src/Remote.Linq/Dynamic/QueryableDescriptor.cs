// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Remote.Linq.Dynamic
{
    /// <summary>
    /// Marker type used as a descriptor of <see cref="IQueryable" /> types within linq expressions
    /// </summary>
    /// <remarks>This class is meant for internal use only!</remarks>
    internal class QueryableDescriptor : IQueryable
    {
        private readonly Type _elementType;

        internal QueryableDescriptor(Type elementType)
        {
            _elementType = elementType;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        Type IQueryable.ElementType
        {
            get { return _elementType; }
        }

        System.Linq.Expressions.Expression IQueryable.Expression
        {
            get { throw new NotImplementedException(); }
        }

        IQueryProvider IQueryable.Provider
        {
            get { throw new NotImplementedException(); }
        }
    }
}
