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
            if (!ReferenceEquals(null, constantExpression?.Value))
            {
                var converter = PrimitiveValueInspector.GetConverter(constantExpression.Type);
                if (!ReferenceEquals(null, converter))
                {
                    constantExpression.Value = converter(constantExpression.Value);
                }
            }
        }

        public static void VariableQueryArgumentSerializationCallback(object o, StreamingContext context)
        {
            var variableQueryArgument = o as VariableQueryArgument;
            if (!ReferenceEquals(null, variableQueryArgument?.Value))
            {
                var converter = PrimitiveValueInspector.GetConverter(variableQueryArgument.Type);
                if (!ReferenceEquals(null, converter))
                {
                    variableQueryArgument.Value = converter(variableQueryArgument.Value);
                }
            }
        }

        public static void VariableQueryArgumentListSerializationCallback(object o, StreamingContext context)
        {
            var variableQueryArgumentList = o as VariableQueryArgumentList;
            if (!ReferenceEquals(null, variableQueryArgumentList?.Values))
            {
                var converter = PrimitiveValueInspector.GetConverter(variableQueryArgumentList.ElementType);
                if (!ReferenceEquals(null, converter))
                {
                    variableQueryArgumentList.Values = variableQueryArgumentList.Values.Select(converter).ToList();
                }
            }
        }
    }
}
