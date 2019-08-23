// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using NodeType = System.Linq.Expressions.ExpressionType;

    [Serializable]
    public enum BinaryOperator
    {
        // Binary Arithmetic Operations
        Add = NodeType.Add,
        AddChecked = NodeType.AddChecked,
        Divide = NodeType.Divide,
        Modulo = NodeType.Modulo,
        Multiply = NodeType.Multiply,
        MultiplyChecked = NodeType.MultiplyChecked,
        Power = NodeType.Power,
        Subtract = NodeType.Subtract,
        SubtractChecked = NodeType.SubtractChecked,

        // Bitwise Operations
        And = NodeType.And,
        Or = NodeType.Or,
        ExclusiveOr = NodeType.ExclusiveOr,

        // Shift Operations
        LeftShift = NodeType.LeftShift,
        RightShift = NodeType.RightShift,

        // Conditional Boolean Operations
        AndAlso = NodeType.AndAlso,
        OrElse = NodeType.OrElse,

        // Comparison Operations
        Equal = NodeType.Equal,
        NotEqual = NodeType.NotEqual,
        GreaterThanOrEqual = NodeType.GreaterThanOrEqual,
        GreaterThan = NodeType.GreaterThan,
        LessThan = NodeType.LessThan,
        LessThanOrEqual = NodeType.LessThanOrEqual,

        // Coalescing Operations
        Coalesce = NodeType.Coalesce,

        // Array Indexing Operations
        ArrayIndex = NodeType.ArrayIndex,

        // Assignment Operations
        AddAssign = NodeType.AddAssign,
        AddAssignChecked = NodeType.AddAssignChecked,
        AndAssign = NodeType.AndAssign,
        Assign = NodeType.Assign,
        DivideAssign = NodeType.DivideAssign,
        ExclusiveOrAssign = NodeType.ExclusiveOrAssign,
        LeftShiftAssign = NodeType.LeftShiftAssign,
        ModuloAssign = NodeType.ModuloAssign,
        MultiplyAssign = NodeType.MultiplyAssign,
        MultiplyAssignChecked = NodeType.MultiplyAssignChecked,
        OrAssign = NodeType.OrAssign,
        PowerAssign = NodeType.PowerAssign,
        RightShiftAssign = NodeType.RightShiftAssign,
        SubtractAssign = NodeType.SubtractAssign,
        SubtractAssignChecked = NodeType.SubtractAssignChecked,
    }
}
