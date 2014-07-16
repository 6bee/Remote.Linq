// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System.Runtime.Serialization;

namespace Remote.Linq.Dynamic
{
    partial class DynamicObject : ISerializable
    {
        private DynamicObject(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
