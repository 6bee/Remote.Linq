// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.DynamicQuery;

public sealed class VariableQueryArgumentListFormatter : IMessagePackFormatter<VariableQueryArgumentList?>
{
    public static readonly VariableQueryArgumentListFormatter Instance = new();

    private const int FieldCount = 2;

    public void Serialize(ref MessagePackWriter writer, VariableQueryArgumentList? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.ElementType, options);
        if (value.Values is null)
        {
            writer.WriteNil();
        }
        else
        {
            writer.WriteArrayHeader(value.Values.Count);
            foreach (var item in value.Values)
            {
                AquaValueFormatter.Instance.Serialize(ref writer, item, options);
            }
        }
    }

    public VariableQueryArgumentList? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var elementType = len > 0 ? TypeInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        List<object?>? values = null;
        if (len > 1)
        {
            if (!reader.TryReadNil())
            {
                var count = (int)reader.ReadArrayHeader();
                values = new List<object?>(count);
                for (var i = 0; i < count; i++)
                {
                    values.Add(AquaValueFormatter.Instance.Deserialize(ref reader, options));
                }
            }
        }

        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new VariableQueryArgumentList { ElementType = elementType!, Values = values! };
    }
}
