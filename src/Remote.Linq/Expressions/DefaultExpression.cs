using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Aqua.TypeSystem;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class DefaultExpression : Expression
    {
        public override ExpressionType NodeType => ExpressionType.Default;
        
        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; }

        public DefaultExpression()
        {
            
        }

        public DefaultExpression(Type type)
        {
            Type = new TypeInfo(type,false,false);
        }

        public override string ToString()
        {
            return $".Default {Type}";
        }
    }
}
