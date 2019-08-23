// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// This type is used to distinguish variable query arguments from constant query arguments.
    /// </summary>
    [Serializable]
    [DataContract]
    public sealed class VariableQueryArgumentList
    {
        public VariableQueryArgumentList()
        {
        }

        public VariableQueryArgumentList(System.Collections.IEnumerable values, Type elementType = null)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (elementType is null)
            {
                var collectionType = values.GetType();
                elementType = TypeHelper.GetElementType(collectionType);
            }

            ElementType = new TypeInfo(elementType, false, false);

            Values = values.Cast<object>().ToList();
        }

        public VariableQueryArgumentList(System.Collections.IEnumerable values, TypeInfo elementType = null)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (elementType is null)
            {
                var collectionType = values.GetType();
                var type = TypeHelper.GetElementType(collectionType);
                elementType = new TypeInfo(type, false, false);
            }

            ElementType = elementType;

            Values = values.Cast<object>().ToList();
        }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo ElementType { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = true)]
        public List<object> Values { get; set; }

        public override string ToString()
        {
            var elementType = ElementType;
            var values = Values;

            return string.Format(
                "{0}Of{1}[{2}]",
                GetType().Name,
                elementType is null ? "?" : elementType.Name,
                values is null ? 0 : values.Count);
        }
    }
}
