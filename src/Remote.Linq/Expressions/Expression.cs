// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
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
    public abstract class Expression
    {
        public abstract ExpressionType NodeType { get; }
    }
}
