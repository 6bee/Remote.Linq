// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.TypeSystem;
    using System;

    public class QueryTypeMapper : TypeInfoProvider
    {
        public override TypeInfo GetTypeInfo(Type type, bool? includePropertyInfosOverride = null, bool? setMemberDeclaringTypesOverride = null)
        {
            TypeInfo typeInfo = base.GetTypeInfo(type, includePropertyInfosOverride, setMemberDeclaringTypesOverride);
            if (typeInfo?.Namespace == "Client.ClientModel")
            {
                typeInfo.Namespace = "Common.Model";
            }

            return typeInfo;
        }
    }
}