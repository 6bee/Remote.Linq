using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Aqua.TypeSystem;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class GotoExpression : Expression
    {
        public override ExpressionType NodeType => ExpressionType.Goto;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public GotoExpressionKind Kind { get; set; }
        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public Expression Expression { get; set; }
        [DataMember(Order = 3, IsRequired = true, EmitDefaultValue = false)]
        public LabelTarget Target { get; set; }
        [DataMember(Order = 4, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; }

        public GotoExpression()
        {
            
        }

        public GotoExpression(GotoExpressionKind kind, Expression expression, LabelTarget labelTarget, Type type):this(kind,expression,labelTarget, new TypeInfo(type, false, false))
        {
        }

        public GotoExpression(GotoExpressionKind kind, Expression expression, LabelTarget labelTarget, TypeInfo type)
        {
            Kind = kind;
            Expression = expression;
            Target = labelTarget;
            Type = type;
        }

        public override string ToString()
        {
            return $".Goto {Kind} {Target} {Expression}";
        }
    }
}
