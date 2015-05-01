// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Dynamic
{
    using System.Linq;
    using ProtoBuf.Meta;

    internal static class DynamicObjectTypeModel
    {
        public static RuntimeTypeModel ConfigureDynamicObject(this RuntimeTypeModel typeModel)
        {
            typeModel[typeof(DynamicObject)].AsReferenceDefault = true;

            var type = typeModel[typeof(DynamicObject.Property)];

            var valueProperty = type.GetFields().Single(x => string.Equals(x.Name, "Value"));
            valueProperty.DynamicType = true;
            valueProperty.AsReference = true;

            return typeModel;
        }
    }
}
