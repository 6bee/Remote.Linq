// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class MemberListBindingFormatter : IMessagePackFormatter<MemberListBinding?>
{
    public static readonly MemberListBindingFormatter Instance = new();

    private const int FieldCount = 2;

    public void Serialize(ref MessagePackWriter writer, MemberListBinding? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        MemberInfoFormatter.Instance.Serialize(ref writer, value.Member, options);
        FormatterHelpers.WriteElementInitList(ref writer, value.Initializers, options);
    }

    public MemberListBinding? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var member = len > 0 ? MemberInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        var inits = len > 1 ? FormatterHelpers.ReadElementInitList(ref reader, options) : [];
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new MemberListBinding { Member = member!, Initializers = inits };
    }
}
