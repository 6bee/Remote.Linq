// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Remote.Linq.Expressions;

public sealed class ConditionalExpressionFormatter : IMessagePackFormatter<ConditionalExpression?>
{
    public static readonly ConditionalExpressionFormatter Instance = new();

    private const int FieldCount = 3;

    public void Serialize(ref MessagePackWriter writer, ConditionalExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Test, options);
        ExpressionFormatter.Instance.Serialize(ref writer, value.IfTrue, options);
        ExpressionFormatter.Instance.Serialize(ref writer, value.IfFalse, options);
    }

    public ConditionalExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var test = len > 0 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var ifTrue = len > 1 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var ifFalse = len > 2 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new ConditionalExpression { Test = test!, IfTrue = ifTrue!, IfFalse = ifFalse! };
    }
}
