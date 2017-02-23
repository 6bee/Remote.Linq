// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

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
            if (typeof(ConstantExpression) == type)
            {
                return base.ResolveContract(type);
            }

            return _decorated.ResolveContract(type);
        }

        //protected override JsonContract CreateContract(Type objectType)
        //{
        //    //if (typeof(DynamicObject).IsAssignableFrom(objectType))
        //    //{
        //    //    return CreateObjectContract(objectType);
        //    //}

        //    return base.CreateContract(objectType);
        //}

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);

            //if (typeof(DynamicObject).IsAssignableFrom(objectType))
            //{
            //    contract.IsReference = true;
            //    foreach(var property in contract.Properties.Where(x => !x.Writable || !x.Readable))
            //    {
            //        property.Ignored = true;
            //    }
            //}
            
            return contract;
        }
    }
}
