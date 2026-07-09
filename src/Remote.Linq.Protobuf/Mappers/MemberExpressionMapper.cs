// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class MemberExpressionMapper : ProtoMapper<MemberExpression, Proto.MemberExpression>
{
    public static readonly MemberExpressionMapper Instance = new();

    public override MemberExpression FromProto(Proto.MemberExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Expression = ExpressionMapper.Instance.FromProto(proto.Expression, context),
            Member = MemberInfoMapper.Instance.FromProto(proto.Member, context),
        };

    public override Proto.MemberExpression ToProto(MemberExpression value, ProtoContext context)
        => value is null ? null! : new()
        {
            Expression = value.Expression is null ? null : ExpressionMapper.Instance.ToProto(value.Expression, context),
            Member = MemberInfoMapper.Instance.ToProto(value.Member, context),
        };
}
