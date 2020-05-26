// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [Serializable]
    [DataContract]
    [KnownType(typeof(BinaryExpression)), XmlInclude(typeof(BinaryExpression))]
    [KnownType(typeof(BlockExpression)), XmlInclude(typeof(BlockExpression))]
    [KnownType(typeof(ConditionalExpression)), XmlInclude(typeof(ConditionalExpression))]
    [KnownType(typeof(ConstantExpression)), XmlInclude(typeof(ConstantExpression))]
    [KnownType(typeof(DefaultExpression)), XmlInclude(typeof(DefaultExpression))]
    [KnownType(typeof(GotoExpression)), XmlInclude(typeof(GotoExpression))]
    [KnownType(typeof(InvokeExpression)), XmlInclude(typeof(InvokeExpression))]
    [KnownType(typeof(LabelExpression)), XmlInclude(typeof(LabelExpression))]
    [KnownType(typeof(LambdaExpression)), XmlInclude(typeof(LambdaExpression))]
    [KnownType(typeof(ListInitExpression)), XmlInclude(typeof(ListInitExpression))]
    [KnownType(typeof(LoopExpression)), XmlInclude(typeof(LoopExpression))]
    [KnownType(typeof(MemberExpression)), XmlInclude(typeof(MemberExpression))]
    [KnownType(typeof(MemberInitExpression)), XmlInclude(typeof(MemberInitExpression))]
    [KnownType(typeof(MethodCallExpression)), XmlInclude(typeof(MethodCallExpression))]
    [KnownType(typeof(NewExpression)), XmlInclude(typeof(NewExpression))]
    [KnownType(typeof(NewArrayExpression)), XmlInclude(typeof(NewArrayExpression))]
    [KnownType(typeof(ParameterExpression)), XmlInclude(typeof(ParameterExpression))]
    [KnownType(typeof(SwitchExpression)), XmlInclude(typeof(SwitchExpression))]
    [KnownType(typeof(TryExpression)), XmlInclude(typeof(TryExpression))]
    [KnownType(typeof(TypeBinaryExpression)), XmlInclude(typeof(TypeBinaryExpression))]
    [KnownType(typeof(UnaryExpression)), XmlInclude(typeof(UnaryExpression))]
    [KnownType(typeof(TypeInfo)), XmlInclude(typeof(TypeInfo))]
    [KnownType(typeof(ConstructorInfo)), XmlInclude(typeof(ConstructorInfo))]
    [KnownType(typeof(FieldInfo)), XmlInclude(typeof(FieldInfo))]
    [KnownType(typeof(PropertyInfo)), XmlInclude(typeof(PropertyInfo))]
    [KnownType(typeof(MethodInfo)), XmlInclude(typeof(MethodInfo))]
    [DebuggerDisplay("{DebugFormatter}")]
    public abstract class Expression
    {
        [Unmapped]
        public abstract ExpressionType NodeType { get; }

        [Unmapped]
        internal string DebugView => DebugFormatter.ToString(0);

        // TODO: make DebugFormatter property abstract and have every expression implement its own formatter
        [Unmapped]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected internal virtual ExpressionDebugFormatter DebugFormatter => new ExpressionDebugFormatter(this);

        protected internal class ExpressionDebugFormatter
        {
            public ExpressionDebugFormatter(Expression expression)
            {
                Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            }

            protected Expression Expression { get; }

            public override string ToString() => Expression.ToString();

            // TODO: add indentation and line break formtting
            protected internal string ToString(int level) => ToString();

            protected static string Format(Type? t) => Format(t.AsTypeInfo());

            protected static string Format(TypeInfo? typeInfo) => $"{typeInfo?.NameWithoutNameSpace}{typeInfo?.GetGenericArgumentsString()}";
        }

        protected internal abstract class ExpressionDebugFormatter<T> : ExpressionDebugFormatter
            where T : Expression
        {
            protected ExpressionDebugFormatter(T expression)
                : base(expression)
            {
            }

            public new T Expression => (T)base.Expression;
        }
    }
}
