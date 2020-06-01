// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ProtoBuf.DynamicQuery
{
    using Aqua.ProtoBuf;
    using Remote.Linq.DynamicQuery;

    internal static class DynamicQueryTypeModel
    {
        public static AquaTypeModel ConfigureRemoteLinqDynamicQueryTypes(this AquaTypeModel typeModel)
            => typeModel
            .AddTypeSurrogate<ConstantQueryArgument, ConstantQueryArgumentSurrogate>()
            .AddType<QueryableResourceDescriptor>()
            .AddTypeSurrogate<VariableQueryArgument, VariableQueryArgumentSurrogate>()
            .AddTypeSurrogate<VariableQueryArgumentList, VariableQueryArgumentListSurrogate>();
    }
}
