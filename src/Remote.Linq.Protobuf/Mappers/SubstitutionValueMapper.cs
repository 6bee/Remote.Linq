// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.DynamicQuery;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class SubstitutionValueMapper : ProtoMapper<SubstitutionValue, Proto.SubstitutionValue>
{
    public static readonly SubstitutionValueMapper Instance = new();

    public override SubstitutionValue FromProto(Proto.SubstitutionValue proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Type = TypeInfoMapper.Instance.FromProto(proto.Type, context),
        };

    public override Proto.SubstitutionValue ToProto(SubstitutionValue value, ProtoContext context)
        => value is null ? null! : new()
        {
            Type = TypeInfoMapper.Instance.ToProto(value.Type, context),
        };
}
