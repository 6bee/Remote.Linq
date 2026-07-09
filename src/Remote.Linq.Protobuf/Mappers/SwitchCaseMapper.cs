// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class SwitchCaseMapper : ProtoMapper<SwitchCase, Proto.SwitchCase>
{
    public static readonly SwitchCaseMapper Instance = new();

    public override SwitchCase FromProto(Proto.SwitchCase proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Body = ExpressionMapper.Instance.FromProto(proto.Body, context),
            TestValues = [.. ExpressionMapper.Instance.FromProto(proto.TestValues, context)],
        };

    public override Proto.SwitchCase ToProto(SwitchCase value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.SwitchCase
        {
            Body = ExpressionMapper.Instance.ToProto(value.Body, context),
        };
        ExpressionMapper.Instance.ToProto(result.TestValues, value.TestValues, context);
        return result;
    }
}
