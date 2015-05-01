// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System.Linq;
    using ProtoBuf.Meta;

    internal static class ConstantExpressionTypeModel
    {
        public static RuntimeTypeModel ConfigureConstantExpression(this RuntimeTypeModel typeModel)
        {
            var type = typeModel[typeof(ConstantExpression)];

            type.GetFields().Single(x => string.Equals(x.Name, "Value")).DynamicType = true;

            return typeModel;
        }
    }
}
