// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.DynamicQuery;
using Proto = Remote.Linq.Protobuf.Schema;

// TODO: consider introducing a ConstantValue type for to be used by ConstantExpression.Value to distinguish from object type values

/// <summary>
/// <see cref="ConstantValueMapper"/> is not registered to not overrule <c>object</c> mapping (provided by <see cref="ValueMapper"/>)
/// but is used from <see cref="ConstantExpressionMapper"/> directly.
/// </summary>
public sealed class ConstantValueMapper : ProtoMapper<object?, Proto.ConstantValue>
{
    public static readonly ConstantValueMapper Instance = new();

    public override object? FromProto(Proto.ConstantValue proto, ProtoContext context)
        => proto is null ? null! : proto.KindCase switch
        {
            Proto.ConstantValue.KindOneofCase.ConstantQueryArgument => ConstantQueryArgumentMapper.Instance.FromProto(proto.ConstantQueryArgument, context),
            Proto.ConstantValue.KindOneofCase.VariableQueryArgument => VariableQueryArgumentMapper.Instance.FromProto(proto.VariableQueryArgument, context),
            Proto.ConstantValue.KindOneofCase.VariableQueryArgumentList => VariableQueryArgumentListMapper.Instance.FromProto(proto.VariableQueryArgumentList, context),
            Proto.ConstantValue.KindOneofCase.SubstitutionValue => SubstitutionValueMapper.Instance.FromProto(proto.SubstitutionValue, context),
            Proto.ConstantValue.KindOneofCase.QueryableResourceDescriptor => QueryableResourceDescriptorMapper.Instance.FromProto(proto.QueryableResourceDescriptor, context),
            Proto.ConstantValue.KindOneofCase.Value => ValueMapper.Instance.FromProto(proto.Value, context),
            _ => throw SerializationException($"{proto.KindCase} is not supported"),
        };

    public override Proto.ConstantValue ToProto(object? value, ProtoContext context)
        => value is null ? null! : value switch
        {
            ConstantQueryArgument v => new() { ConstantQueryArgument = ConstantQueryArgumentMapper.Instance.ToProto(v, context) },
            VariableQueryArgument v => new() { VariableQueryArgument = VariableQueryArgumentMapper.Instance.ToProto(v, context) },
            VariableQueryArgumentList v => new() { VariableQueryArgumentList = VariableQueryArgumentListMapper.Instance.ToProto(v, context) },
            SubstitutionValue v => new() { SubstitutionValue = SubstitutionValueMapper.Instance.ToProto(v, context) },
            QueryableResourceDescriptor v => new() { QueryableResourceDescriptor = QueryableResourceDescriptorMapper.Instance.ToProto(v, context) },
            _ => new() { Value = ValueMapper.Instance.ToProto(value, context) },
        };

    private static ProtobufSerializationException SerializationException(string message) => new(message);
}
