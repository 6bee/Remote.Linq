// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remote.Linq
{
    internal static class ListExtensions
    {
        public static ReadOnlyCollection<T> AsReadOnly<T>(this List<T> list)
        {
            return new ReadOnlyCollection<T>(list);
        }
    }
}