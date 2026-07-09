// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class MemberAssignmentMapper : ProtoMapper<MemberAssignment, Proto.MemberAssignment>
{
    public static readonly MemberAssignmentMapper Instance = new();

    public override MemberAssignment FromProto(Proto.MemberAssignment proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Member = MemberInfoMapper.Instance.FromProto(proto.Member, context),
            Expression = ExpressionMapper.Instance.FromProto(proto.Expression, context),
        };

    public override Proto.MemberAssignment ToProto(MemberAssignment value, ProtoContext context)
        => value is null ? null! : new()
        {
            Member = MemberInfoMapper.Instance.ToProto(value.Member, context),
            Expression = ExpressionMapper.Instance.ToProto(value.Expression, context),
        };
}
