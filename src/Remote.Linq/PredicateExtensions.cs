// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

using System;
using System.ComponentModel;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class PredicateExtensions
{
    /// <summary>
    /// Combines two predicates with boolean AND. In case of one predicate is null, the other is returned without being combined.
    /// </summary>
    public static Func<T, bool>? And<T>(this Func<T, bool>? predicate1, Func<T, bool>? predicate2)
    {
        if (predicate1 is null)
        {
            return predicate2;
        }

        if (predicate2 is null)
        {
            return predicate1;
        }

        return x => predicate1(x) && predicate2(x);
    }

    /// <summary>
    /// Combines two predicates with boolean OR. In case of one predicate is null, the other is returned without being combined.
    /// </summary>
    public static Func<T, bool>? Or<T>(this Func<T, bool>? predicate1, Func<T, bool>? predicate2)
    {
        if (predicate1 is null)
        {
            return predicate2;
        }

        if (predicate2 is null)
        {
            return predicate1;
        }

        return x => predicate1(x) || predicate2(x);
    }
}