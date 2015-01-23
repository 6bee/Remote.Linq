// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.TypeSystem
{
    using System;

    [Serializable]
    [Flags]
    public enum MemberTypes
    {
        Constructor = 1,
        Field = 4,
        Method = 8,
        Property = 16,
    }
}
