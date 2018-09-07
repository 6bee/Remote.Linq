// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class TryExpression : Expression
    {
        public TryExpression()
        {
        }

        public TryExpression(Type type, Expression body, Expression fault, Expression @finally, List<CatchBlock> handlers)
            : this(type is null ? null : new TypeInfo(type, false, false), body, fault, @finally, handlers)
        {
        }

        public TryExpression(TypeInfo type, Expression body, Expression fault, Expression @finally, List<CatchBlock> handlers)
        {
            Type = type;
            Body = body;
            Handlers = handlers;
            Finally = @finally;
            Fault = fault;
        }

        public override ExpressionType NodeType => ExpressionType.Try;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public Expression Body { get; set; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public List<CatchBlock> Handlers { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public Expression Finally { get; set; }

        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public Expression Fault { get; set; }

        [DataMember(Order = 5, IsRequired = false, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; }

        public override string ToString()
            => $"try({Type}) {{{Body}}}" +
                (Handlers is null ? string.Empty : " " + string.Join("; ", Handlers)) +
                (Finally is null ? string.Empty : $" finally{{{Finally}}}") +
                (Fault is null ? string.Empty : $" faulted{{{Fault}}}");
    }
}
