// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Remote.Linq.Dynamic
{
    public interface IDynamicObjectMapper
    {
        object Map(DynamicObject obj, Type targetType = null);

        T Map<T>(DynamicObject obj);

        IEnumerable<object> Map(IEnumerable<DynamicObject> objects, Type targetType = null);

        IEnumerable<T> Map<T>(IEnumerable<DynamicObject> objects);

        DynamicObject MapObject(object obj);

        IEnumerable<DynamicObject> MapCollection(object objects);
    }
}
