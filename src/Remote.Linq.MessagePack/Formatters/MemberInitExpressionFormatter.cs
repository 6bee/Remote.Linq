// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Remote.Linq.Expressions;

public sealed class MemberInitExpressionFormatter : IMessagePackFormatter<MemberInitExpression?>
{
    public static readonly MemberInitExpressionFormatter Instance = new();

    private const int FieldCount = 2;

    public void Serialize(ref MessagePackWriter writer, MemberInitExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        NewExpressionFormatter.Instance.Serialize(ref writer, value.NewExpression, options);
        FormatterHelpers.WriteMemberBindingList(ref writer, value.Bindings, options);
    }

    public MemberInitExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var newExpr = len > 0 ? NewExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var bindings = len > 1 ? FormatterHelpers.ReadMemberBindingList(ref reader, options) : [];
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new MemberInitExpression { NewExpression = newExpr!, Bindings = bindings };
    }
}
