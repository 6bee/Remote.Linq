// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    internal static class InstanceTypeConverter
    {
        public static void ConstantExpressionSerializationCallback(object o, StreamingContext context)
        {
            var constantExpression = o as ConstantExpression;
            if (!ReferenceEquals(null, constantExpression?.Value))
            {
                var converter = GetConverter(constantExpression.Type);
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
                var converter = GetConverter(variableQueryArgument.Type);
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
                var converter = GetConverter(variableQueryArgumentList.ElementType);
                if (!ReferenceEquals(null, converter))
                {
                    variableQueryArgumentList.Values = variableQueryArgumentList.Values.Select(converter).ToList();
                }
            }
        }

        private static Func<object, object> GetConverter(Aqua.TypeSystem.TypeInfo typeInfo)
        {
            if (ReferenceEquals(null, typeInfo))
            {
                return null;
            }

            if (string.Equals(typeInfo.FullName, typeof(int).FullName))
            {
                return x => Convert.ToInt32(x);
            }

            if (string.Equals(typeInfo.FullName, typeof(char).FullName))
            {
                return x => Convert.ToChar(x);
            }

            if (string.Equals(typeInfo.FullName, typeof(Nullable<>).FullName) && typeInfo.GenericArguments?.Count == 1)
            {
                var converter = GetConverter(typeInfo.GenericArguments.Single());
                return x => ReferenceEquals(null, x) ? null : converter(x);
            }

            return null;
        }
    }
}
