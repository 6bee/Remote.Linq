using System;
using System.Runtime.Serialization;
using Aqua.TypeSystem;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class LabelTarget
    {
        public LabelTarget()
        {
            
        }

        public LabelTarget(Type type, string name, int instanceId)
        {
            Type = new TypeInfo(type,false,false);
            Name = name;
            InstanceId = instanceId;
        }

        [DataMember(Order = 1,IsRequired = false,EmitDefaultValue = false)]
        public string Name { get; set; }
        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; }
        [DataMember(Order = 3, IsRequired = true, EmitDefaultValue = false)]
        public int InstanceId { get; set; }

        public override string ToString()
        {
            return $"#{Name??"Label"+InstanceId}";
        }
    }
}