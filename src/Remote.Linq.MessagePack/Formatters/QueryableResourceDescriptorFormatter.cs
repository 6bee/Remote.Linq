// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.DynamicQuery;

public sealed class QueryableResourceDescriptorFormatter : IMessagePackFormatter<QueryableResourceDescriptor?>
{
    public static readonly QueryableResourceDescriptorFormatter Instance = new();

    private const int FieldCount = 1;

    public void Serialize(ref MessagePackWriter writer, QueryableResourceDescriptor? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.Type, options);
    }

    public QueryableResourceDescriptor? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
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

        return new QueryableResourceDescriptor { Type = type! };
    }
}
