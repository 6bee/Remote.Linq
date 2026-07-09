// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Remote.Linq.Expressions;

public sealed class LoopExpressionFormatter : IMessagePackFormatter<LoopExpression?>
{
    public static readonly LoopExpressionFormatter Instance = new();

    private const int FieldCount = 3;

    public void Serialize(ref MessagePackWriter writer, LoopExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Body, options);
        LabelTargetFormatter.Instance.Serialize(ref writer, value.BreakLabel, options);
        LabelTargetFormatter.Instance.Serialize(ref writer, value.ContinueLabel, options);
    }

    public LoopExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var body = len > 0 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var breakLabel = len > 1 ? LabelTargetFormatter.Instance.Deserialize(ref reader, options) : null;
        var continueLabel = len > 2 ? LabelTargetFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new LoopExpression { Body = body!, BreakLabel = breakLabel, ContinueLabel = continueLabel };
    }
}
