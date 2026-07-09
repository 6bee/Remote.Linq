// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Aqua.MessagePack.Formatters;
using Remote.Linq.Expressions;

internal static class FormatterHelpers
{
    internal static void WriteExpressionList(ref MessagePackWriter writer, List<Expression>? list, MessagePackSerializerOptions options)
    {
        if (list is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(list.Count);
        foreach (var item in list)
        {
            ExpressionFormatter.Instance.Serialize(ref writer, item, options);
        }
    }

    internal static List<Expression>? ReadExpressionList(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var count = (int)reader.ReadArrayHeader();
        var list = new List<Expression>(count);
        for (var i = 0; i < count; i++)
        {
            list.Add(ExpressionFormatter.Instance.Deserialize(ref reader, options)!);
        }

        return list;
    }

    internal static void WriteParameterList(ref MessagePackWriter writer, List<ParameterExpression>? list, MessagePackSerializerOptions options)
    {
        if (list is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(list.Count);
        foreach (var item in list)
        {
            ParameterExpressionFormatter.Instance.Serialize(ref writer, item, options);
        }
    }

    internal static List<ParameterExpression>? ReadParameterList(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var count = (int)reader.ReadArrayHeader();
        var list = new List<ParameterExpression>(count);
        for (var i = 0; i < count; i++)
        {
            list.Add(ParameterExpressionFormatter.Instance.Deserialize(ref reader, options)!);
        }

        return list;
    }

    internal static void WriteMemberInfoList(ref MessagePackWriter writer, List<Aqua.TypeSystem.MemberInfo>? list, MessagePackSerializerOptions options)
    {
        if (list is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(list.Count);
        foreach (var item in list)
        {
            MemberInfoFormatter.Instance.Serialize(ref writer, item, options);
        }
    }

    internal static List<Aqua.TypeSystem.MemberInfo>? ReadMemberInfoList(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var count = (int)reader.ReadArrayHeader();
        var list = new List<Aqua.TypeSystem.MemberInfo>(count);
        for (var i = 0; i < count; i++)
        {
            list.Add(MemberInfoFormatter.Instance.Deserialize(ref reader, options)!);
        }

        return list;
    }

    internal static void WriteElementInitList(ref MessagePackWriter writer, List<ElementInit> list, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(list.Count);
        foreach (var item in list)
        {
            ElementInitFormatter.Instance.Serialize(ref writer, item, options);
        }
    }

    internal static List<ElementInit> ReadElementInitList(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var count = (int)reader.ReadArrayHeader();
        var list = new List<ElementInit>(count);
        for (var i = 0; i < count; i++)
        {
            list.Add(ElementInitFormatter.Instance.Deserialize(ref reader, options)!);
        }

        return list;
    }

    internal static void WriteMemberBindingList(ref MessagePackWriter writer, List<MemberBinding> list, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(list.Count);
        foreach (var item in list)
        {
            MemberBindingFormatter.Instance.Serialize(ref writer, item, options);
        }
    }

    internal static List<MemberBinding> ReadMemberBindingList(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var count = (int)reader.ReadArrayHeader();
        var list = new List<MemberBinding>(count);
        for (var i = 0; i < count; i++)
        {
            list.Add(MemberBindingFormatter.Instance.Deserialize(ref reader, options)!);
        }

        return list;
    }

    internal static void WriteSwitchCaseList(ref MessagePackWriter writer, List<SwitchCase>? list, MessagePackSerializerOptions options)
    {
        if (list is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(list.Count);
        foreach (var item in list)
        {
            SwitchCaseFormatter.Instance.Serialize(ref writer, item, options);
        }
    }

    internal static List<SwitchCase>? ReadSwitchCaseList(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var count = (int)reader.ReadArrayHeader();
        var list = new List<SwitchCase>(count);
        for (var i = 0; i < count; i++)
        {
            list.Add(SwitchCaseFormatter.Instance.Deserialize(ref reader, options)!);
        }

        return list;
    }

    internal static void WriteCatchBlockList(ref MessagePackWriter writer, List<CatchBlock>? list, MessagePackSerializerOptions options)
    {
        if (list is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(list.Count);
        foreach (var item in list)
        {
            CatchBlockFormatter.Instance.Serialize(ref writer, item, options);
        }
    }

    internal static List<CatchBlock>? ReadCatchBlockList(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var count = (int)reader.ReadArrayHeader();
        var list = new List<CatchBlock>(count);
        for (var i = 0; i < count; i++)
        {
            list.Add(CatchBlockFormatter.Instance.Deserialize(ref reader, options)!);
        }

        return list;
    }
}
