// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class ElementInitMapper : ProtoMapper<ElementInit, Proto.ElementInit>
{
    public static readonly ElementInitMapper Instance = new();

    public override ElementInit FromProto(Proto.ElementInit proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            AddMethod = MethodInfoMapper.Instance.FromProto(proto.AddMethod, context),
            Arguments = [.. ExpressionMapper.Instance.FromProto(proto.Arguments, context)],
        };

    public override Proto.ElementInit ToProto(ElementInit value, ProtoContext context)
    {
        value.AssertNotNull();
        context.AssertNotNull();

        var result = new Proto.ElementInit
        {
            AddMethod = MethodInfoMapper.Instance.ToProto(value.AddMethod, context),
        };
        ExpressionMapper.Instance.ToProto(result.Arguments, value.Arguments, context);
        return result;
    }
}
