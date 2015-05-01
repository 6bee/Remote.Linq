// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Linq;
    using ProtoBuf.Meta;

    internal static class VariableQueryArgumentTypeModel
    {
        public static RuntimeTypeModel ConfigureVariableQueryArgument(this RuntimeTypeModel typeModel)
        {
            var type = typeModel[typeof(VariableQueryArgument)];

            var valueProperty = type.GetFields().Single(x => string.Equals(x.Name, "Value"));
            valueProperty.DynamicType = true;
            //valueProperty.AsReference = true;

            var listType = typeModel[typeof(VariableQueryArgumentList)];
            
            var valuesProperty = listType.GetFields().Single(x => string.Equals(x.Name, "Values"));
            valuesProperty.DynamicType = true;
            //valuesProperty.AsReference = true;
            //valuesProperty.OverwriteList = true;

            return typeModel;
        }
    }
}
