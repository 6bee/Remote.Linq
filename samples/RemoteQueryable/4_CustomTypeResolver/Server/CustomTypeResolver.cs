// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Remote.Linq.TypeSystem;
    using System;

    public class CustomTypeResolver : TypeResolver
    {
        public override Type ResolveType(TypeInfo typeInfo)
        {
            if (typeInfo.Namespace == "Client.Model")
            {
                switch (typeInfo.Name)
                {
                    case "OrderItem":
                        return typeof(Server.Model.OrderItem);

                    case "Product":
                        return typeof(Server.Model.Product);

                    case "ProductCategory":
                        return typeof(Server.Model.ProductCategory);
                }
            }

            return base.ResolveType(typeInfo);
        }
    }
}
