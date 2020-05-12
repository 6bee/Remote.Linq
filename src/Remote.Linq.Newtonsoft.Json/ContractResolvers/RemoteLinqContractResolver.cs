﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Newtonsoft.Json.ContractResolvers
{
    using Aqua.Newtonsoft.Json.ContractResolvers;
    using Aqua.Newtonsoft.Json.Converters;
    using global::Newtonsoft.Json;
    using global::Newtonsoft.Json.Serialization;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using Remote.Linq.Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    public sealed class RemoteLinqContractResolver : DefaultContractResolver
    {
        private static readonly Dictionary<Type, JsonConverter> _converters = new Dictionary<Type, JsonConverter>
        {
            { typeof(Expression), new ExpressionConverter() },
            { typeof(ConstantQueryArgument), new ConstantQueryArgumentConverter() },
            { typeof(VariableQueryArgument), new VariableQueryArgumentConverter() },
            { typeof(VariableQueryArgumentList), new VariableQueryArgumentListConverter() },
        };

        private readonly IContractResolver _decorated;

        public RemoteLinqContractResolver(IContractResolver? decorated = null)
        {
            if (decorated is RemoteLinqContractResolver self)
            {
                decorated = self._decorated;
            }

            _decorated = (decorated as AquaContractResolver) ?? new AquaContractResolver(decorated);
        }

        public override JsonContract ResolveContract(Type type)
            => _decorated is null || IsTypeHandled(type)
            ? base.ResolveContract(type)
            : _decorated.ResolveContract(type);

        protected override JsonContract CreateContract(Type objectType)
            => IsTypeHandled(objectType)
            ? CreateObjectContract(objectType)
            : base.CreateContract(objectType);

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);

            if (IsTypeHandled(objectType))
            {
                contract.IsReference = true;
                contract.ItemReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                var converter = _converters
                    .Where(x => x.Key.IsAssignableFrom(objectType))
                    .Select(x => x.Value)
                    .FirstOrDefault();
                contract.Converter = converter ?? CreateObjectConverter(objectType);
                foreach (var property in contract.Properties.Where(x => !x.Writable || !x.Readable))
                {
                    property.Ignored = true;
                }
            }

            return contract;
        }

        private static bool IsTypeHandled(Type type)
            => Equals(type.Assembly, typeof(Expression).Assembly)
            && type.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0;

        private static JsonConverter CreateObjectConverter(Type type)
            => (JsonConverter)Activator.CreateInstance(typeof(ObjectConverter<>).MakeGenericType(type));
    }
}