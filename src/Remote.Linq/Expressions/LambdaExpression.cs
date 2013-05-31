// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class LambdaExpression : Expression
    {
        internal LambdaExpression(Expression expression, IEnumerable<ParameterExpression> parameters)
        {
            Expression = expression;
            Parameters = parameters.ToArray();
        }

        public override ExpressionType NodeType { get { return ExpressionType.Lambda; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Expression Expression { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public IEnumerable<ParameterExpression> Parameters { get; private set; }

        public override string ToString()
        {
            return string.Format("({0}) => {1}", string.Join(",", Parameters.Select(p => p.ToString()).ToArray()), Expression);
        }
    }
}
