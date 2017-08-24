// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using DynamicObject = Aqua.Dynamic.DynamicObject;
    using TypeInfo = Aqua.TypeSystem.TypeInfo;

    /// <summary>
    /// This type is used to wrap complex constant query argument values in <see cref="Expressions.ConstantExpression"/>.
    /// It is used in particular to substitute anonymous types within linq expressions.
    /// </summary>
    [Serializable]
    [DataContract]
    [KnownType(typeof(DynamicObject))]
    [XmlInclude(typeof(TimeSpan))]
    public sealed class ConstantQueryArgument : DynamicObject
    {
        public ConstantQueryArgument()
        {
        }

        public ConstantQueryArgument(Type type)
            : base(type)
        {
        }

        public ConstantQueryArgument(TypeInfo type)
            : base(type)
        {
        }

        internal ConstantQueryArgument(ConstantQueryArgument constantQueryArgument)
            : base(constantQueryArgument, true)
        {
        }
    }
}
