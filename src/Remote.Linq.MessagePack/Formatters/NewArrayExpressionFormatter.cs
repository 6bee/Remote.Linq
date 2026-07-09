// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class NewArrayExpressionFormatter : IMessagePackFormatter<NewArrayExpression?>
{
    public static readonly NewArrayExpressionFormatter Instance = new();

    private const int FieldCount = 3;

    public void Serialize(ref MessagePackWriter writer, NewArrayExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        writer.Write((int)value.NewArrayType);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.Type, options);
        FormatterHelpers.WriteExpressionList(ref writer, value.Expressions, options);
    }

    public NewArrayExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var arrayType = len > 0 ? (NewArrayType)reader.ReadInt32() : default;
        var type = len > 1 ? TypeInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        var exprs = len > 2 ? FormatterHelpers.ReadExpressionList(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new NewArrayExpression { NewArrayType = arrayType, Type = type!, Expressions = exprs! };
    }
}
