// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

public sealed class CatchBlockFormatter : IMessagePackFormatter<CatchBlock?>
{
    public static readonly CatchBlockFormatter Instance = new();

    private const int FieldCount = 4;

    public void Serialize(ref MessagePackWriter writer, CatchBlock? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(FieldCount);
        ParameterExpressionFormatter.Instance.Serialize(ref writer, value.Variable, options);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Filter, options);
        ExpressionFormatter.Instance.Serialize(ref writer, value.Body, options);
        TypeInfoFormatter.Instance.Serialize(ref writer, value.Test, options);
    }

    public CatchBlock? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        var variable = len > 0 ? ParameterExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var filter = len > 1 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var body = len > 2 ? ExpressionFormatter.Instance.Deserialize(ref reader, options) : null;
        var test = len > 3 ? TypeInfoFormatter.Instance.Deserialize(ref reader, options) : null;
        for (var i = FieldCount; i < len; i++)
        {
            reader.Skip();
        }

        return new CatchBlock { Variable = variable, Filter = filter, Body = body!, Test = test! };
    }
}
