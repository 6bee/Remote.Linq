// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Remote.Linq.TypeSystem;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This type is used to distinguish variable query arguments from constant query arguments
    /// </summary>
    /// <typeparam name="T">Type of the query argument</typeparam>
    [Serializable]
    [DataContract]
    public sealed class VariableQueryArgument<T>
    {
        public VariableQueryArgument()
        {
        }

        public VariableQueryArgument(T value)
        {   
            Value = value;
        }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public T Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}<{1}>({3}{2}{3})", 
                GetType().Name,
                typeof(T).Name,
                (object)Value ?? "null", 
                Value is string || Value is char ? "'" : null);
        }

        //internal static VariableQueryArgument<T> CreateFromNonGeneric(VariableQueryArgument queryArgument, ITypeResolver typeResolver = null)
        //{          
        //    if (ReferenceEquals(null, queryArgument))
        //    {
        //        throw new ArgumentNullException("queryArgument");
        //    }

        //    var type = (typeResolver ?? TypeResolver.Instance).ResolveType(queryArgument.Type);
        //    if (!typeof(T).IsAssignableFrom(type))
        //    {
        //        throw new Exception(string.Format("Generic type mismatch: {0} vs. {1}", typeof(T), queryArgument.Type));
        //    }

        //    var value = (T)queryArgument.Value;

        //    var instance = new VariableQueryArgument<T>(value);
        //    return instance;
        //}
    }
}
