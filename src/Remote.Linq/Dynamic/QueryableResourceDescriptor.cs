// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Dynamic
{
    [DataContract]
    public sealed class QueryableResourceDescriptor
    {
        public QueryableResourceDescriptor(Type type)
        {
            Type = new TypeInfo(type);
        }

        [DataMember(Name = "Type", IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo Type { get; private set; }
//#if SILVERLIGHT
//        internal TypeInfo Type { get; private set; }
//#else
//        private TypeInfo Type { get; set; }
//#endif

        //public Type Type
        //{
        //    get
        //    {
        //        if (ReferenceEquals(null, _type))
        //        {
        //            _type = TypeResolver.Instance.ResolveType(TypeName);
        //        }
        //        return _type;
        //    }
        //}
        //[NonSerialized]
        //private Type _type;
    }
}
