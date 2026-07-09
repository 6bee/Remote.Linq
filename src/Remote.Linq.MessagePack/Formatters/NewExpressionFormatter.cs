// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class NewExpressionFormatter : IMessagePackFormatter<NewExpression?>
{
    public static readonly NewExpressionFormatter Instance = new();

    private const int FieldCount = 4;

    public void Serialize(ref MessagePackWriter writer, NewExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        ConstructorInfoFormatter.Instance.Serialize(ref writer, value.Constructor, options);
        FormatterHelpers.WriteExpressionList(ref writer, value.Arguments, options);
        FormatterHelpers.WriteMemberInfoList(ref writer, value.Members, options);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.Type, options);
    }

    public NewExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var ctor = len > 0 ? ConstructorInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        var args = len > 1 ? FormatterHelpers.ReadExpressionList(ref reader, options) : null;
        var members = len > 2 ? FormatterHelpers.ReadMemberInfoList(ref reader, options) : null;
        var type = len > 3 ? TypeInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new NewExpression { Constructor = ctor, Arguments = args, Members = members, Type = type! };
    }
}
