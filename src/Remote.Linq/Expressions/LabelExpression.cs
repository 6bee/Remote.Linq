using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class LabelExpression : Expression
    {
        [DataMember(Order = 1,IsRequired = true,EmitDefaultValue = false)]
        public LabelTarget LabelTarget { get; set; }
        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public Expression Expression { get; set; }

        public override ExpressionType NodeType => ExpressionType.Label;
        
        public LabelExpression()
        {
            
        }

        public LabelExpression(LabelTarget labelTarget, Expression expression)
        {
            LabelTarget = labelTarget;
            Expression = expression;
        }

        public override string ToString()
        {
            return $".Label {LabelTarget} {Expression}";
        }
    }
}
