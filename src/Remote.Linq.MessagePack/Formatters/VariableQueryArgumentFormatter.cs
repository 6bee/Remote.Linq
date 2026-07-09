// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.DynamicQuery;

public sealed class VariableQueryArgumentFormatter : IMessagePackFormatter<VariableQueryArgument?>
{
    public static readonly VariableQueryArgumentFormatter Instance = new();

    private const int FieldCount = 2;

    public void Serialize(ref MessagePackWriter writer, VariableQueryArgument? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.Type, options);
        AquaValueFormatter.Instance.Serialize(ref writer, value.Value, options);
    }

    public VariableQueryArgument? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var type = len > 0 ? TypeInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        var val = len > 1 ? AquaValueFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new VariableQueryArgument { Type = type!, Value = val };
    }
}
