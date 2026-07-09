// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.DynamicQuery;

/// <summary>Tagged-union formatter for <c>ConstantExpression.Value</c> — not registered in resolver.</summary>
/// <remarks>Tags: 0=Aqua fallback, 1=ConstantQueryArgument, 2=VariableQueryArgument,
/// 3=VariableQueryArgumentList, 4=SubstitutionValue, 5=QueryableResourceDescriptor.</remarks>
public sealed class ConstantValueFormatter : IMessagePackFormatter<object?>
{
    public static readonly ConstantValueFormatter Instance = new();

    public void Serialize(ref MessagePackWriter writer, object? value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(2);
        switch (value)
        {
            case ConstantQueryArgument cqa:
                writer.Write((byte)1);
                ConstantQueryArgumentFormatter.Instance.Serialize(ref writer, cqa, options);
                break;
            case VariableQueryArgument vqa:
                writer.Write((byte)2);
                VariableQueryArgumentFormatter.Instance.Serialize(ref writer, vqa, options);
                break;
            case VariableQueryArgumentList vqal:
                writer.Write((byte)3);
                VariableQueryArgumentListFormatter.Instance.Serialize(ref writer, vqal, options);
                break;
            case SubstitutionValue sv:
                writer.Write((byte)4);
                SubstitutionValueFormatter.Instance.Serialize(ref writer, sv, options);
                break;
            case QueryableResourceDescriptor qrd:
                writer.Write((byte)5);
                QueryableResourceDescriptorFormatter.Instance.Serialize(ref writer, qrd, options);
                break;
            default:
                writer.Write((byte)0);
                AquaValueFormatter.Instance.Serialize(ref writer, value, options);
                break;
        }
    }

    public object? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        if (len < 1)
        {
            return null;
        }

        var tag = reader.ReadByte();
        object? result = tag switch
        {
            0 => AquaValueFormatter.Instance.Deserialize(ref reader, options),
            1 => ConstantQueryArgumentFormatter.Instance.Deserialize(ref reader, options),
            2 => VariableQueryArgumentFormatter.Instance.Deserialize(ref reader, options),
            3 => VariableQueryArgumentListFormatter.Instance.Deserialize(ref reader, options),
            4 => SubstitutionValueFormatter.Instance.Deserialize(ref reader, options),
            5 => QueryableResourceDescriptorFormatter.Instance.Deserialize(ref reader, options),
            _ => throw new MessagePackSerializationException($"Unknown constant value tag: {tag}"),
        };
        for (var i = 2; i < len; i++)
        {
            reader.Skip();
        }

        return result;
    }
}
