// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class MemberMemberBindingMapper : ProtoMapper<MemberMemberBinding, Proto.MemberMemberBinding>
{
    public static readonly MemberMemberBindingMapper Instance = new();

    public override MemberMemberBinding FromProto(Proto.MemberMemberBinding proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Member = MemberInfoMapper.Instance.FromProto(proto.Member, context),
            Bindings = [.. MemberBindingMapper.Instance.FromProto(proto.Bindings, context)],
        };

    public override Proto.MemberMemberBinding ToProto(MemberMemberBinding value, ProtoContext context)
    {
        value.AssertNotNull();
        context.AssertNotNull();

        var result = new Proto.MemberMemberBinding
        {
            Member = MemberInfoMapper.Instance.ToProto(value.Member, context),
        };
        MemberBindingMapper.Instance.ToProto(result.Bindings, value.Bindings, context);
        return result;
    }
}
