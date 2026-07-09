// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.DynamicQuery;

public sealed class ConstantQueryArgumentFormatter : IMessagePackFormatter<ConstantQueryArgument?>
{
    public static readonly ConstantQueryArgumentFormatter Instance = new();

    private const int FieldCount = 1;

    public void Serialize(ref MessagePackWriter writer, ConstantQueryArgument? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        DynamicObjectFormatter.Instance.Serialize(ref writer, value.Value, options);
    }

    public ConstantQueryArgument? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var val = len > 0 ? DynamicObjectFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new ConstantQueryArgument { Value = val! };
    }
}
