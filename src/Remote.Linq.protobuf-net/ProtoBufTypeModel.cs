// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua;
    using global::ProtoBuf.Meta;
    using Remote.Linq.ProtoBuf.DynamicQuery;
    using Remote.Linq.ProtoBuf.Expressions;
    using System;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ProtoBufTypeModel
    {
        public static RuntimeTypeModel ConfigureRemoteLinq(string? name = null)
            => ConfigureRemoteLinq(RuntimeTypeModel.Create(name));

        public static RuntimeTypeModel ConfigureRemoteLinq(this RuntimeTypeModel typeModel)
            => (typeModel ?? throw new ArgumentNullException(nameof(typeModel)))
            .ConfigureAquaTypes()
            .ConfigureRemoteLinqDynamicQueryTypes()
            .ConfigureRemoteLinqExpressionTypes();
    }
}
