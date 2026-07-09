// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class TypeBinaryExpressionMapper : ProtoMapper<TypeBinaryExpression, Proto.TypeBinaryExpression>
{
    public static readonly TypeBinaryExpressionMapper Instance = new();

    public override TypeBinaryExpression FromProto(Proto.TypeBinaryExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Expression = ExpressionMapper.Instance.FromProto(proto.Expression, context),
            TypeOperand = TypeInfoMapper.Instance.FromProto(proto.TypeOperand, context),
        };

    public override Proto.TypeBinaryExpression ToProto(TypeBinaryExpression value, ProtoContext context)
        => value is null ? null! : new()
        {
            Expression = ExpressionMapper.Instance.ToProto(value.Expression, context),
            TypeOperand = TypeInfoMapper.Instance.ToProto(value.TypeOperand, context),
        };
}
