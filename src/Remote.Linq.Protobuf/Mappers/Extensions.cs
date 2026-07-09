// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

internal static class Extensions
{
    extension<T>(T[] array)
    {
        public List<T>? ToListOrNull() => array.Length is 0 ? null : [.. array];
    }
}
