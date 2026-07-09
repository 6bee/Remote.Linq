// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class MethodCallExpressionFormatter : IMessagePackFormatter<MethodCallExpression?>
{
    public static readonly MethodCallExpressionFormatter Instance = new();

    private const int FieldCount = 3;

    public void Serialize(ref MessagePackWriter writer, MethodCallExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Instance, options);
        MethodInfoFormatter.Instance.Serialize(ref writer, value.Method, options);
        FormatterHelpers.WriteExpressionList(ref writer, value.Arguments, options);
    }

    public MethodCallExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var instance = len > 0 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var method = len > 1 ? MethodInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        var args = len > 2 ? FormatterHelpers.ReadExpressionList(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new MethodCallExpression { Instance = instance, Method = method!, Arguments = args };
    }
}
