// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.TypeSystem;
    using System;

    public class CustomTypeResolver : TypeResolver
    {
        public override Type ResolveType(TypeInfo typeInfo)
        {
            if (typeInfo.Namespace == "Client.ClientModel")
            {
                switch (typeInfo.Name)
                {
                    case "OrderItem":
                        return typeof(ServerModel.OrderItem);

                    case "Product":
                        return typeof(ServerModel.Product);

                    case "ProductCategory":
                        return typeof(ServerModel.ProductCategory);
                }
            }

            return base.ResolveType(typeInfo);
        }
    }
}
