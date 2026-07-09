// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class UnaryExpressionFormatter : IMessagePackFormatter<UnaryExpression?>
{
    public static readonly UnaryExpressionFormatter Instance = new();

    private const int FieldCount = 4;

    public void Serialize(ref MessagePackWriter writer, UnaryExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        writer.Write((int)value.UnaryOperator);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Operand, options);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.Type, options);
        MethodInfoFormatter.Instance.Serialize(ref writer, value.Method, options);
    }

    public UnaryExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var op = len > 0 ? (UnaryOperator)reader.ReadInt32() : default;
        var operand = len > 1 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var type = len > 2 ? TypeInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        var method = len > 3 ? MethodInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new UnaryExpression { UnaryOperator = op, Operand = operand!, Type = type!, Method = method };
    }
}
