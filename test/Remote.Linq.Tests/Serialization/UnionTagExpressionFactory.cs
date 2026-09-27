// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using Aqua.TypeSystem;
using RemoteLinq = Remote.Linq.Expressions;

/// <summary>
/// Builds representative remote expression payloads for all <see cref="RemoteLinq.ExpressionType"/> tags.
/// </summary>
public static class UnionTagExpressionFactory
{
    /// <summary>The <c>int.MaxValue</c> member as remote <see cref="MemberInfo"/>.</summary>
    public static readonly MemberInfo IntMaxValueMember = MemberInfo.Create(typeof(int).GetField(nameof(int.MaxValue)));

    /// <summary>
    /// Builds a simple, self-contained remote expression for the given node type tag.
    /// </summary>
    /// <param name="nodeType">The expression node type tag.</param>
    /// <returns>A remote expression with the given node type.</returns>
    public static RemoteLinq.Expression Build(RemoteLinq.ExpressionType nodeType)
        => nodeType switch
        {
            RemoteLinq.ExpressionType.Binary => new RemoteLinq.BinaryExpression(RemoteLinq.BinaryOperator.Add, Constant(1), Constant(2)),
            RemoteLinq.ExpressionType.Block => new RemoteLinq.BlockExpression(typeof(int), null, [Constant(1)]),
            RemoteLinq.ExpressionType.Conditional => new RemoteLinq.ConditionalExpression(new RemoteLinq.BinaryExpression(RemoteLinq.BinaryOperator.GreaterThan, Constant(1), Constant(0)), Constant(1), Constant(0)),
            RemoteLinq.ExpressionType.Constant => Constant(42),
            RemoteLinq.ExpressionType.Default => new RemoteLinq.DefaultExpression(typeof(int)),
            RemoteLinq.ExpressionType.Goto => new RemoteLinq.GotoExpression(RemoteLinq.GotoExpressionKind.Break, Label("goto"), typeof(int), null),
            RemoteLinq.ExpressionType.Invoke => new RemoteLinq.InvokeExpression(new RemoteLinq.LambdaExpression(Constant(0), null), [Constant(0)]),
            RemoteLinq.ExpressionType.Label => new RemoteLinq.LabelExpression(Label("label"), Constant(0)),
            RemoteLinq.ExpressionType.Lambda => new RemoteLinq.LambdaExpression(new RemoteLinq.ParameterExpression(typeof(int), "x", 0), null),
            RemoteLinq.ExpressionType.ListInit => new RemoteLinq.ListInitExpression(new RemoteLinq.NewExpression(typeof(List<int>).GetConstructors()[0]), [new RemoteLinq.ElementInit(typeof(List<int>).GetMethod(nameof(List<int>.Add)), [Constant(1)])]),
            RemoteLinq.ExpressionType.Loop => new RemoteLinq.LoopExpression(Constant(0), Label("break"), Label("continue")),
            RemoteLinq.ExpressionType.MemberAccess => new RemoteLinq.MemberExpression(null, IntMaxValueMember),
            RemoteLinq.ExpressionType.MemberInit => new RemoteLinq.MemberInitExpression(new RemoteLinq.NewExpression(typeof(int)), [new RemoteLinq.MemberAssignment(IntMaxValueMember, Constant(0))]),
            RemoteLinq.ExpressionType.Call => new RemoteLinq.MethodCallExpression(null, typeof(object).GetMethod(nameof(object.ToString)), []),
            RemoteLinq.ExpressionType.New => new RemoteLinq.NewExpression(typeof(int)),
            RemoteLinq.ExpressionType.NewArray => new RemoteLinq.NewArrayExpression(RemoteLinq.NewArrayType.NewArrayBounds, typeof(int), [Constant(1), Constant(2)]),
            RemoteLinq.ExpressionType.Parameter => new RemoteLinq.ParameterExpression(typeof(int), "x", 0),
            RemoteLinq.ExpressionType.Switch => new RemoteLinq.SwitchExpression(Constant(1), (System.Reflection.MethodInfo)null, Constant(-1), [new RemoteLinq.SwitchCase(Constant(1), [Constant(1)])]),
            RemoteLinq.ExpressionType.Try => new RemoteLinq.TryExpression(typeof(Exception), Constant(0), null, null, [new RemoteLinq.CatchBlock(typeof(InvalidOperationException), null, Constant(0), null)]),
            RemoteLinq.ExpressionType.TypeIs => new RemoteLinq.TypeBinaryExpression(Constant("a", typeof(string)), typeof(string)),
            RemoteLinq.ExpressionType.Unary => new RemoteLinq.UnaryExpression(RemoteLinq.UnaryOperator.Negate, Constant(42), typeof(int), null),
            _ => throw new ArgumentOutOfRangeException(nameof(nodeType)),
        };

    private static RemoteLinq.ConstantExpression Constant(object value, Type type = null) => new(value, type);

    private static RemoteLinq.LabelTarget Label(string name) => new(name, typeof(int));
}
