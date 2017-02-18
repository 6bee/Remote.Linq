// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua;
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

        internal LambdaExpression(Expression expression, IEnumerable<ParameterExpression> parameters)
        {
            Expression = expression;
            Parameters = parameters.ToList();
        }

        public override ExpressionType NodeType => ExpressionType.Lambda;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public Expression Expression { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public List<ParameterExpression> Parameters { get; set; }

        public override string ToString()
        {
            var parameters = Parameters;
            var parameterString = parameters == null ? null : string.Join(",", parameters.Select(p => p.ToString()).ToArray());
            return $"{(parameters.Count == 1 ? null : "(")}{parameterString}{(parameters.Count == 1 ? null : ")")} => {Expression}";
        }
    }
}
