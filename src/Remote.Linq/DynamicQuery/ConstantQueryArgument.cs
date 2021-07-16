// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Dynamic;
    using System;
    using System.Runtime.Serialization;
    using RemoteLinq = Remote.Linq.Expressions;

    /// <summary>
    /// This type is used to wrap complex constant query argument values in <see cref="RemoteLinq.ConstantExpression"/>.
    /// It is used in particular to substitute anonymous types within linq expressions.
    /// </summary>
    [Serializable]
    [DataContract]
    [KnownType(typeof(DynamicObject))]
    [QueryArgument]
    public sealed class ConstantQueryArgument
    {
        public ConstantQueryArgument()
        {
        }

        public ConstantQueryArgument(DynamicObject value)
            => Value = value.CheckNotNull(nameof(value));

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public DynamicObject Value { get; set; } = default!;
    }
}