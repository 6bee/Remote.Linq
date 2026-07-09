// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class GotoExpressionFormatter : IMessagePackFormatter<GotoExpression?>
{
    public static readonly GotoExpressionFormatter Instance = new();

    private const int FieldCount = 4;

    public void Serialize(ref MessagePackWriter writer, GotoExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        writer.Write((int)value.Kind);
        LabelTargetFormatter.Instance.Serialize(ref writer, value.Target, options);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.Type, options);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Value, options);
    }

    public GotoExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var kind = len > 0 ? (GotoExpressionKind)reader.ReadInt32() : default;
        var target = len > 1 ? LabelTargetFormatter.Instance.Deserialize(ref reader, options) : null;
        var type = len > 2 ? TypeInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        var val = len > 3 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new GotoExpression { Kind = kind, Target = target!, Type = type, Value = val };
    }
}
