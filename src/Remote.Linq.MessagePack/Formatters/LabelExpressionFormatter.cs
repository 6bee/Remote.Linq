// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Remote.Linq.Expressions;

public sealed class LabelExpressionFormatter : IMessagePackFormatter<LabelExpression?>
{
    public static readonly LabelExpressionFormatter Instance = new();

    private const int FieldCount = 2;

    public void Serialize(ref MessagePackWriter writer, LabelExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        LabelTargetFormatter.Instance.Serialize(ref writer, value.Target, options);
        ExpressionFormatter.Instance.Serialize(ref writer, value.DefaultValue, options);
    }

    public LabelExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var target = len > 0 ? LabelTargetFormatter.Instance.Deserialize(ref reader, options) : null;
        var defaultVal = len > 1 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new LabelExpression { Target = target!, DefaultValue = defaultVal };
    }
}
