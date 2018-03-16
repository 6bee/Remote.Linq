// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua;
    using Newtonsoft.Json.Serialization;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using System;

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
            return type == typeof(ConstantExpression)
                || type == typeof(VariableQueryArgument)
                || type == typeof(VariableQueryArgumentList);
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);

            if (objectType == typeof(ConstantExpression))
            {
                contract.OnDeserializedCallbacks.Add(InstanceTypeConverter.ConstantExpressionSerializationCallback);
            }
            else if (objectType == typeof(VariableQueryArgument))
            {
                contract.OnDeserializedCallbacks.Add(InstanceTypeConverter.VariableQueryArgumentSerializationCallback);
            }
            else if (objectType == typeof(VariableQueryArgumentList))
            {
                contract.OnDeserializedCallbacks.Add(InstanceTypeConverter.VariableQueryArgumentListSerializationCallback);
            }

            return contract;
        }
    }
}
