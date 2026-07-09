// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class SwitchExpressionFormatter : IMessagePackFormatter<SwitchExpression?>
{
    public static readonly SwitchExpressionFormatter Instance = new();

    private const int FieldCount = 4;

    public void Serialize(ref MessagePackWriter writer, SwitchExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        ExpressionFormatter.Instance.Serialize(ref writer, value.SwitchValue, options);
        MethodInfoFormatter.Instance.Serialize(ref writer, value.Comparison, options);
        ExpressionFormatter.Instance.Serialize(ref writer, value.DefaultExpression, options);
        FormatterHelpers.WriteSwitchCaseList(ref writer, value.Cases, options);
    }

    public SwitchExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var switchVal = len > 0 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var comparison = len > 1 ? MethodInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        var defaultExpr = len > 2 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var cases = len > 3 ? FormatterHelpers.ReadSwitchCaseList(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new SwitchExpression
        {
            SwitchValue = switchVal!,
            Comparison = comparison,
            DefaultExpression = defaultExpr,
            Cases = cases,
        };
    }
}
