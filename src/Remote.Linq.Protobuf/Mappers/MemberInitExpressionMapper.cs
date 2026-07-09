// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class MemberInitExpressionMapper : ProtoMapper<MemberInitExpression, Proto.MemberInitExpression>
{
    public static readonly MemberInitExpressionMapper Instance = new();

    public override MemberInitExpression FromProto(Proto.MemberInitExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            NewExpression = NewExpressionMapper.Instance.FromProto(proto.NewExpression, context),
            Bindings = [.. MemberBindingMapper.Instance.FromProto(proto.Bindings, context)],
        };

    public override Proto.MemberInitExpression ToProto(MemberInitExpression value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.MemberInitExpression
        {
            NewExpression = NewExpressionMapper.Instance.ToProto(value.NewExpression, context),
        };
        MemberBindingMapper.Instance.ToProto(result.Bindings, value.Bindings, context);
        return result;
    }
}
