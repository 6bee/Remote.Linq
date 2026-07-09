// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class BlockExpressionFormatter : IMessagePackFormatter<BlockExpression?>
{
    public static readonly BlockExpressionFormatter Instance = new();

    private const int FieldCount = 3;

    public void Serialize(ref MessagePackWriter writer, BlockExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.Type, options);
        FormatterHelpers.WriteParameterList(ref writer, value.Variables, options);
        FormatterHelpers.WriteExpressionList(ref writer, value.Expressions, options);
    }

    public BlockExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var type = len > 0 ? TypeInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        var vars = len > 1 ? FormatterHelpers.ReadParameterList(ref reader, options) : null;
        var exprs = len > 2 ? FormatterHelpers.ReadExpressionList(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new BlockExpression { Type = type, Variables = vars, Expressions = exprs };
    }
}
