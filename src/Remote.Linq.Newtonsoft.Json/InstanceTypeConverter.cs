// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using System.Linq;
    using System.Runtime.Serialization;

    internal static class InstanceTypeConverter
    {
        public static void ConstantExpressionSerializationCallback(object o, StreamingContext context)
        {
            var constantExpression = o as ConstantExpression;
            if (!(constantExpression?.Value is null))
            {
                var converter = PrimitiveValueInspector.GetConverter(constantExpression.Type);
                if (!(converter is null))
                {
                    constantExpression.Value = converter(constantExpression.Value);
                }
            }
        }

        public static void VariableQueryArgumentSerializationCallback(object o, StreamingContext context)
        {
            var variableQueryArgument = o as VariableQueryArgument;
            if (!(variableQueryArgument?.Value is null))
            {
                var converter = PrimitiveValueInspector.GetConverter(variableQueryArgument.Type);
                if (!(converter is null))
                {
                    variableQueryArgument.Value = converter(variableQueryArgument.Value);
                }
            }
        }

        public static void VariableQueryArgumentListSerializationCallback(object o, StreamingContext context)
        {
            var variableQueryArgumentList = o as VariableQueryArgumentList;
            if (!(variableQueryArgumentList?.Values is null))
            {
                var converter = PrimitiveValueInspector.GetConverter(variableQueryArgumentList.ElementType);
                if (!(converter is null))
                {
                    variableQueryArgumentList.Values = variableQueryArgumentList.Values.Select(converter).ToList();
                }
            }
        }
    }
}
