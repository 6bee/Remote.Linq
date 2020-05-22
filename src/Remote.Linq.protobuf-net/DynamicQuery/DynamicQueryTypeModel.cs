// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ProtoBuf.DynamicQuery
{
    using Aqua.Dynamic;
    using Aqua.ProtoBuf;
    using global::ProtoBuf.Meta;
    using Remote.Linq.DynamicQuery;

    internal static class DynamicQueryTypeModel
    {
        public static RuntimeTypeModel ConfigureRemoteLinqDynamicQueryTypes(this RuntimeTypeModel typeModel)
        {
            typeModel.GetType<DynamicObject>().AddSubType(5, typeof(ConstantQueryArgument));

            _ = typeModel.GetType<QueryableResourceDescriptor>();

            typeModel.GetType<VariableQueryArgument>().SetSurrogate<VariableQueryArgumentSurrogate>();

            typeModel.GetType<VariableQueryArgumentList>().SetSurrogate<VariableQueryArgumentListSurrogate>();

            _ = typeModel.GetType<VariableQueryArgumentList>();

            return typeModel;
        }
    }
}
