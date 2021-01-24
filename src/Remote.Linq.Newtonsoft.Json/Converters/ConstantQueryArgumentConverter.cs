// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Newtonsoft.Json.Converters
{
    using Aqua.Dynamic;
    using Aqua.Newtonsoft.Json;
    using Aqua.Newtonsoft.Json.Converters;
    using global::Newtonsoft.Json;
    using Remote.Linq.DynamicQuery;
    using System;

    public sealed class ConstantQueryArgumentConverter : DynamicObjectConverter
    {
        public ConstantQueryArgumentConverter(KnownTypesRegistry knownTypeRegistry)
            : base(knownTypeRegistry)
        {
        }

        protected override DynamicObject CreateObject(Type type)
        {
            if (type != typeof(ConstantQueryArgument))
            {
                throw new JsonSerializationException($"{nameof(ConstantQueryArgumentConverter)} must be user for {typeof(ConstantQueryArgument).FullName} type only.");
            }

            return new ConstantQueryArgument();
        }
    }
}
