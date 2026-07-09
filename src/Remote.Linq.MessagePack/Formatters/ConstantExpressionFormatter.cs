// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class ConstantExpressionFormatter : IMessagePackFormatter<ConstantExpression?>
{
    public static readonly ConstantExpressionFormatter Instance = new();

    private const int FieldCount = 2;

    public void Serialize(ref MessagePackWriter writer, ConstantExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.Type, options);
        ConstantValueFormatter.Instance.Serialize(ref writer, value.Value, options);
    }

    public ConstantExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var type = len > 0 ? TypeInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        var val = len > 1 ? ConstantValueFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new ConstantExpression { Type = type!, Value = val };
    }
}
