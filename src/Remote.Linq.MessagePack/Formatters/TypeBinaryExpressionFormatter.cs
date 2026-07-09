// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class TypeBinaryExpressionFormatter : IMessagePackFormatter<TypeBinaryExpression?>
{
    public static readonly TypeBinaryExpressionFormatter Instance = new();

    private const int FieldCount = 2;

    public void Serialize(ref MessagePackWriter writer, TypeBinaryExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Expression, options);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.TypeOperand, options);
    }

    public TypeBinaryExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var expr = len > 0 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var typeOp = len > 1 ? TypeInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new TypeBinaryExpression { Expression = expr!, TypeOperand = typeOp! };
    }
}
