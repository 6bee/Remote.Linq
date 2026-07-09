// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class ParameterExpressionFormatter : IMessagePackFormatter<ParameterExpression?>
{
    public static readonly ParameterExpressionFormatter Instance = new();

    private const int FieldCount = 3;

    public void Serialize(ref MessagePackWriter writer, ParameterExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        writer.Write(value.ParameterName);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.ParameterType, options);
        writer.Write(value.InstanceId);
    }

    public ParameterExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var name = len > 0 ? reader.ReadString() : null;
        var paramType = len > 1 ? TypeInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        var id = len > 2 ? reader.ReadInt32() : 0;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new ParameterExpression { ParameterName = name, ParameterType = paramType!, InstanceId = id };
    }
}
