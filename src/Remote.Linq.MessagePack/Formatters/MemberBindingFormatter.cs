// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Remote.Linq.Expressions;

/// <summary>Union formatter for <see cref="MemberBinding"/> — writes <c>[BindingType_tag, concrete_array]</c>.</summary>
public sealed class MemberBindingFormatter : IMessagePackFormatter<MemberBinding?>
{
    public static readonly MemberBindingFormatter Instance = new();

    public void Serialize(ref MessagePackWriter writer, MemberBinding? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(2);
        writer.Write((int)value.BindingType);
        switch (value.BindingType)
        {
            case MemberBindingType.Assignment:
                MemberAssignmentFormatter.Instance.Serialize(ref writer, (MemberAssignment)value, options);
                break;
            case MemberBindingType.MemberBinding:
                MemberMemberBindingFormatter.Instance.Serialize(ref writer, (MemberMemberBinding)value, options);
                break;
            case MemberBindingType.ListBinding:
                MemberListBindingFormatter.Instance.Serialize(ref writer, (MemberListBinding)value, options);
                break;
            default:
                throw new MessagePackSerializationException($"Unsupported MemberBindingType: {value.BindingType}");
        }
    }

    public MemberBinding? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        if (len < 1)
        {
            throw new MessagePackSerializationException("Empty member binding array.");
        }

        var tag = (MemberBindingType)reader.ReadInt32();
        MemberBinding? result = tag switch
        {
            MemberBindingType.Assignment => MemberAssignmentFormatter.Instance.Deserialize(ref reader, options),
            MemberBindingType.MemberBinding => MemberMemberBindingFormatter.Instance.Deserialize(ref reader, options),
            MemberBindingType.ListBinding => MemberListBindingFormatter.Instance.Deserialize(ref reader, options),
            _ => throw new MessagePackSerializationException($"Unknown MemberBindingType tag: {(int)tag}"),
        };
        for (var i = 2; i < len; i++)
        {
            reader.Skip();
        }

        return result;
    }
}
