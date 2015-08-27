// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.TypeSystem;
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

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = true)]
        public T Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}<{1}>({3}{2}{3})", 
                GetType().Name,
                typeof(T).Name,
                (object)Value ?? "null", 
                Value is string || Value is char ? "'" : null);
        }
    }
}
