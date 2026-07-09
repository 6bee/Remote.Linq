// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class TryExpressionFormatter : IMessagePackFormatter<TryExpression?>
{
    public static readonly TryExpressionFormatter Instance = new();

    private const int FieldCount = 5;

    public void Serialize(ref MessagePackWriter writer, TryExpression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Body, options);
        FormatterHelpers.WriteCatchBlockList(ref writer, value.Handlers, options);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Finally, options);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Fault, options);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.Type, options);
    }

    public TryExpression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var body = len > 0 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var handlers = len > 1 ? FormatterHelpers.ReadCatchBlockList(ref reader, options) : null;
        var @finally = len > 2 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var fault = len > 3 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var type = len > 4 ? TypeInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new TryExpression
        {
            Body = body!,
            Handlers = handlers,
            Finally = @finally,
            Fault = fault,
            Type = type!,
        };
    }
}
