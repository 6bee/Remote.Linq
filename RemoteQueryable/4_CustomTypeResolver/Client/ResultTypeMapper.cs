// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.TypeSystem;
    using System;

    public class ResultTypeMapper : TypeResolver
    {
        public override Type ResolveType(TypeInfo typeInfo)
        {
            if (typeInfo.Namespace == "Server.ServerModel")
            {
                typeInfo.Namespace = "Client.ClientModel";
            }

            var type = base.ResolveType(typeInfo);
            return type;
        }
    }
}
