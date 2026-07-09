// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class MemberListBindingMapper : ProtoMapper<MemberListBinding, Proto.MemberListBinding>
{
    public static readonly MemberListBindingMapper Instance = new();

    public override MemberListBinding FromProto(Proto.MemberListBinding proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Member = MemberInfoMapper.Instance.FromProto(proto.Member, context),
            Initializers = [.. ElementInitMapper.Instance.FromProto(proto.Initializers, context)],
        };

    public override Proto.MemberListBinding ToProto(MemberListBinding value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.MemberListBinding
        {
            Member = MemberInfoMapper.Instance.ToProto(value.Member, context),
        };
        ElementInitMapper.Instance.ToProto(result.Initializers, value.Initializers, context);
        return result;
    }
}
