// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.TypeSystem;
    using System;

    public class ResultTypeMapper : TypeResolver
    {
        public override Type ResolveType(TypeInfo typeInfo)
        {
            if (typeInfo is null)
            {
                return null;
            }

            if (typeInfo.Namespace == "Common.Model")
            {
                typeInfo.Namespace = "Client.ClientModel";
            }

            Type type = base.ResolveType(typeInfo);
            return type;
        }
    }
}