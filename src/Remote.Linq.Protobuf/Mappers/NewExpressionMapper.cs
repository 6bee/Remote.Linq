// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class NewExpressionMapper : ProtoMapper<NewExpression, Proto.NewExpression>
{
    public static readonly NewExpressionMapper Instance = new();

    public override NewExpression FromProto(Proto.NewExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Constructor = ConstructorInfoMapper.Instance.FromProto(proto.Constructor, context),
            Type = TypeInfoMapper.Instance.FromProto(proto.Type, context),
            Arguments = ExpressionMapper.Instance.FromProto(proto.Arguments, context).ToListOrNull(),
            Members = MemberInfoMapper.Instance.FromProto(proto.Members, context).ToListOrNull(),
        };

    public override Proto.NewExpression ToProto(NewExpression value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.NewExpression
        {
            Constructor = value.Constructor is null ? null : ConstructorInfoMapper.Instance.ToProto(value.Constructor, context),
            Type = TypeInfoMapper.Instance.ToProto(value.Type, context),
        };
        ExpressionMapper.Instance.ToProto(result.Arguments, value.Arguments, context);
        MemberInfoMapper.Instance.ToProto(result.Members, value.Members, context);
        return result;
    }
}
