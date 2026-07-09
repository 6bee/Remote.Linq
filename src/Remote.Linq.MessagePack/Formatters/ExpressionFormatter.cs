// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack.Formatters;

using Remote.Linq.Expressions;

/// <summary>Union formatter for <see cref="Expression"/> — writes <c>[NodeType_tag, concrete_array]</c>.</summary>
public sealed class ExpressionFormatter : IMessagePackFormatter<Expression?>
{
    public static readonly ExpressionFormatter Instance = new();

    public void Serialize(ref MessagePackWriter writer, Expression? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(2);
        writer.Write((int)value.NodeType);
        switch (value.NodeType)
        {
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
            case ExpressionType.Binary: BinaryExpressionFormatter.Instance.Serialize(ref writer, (BinaryExpression)value, options); break;
            case ExpressionType.Block: BlockExpressionFormatter.Instance.Serialize(ref writer, (BlockExpression)value, options); break;
            case ExpressionType.Conditional: ConditionalExpressionFormatter.Instance.Serialize(ref writer, (ConditionalExpression)value, options); break;
            case ExpressionType.Constant: ConstantExpressionFormatter.Instance.Serialize(ref writer, (ConstantExpression)value, options); break;
            case ExpressionType.Default: DefaultExpressionFormatter.Instance.Serialize(ref writer, (DefaultExpression)value, options); break;
            case ExpressionType.Goto: GotoExpressionFormatter.Instance.Serialize(ref writer, (GotoExpression)value, options); break;
            case ExpressionType.Invoke: InvokeExpressionFormatter.Instance.Serialize(ref writer, (InvokeExpression)value, options); break;
            case ExpressionType.Label: LabelExpressionFormatter.Instance.Serialize(ref writer, (LabelExpression)value, options); break;
            case ExpressionType.Lambda: LambdaExpressionFormatter.Instance.Serialize(ref writer, (LambdaExpression)value, options); break;
            case ExpressionType.ListInit: ListInitExpressionFormatter.Instance.Serialize(ref writer, (ListInitExpression)value, options); break;
            case ExpressionType.Loop: LoopExpressionFormatter.Instance.Serialize(ref writer, (LoopExpression)value, options); break;
            case ExpressionType.MemberAccess: MemberExpressionFormatter.Instance.Serialize(ref writer, (MemberExpression)value, options); break;
            case ExpressionType.MemberInit: MemberInitExpressionFormatter.Instance.Serialize(ref writer, (MemberInitExpression)value, options); break;
            case ExpressionType.Call: MethodCallExpressionFormatter.Instance.Serialize(ref writer, (MethodCallExpression)value, options); break;
            case ExpressionType.NewArray: NewArrayExpressionFormatter.Instance.Serialize(ref writer, (NewArrayExpression)value, options); break;
            case ExpressionType.New: NewExpressionFormatter.Instance.Serialize(ref writer, (NewExpression)value, options); break;
            case ExpressionType.Parameter: ParameterExpressionFormatter.Instance.Serialize(ref writer, (ParameterExpression)value, options); break;
            case ExpressionType.Switch: SwitchExpressionFormatter.Instance.Serialize(ref writer, (SwitchExpression)value, options); break;
            case ExpressionType.Try: TryExpressionFormatter.Instance.Serialize(ref writer, (TryExpression)value, options); break;
            case ExpressionType.TypeIs: TypeBinaryExpressionFormatter.Instance.Serialize(ref writer, (TypeBinaryExpression)value, options); break;
            case ExpressionType.Unary: UnaryExpressionFormatter.Instance.Serialize(ref writer, (UnaryExpression)value, options); break;
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
            default: throw new MessagePackSerializationException($"Unsupported expression type: {value.NodeType}");
        }
    }

    public Expression? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        var len = (int)reader.ReadArrayHeader();
        if (len < 1)
        {
            throw new MessagePackSerializationException("Empty expression array.");
        }

        var tag = (ExpressionType)reader.ReadInt32();
        Expression? result = tag switch
        {
#pragma warning disable SA1025 // Code should not contain multiple whitespace in a row
            ExpressionType.Binary => BinaryExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.Block => BlockExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.Conditional => ConditionalExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.Constant => ConstantExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.Default => DefaultExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.Goto => GotoExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.Invoke => InvokeExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.Label => LabelExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.Lambda => LambdaExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.ListInit => ListInitExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.Loop => LoopExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.MemberAccess => MemberExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.MemberInit => MemberInitExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.Call => MethodCallExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.NewArray => NewArrayExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.New => NewExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.Parameter => ParameterExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.Switch => SwitchExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.Try => TryExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.TypeIs => TypeBinaryExpressionFormatter.Instance.Deserialize(ref reader, options),
            ExpressionType.Unary => UnaryExpressionFormatter.Instance.Deserialize(ref reader, options),
#pragma warning restore SA1025 // Code should not contain multiple whitespace in a row
            _ => throw new MessagePackSerializationException($"Unknown expression type tag: {(int)tag}"),
        };
        for (var i = 2; i < len; i++)
        {
            reader.Skip();
        }

        return result;
    }
}
