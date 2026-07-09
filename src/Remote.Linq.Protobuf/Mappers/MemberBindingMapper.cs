// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Aqua.TypeExtensions;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class MemberBindingMapper : ProtoMapper<MemberBinding, Proto.MemberBinding>
{
    public static readonly MemberBindingMapper Instance = new();

    public override MemberBinding FromProto(Proto.MemberBinding proto, ProtoContext context)
        => proto is null ? null! : proto.KindCase switch
        {
            Proto.MemberBinding.KindOneofCase.MemberAssignment => MemberAssignmentMapper.Instance.FromProto(proto.MemberAssignment, context),
            Proto.MemberBinding.KindOneofCase.MemberListBinding => MemberListBindingMapper.Instance.FromProto(proto.MemberListBinding, context),
            Proto.MemberBinding.KindOneofCase.MemberMemberBinding => MemberMemberBindingMapper.Instance.FromProto(proto.MemberMemberBinding, context),
            _ => throw SerializationException($"{proto.KindCase} is not supported"),
        };

    public override Proto.MemberBinding ToProto(MemberBinding value, ProtoContext context)
        => value is null ? null! : value switch
        {
            MemberAssignment v => new() { MemberAssignment = MemberAssignmentMapper.Instance.ToProto(v, context) },
            MemberListBinding v => new() { MemberListBinding = MemberListBindingMapper.Instance.ToProto(v, context) },
            MemberMemberBinding v => new() { MemberMemberBinding = MemberMemberBindingMapper.Instance.ToProto(v, context) },
            _ => throw SerializationException($"{value?.GetType().GetFriendlyName()} is not supported"),
        };

    private static ProtobufSerializationException SerializationException(string message) => new(message);
}
