// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class MemberAssignmentFormatter : IMessagePackFormatter<MemberAssignment?>
{
    public static readonly MemberAssignmentFormatter Instance = new();

    private const int FieldCount = 2;

    public void Serialize(ref MessagePackWriter writer, MemberAssignment? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        MemberInfoFormatter.Instance.Serialize(ref writer, value.Member, options);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Expression, options);
    }

    public MemberAssignment? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var member = len > 0 ? MemberInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        var expr = len > 1 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new MemberAssignment { Member = member!, Expression = expr! };
    }
}
