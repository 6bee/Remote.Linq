// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Remote.Linq.Expressions;

public sealed class InvokeExpressionFormatter : IMessagePackFormatter<InvokeExpression?>
{
    public static readonly InvokeExpressionFormatter Instance = new();

    private const int FieldCount = 2;

    public void Serialize(ref MessagePackWriter writer, InvokeExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Expression, options);
        FormatterHelpers.WriteExpressionList(ref writer, value.Arguments, options);
    }

    public InvokeExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var expr = len > 0 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var args = len > 1 ? FormatterHelpers.ReadExpressionList(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new InvokeExpression { Expression = expr!, Arguments = args };
    }
}
