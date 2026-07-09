// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.DynamicQuery;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class VariableQueryArgumentListMapper : ProtoMapper<VariableQueryArgumentList, Proto.VariableQueryArgumentList>
{
    public static readonly VariableQueryArgumentListMapper Instance = new();

    public override VariableQueryArgumentList FromProto(Proto.VariableQueryArgumentList proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            ElementType = TypeInfoMapper.Instance.FromProto(proto.ElementType, context),
            Values = [.. ValueMapper.Instance.FromProto(proto.Values, context)],
        };

    public override Proto.VariableQueryArgumentList ToProto(VariableQueryArgumentList value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.VariableQueryArgumentList
        {
            ElementType = TypeInfoMapper.Instance.ToProto(value.ElementType, context),
        };
        ValueMapper.Instance.ToProto(result.Values, value.Values, context);
        return result;
    }
}
