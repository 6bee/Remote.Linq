// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class MemberExpressionFormatter : IMessagePackFormatter<MemberExpression?>
{
    public static readonly MemberExpressionFormatter Instance = new();

    private const int FieldCount = 2;

    public void Serialize(ref MessagePackWriter writer, MemberExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Expression, options);
        MemberInfoFormatter.Instance.Serialize(ref writer, value.Member, options);
    }

    public MemberExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var expr = len > 0 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var member = len > 1 ? MemberInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new MemberExpression { Expression = expr, Member = member! };
    }
}
