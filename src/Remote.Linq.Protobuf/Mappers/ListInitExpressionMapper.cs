// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class ListInitExpressionMapper : ProtoMapper<ListInitExpression, Proto.ListInitExpression>
{
    public static readonly ListInitExpressionMapper Instance = new();

    public override ListInitExpression FromProto(Proto.ListInitExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            NewExpression = NewExpressionMapper.Instance.FromProto(proto.NewExpression, context),
            Initializers = [.. ElementInitMapper.Instance.FromProto(proto.Initializers, context)],
        };

    public override Proto.ListInitExpression ToProto(ListInitExpression value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.ListInitExpression
        {
            NewExpression = NewExpressionMapper.Instance.ToProto(value.NewExpression, context),
        };
        ElementInitMapper.Instance.ToProto(result.Initializers, value.Initializers, context);
        return result;
    }
}
