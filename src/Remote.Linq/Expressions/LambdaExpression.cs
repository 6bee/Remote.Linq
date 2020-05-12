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
    public sealed class LambdaExpression : Expression
    {
        public LambdaExpression()
        {
        }

        public LambdaExpression(Expression expression, IEnumerable<ParameterExpression> parameters)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            Parameters = parameters?.ToList() ?? throw new ArgumentNullException(nameof(parameters));
        }

        public LambdaExpression(TypeInfo? type, Expression expression, IEnumerable<ParameterExpression> parameters)
            : this(expression, parameters)
        {
            Type = type;
        }

        public LambdaExpression(Type? type, Expression expression, IEnumerable<ParameterExpression> parameters)
            : this(type is null ? null : new TypeInfo(type, false, false), expression, parameters)
        {
        }

        public override ExpressionType NodeType => ExpressionType.Lambda;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public Expression Expression { get; set; } = null!;

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public List<ParameterExpression> Parameters { get; set; } = null!;

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public TypeInfo? Type { get; set; }

        public override string ToString()
        {
            var parameters = Parameters;
            var open = parameters?.Count == 1 ? null : "(";
            var close = parameters?.Count == 1 ? null : ")";
            var parameterList = parameters.StringJoin("; ");
            return $"{open}{parameterList}{close} => {Expression}";
        }
    }
}
