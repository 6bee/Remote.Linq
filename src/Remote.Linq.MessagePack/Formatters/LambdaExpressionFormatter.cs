// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class LambdaExpressionFormatter : IMessagePackFormatter<LambdaExpression?>
{
    public static readonly LambdaExpressionFormatter Instance = new();

    private const int FieldCount = 3;

    public void Serialize(ref MessagePackWriter writer, LambdaExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Expression, options);
        FormatterHelpers.WriteParameterList(ref writer, value.Parameters, options);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.Type, options);
    }

    public LambdaExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var expr = len > 0 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var parameters = len > 1 ? FormatterHelpers.ReadParameterList(ref reader, options) : null;
        var type = len > 2 ? TypeInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new LambdaExpression { Expression = expr!, Parameters = parameters, Type = type };
    }
}
