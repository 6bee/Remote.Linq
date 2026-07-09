// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class BinaryExpressionFormatter : IMessagePackFormatter<BinaryExpression?>
{
    public static readonly BinaryExpressionFormatter Instance = new();

    private const int FieldCount = 6;

    public void Serialize(ref MessagePackWriter writer, BinaryExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        writer.Write((int)value.BinaryOperator);
        ExpressionFormatter.Instance.Serialize(ref writer, value.LeftOperand, options);
        ExpressionFormatter.Instance.Serialize(ref writer, value.RightOperand, options);
        writer.Write(value.IsLiftedToNull);
        MethodInfoFormatter.Instance.Serialize(ref writer, value.Method, options);
        LambdaExpressionFormatter.Instance.Serialize(ref writer, value.Conversion, options);
    }

    public BinaryExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var op = len > 0 ? (BinaryOperator)reader.ReadInt32() : default;
        var left = len > 1 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var right = len > 2 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var isLifted = len > 3 && reader.ReadBoolean();
        var method = len > 4 ? MethodInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        var conversion = len > 5 ? LambdaExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new BinaryExpression
        {
            BinaryOperator = op,
            LeftOperand = left!,
            RightOperand = right!,
            IsLiftedToNull = isLifted,
            Method = method,
            Conversion = conversion,
        };
    }
}
