// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class ElementInitFormatter : IMessagePackFormatter<ElementInit?>
{
    public static readonly ElementInitFormatter Instance = new();

    private const int FieldCount = 2;

    public void Serialize(ref MessagePackWriter writer, ElementInit? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        MethodInfoFormatter.Instance.Serialize(ref writer, value.AddMethod, options);
        FormatterHelpers.WriteExpressionList(ref writer, value.Arguments, options);
    }

    public ElementInit? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var addMethod = len > 0 ? MethodInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        var args = len > 1 ? FormatterHelpers.ReadExpressionList(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new ElementInit { AddMethod = addMethod!, Arguments = args! };
    }
}
