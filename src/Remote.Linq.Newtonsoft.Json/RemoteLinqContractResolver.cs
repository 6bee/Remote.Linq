// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua;
    using Newtonsoft.Json.Serialization;
    using Remote.Linq.JsonConverters;
    using System;
    using System.Reflection;
    using Newtonsoft.Json;
    using Remote.Linq.Expressions;
    using System.Runtime.Serialization;

    public class RemoteLinqContractResolver : DefaultContractResolver
    {
        private readonly IContractResolver _decorated;

        public RemoteLinqContractResolver(IContractResolver decorated = null)
        {
            _decorated = decorated is AquaContractResolver
                ? decorated
                : new AquaContractResolver(decorated);
        }

        public override JsonContract ResolveContract(Type type)
        {
            if (IsTypeHandled(type))
            {
                return base.ResolveContract(type);
            }

            return _decorated.ResolveContract(type);
        }

        private static bool IsTypeHandled(Type type)
        {
            return type == typeof(ConstantExpression);
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);

            if (objectType == typeof(ConstantExpression))
            {
                // workaround: since ConstantExpressionJsonConverter.ReadJson() 
                //             doesn't seem to get called
                contract.OnDeserializedCallbacks.Add(SerializationCallback);
            }

            return contract;
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType == typeof(ConstantExpression))
            {
                return new ConstantExpressionJsonConverter();
            }

            return base.ResolveContractConverter(objectType);
        }

        private static void SerializationCallback(object o, StreamingContext context)
        {
            var constantExpression = o as ConstantExpression;
            if (!ReferenceEquals(null, constantExpression))
            {
                var typeName = constantExpression?.Type?.FullName;
                if (constantExpression.Value is long)
                {
                    if (string.Equals(typeName, typeof(int).FullName))
                    {
                        constantExpression.Value = (int)(long)constantExpression.Value;
                    }
                }
            }
        }
    }
}
