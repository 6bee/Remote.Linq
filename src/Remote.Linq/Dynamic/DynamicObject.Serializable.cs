// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Remote.Linq.Dynamic
{
    [Serializable]
    partial class DynamicObject : ISerializable
    {
        protected DynamicObject(SerializationInfo info, StreamingContext context)
        {
            Type = (TypeInfo)info.GetValue("Type", typeof(TypeInfo));
            Members = (Dictionary<string, object>)info.GetValue("Members", typeof(Dictionary<string, object>));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Type", Type);
            info.AddValue("Members", Members);
        }
    }
}