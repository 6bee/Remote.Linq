// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class BlockExpression : Expression
    {
        public BlockExpression()
        {
        }

        public BlockExpression(Type type, IEnumerable<ParameterExpression> variables, IEnumerable<Expression> expressions)
            : this(type is null ? null : new TypeInfo(type, false, false), variables, expressions)
        {
        }

        public BlockExpression(TypeInfo type, IEnumerable<ParameterExpression> variables, IEnumerable<Expression> expressions)
        {
            Type = type;
            Variables = variables?.Any() ?? false ? variables.ToList() : null;
            Expressions = expressions?.Any() ?? false ? expressions.ToList() : null;
        }

        public override ExpressionType NodeType => ExpressionType.Block;

        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public List<ParameterExpression> Variables { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public List<Expression> Expressions { get; set; }

        public override string ToString()
        {
            var expressions = Expressions;
            return string.Format(
                ".Block {{0}}",
                expressions is null ? null : string.Join("; ", expressions));
        }
    }
}
