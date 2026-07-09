// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.DynamicQuery;

public sealed class SubstitutionValueFormatter : IMessagePackFormatter<SubstitutionValue?>
{
    public static readonly SubstitutionValueFormatter Instance = new();

    private const int FieldCount = 1;

    public void Serialize(ref MessagePackWriter writer, SubstitutionValue? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.Type, options);
    }

    public SubstitutionValue? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var type = len > 0 ? TypeInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new SubstitutionValue { Type = type! };
    }
}
