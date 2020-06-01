// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua;
    using Aqua.ProtoBuf;
    using global::ProtoBuf.Meta;
    using Remote.Linq.ProtoBuf.DynamicQuery;
    using Remote.Linq.ProtoBuf.Expressions;
    using System;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ProtoBufTypeModel
    {
        public static AquaTypeModel ConfigureRemoteLinq(string? name = null, bool configureDefaultSystemTypes = true)
            => ConfigureRemoteLinq(RuntimeTypeModel.Create(name), configureDefaultSystemTypes);

        public static AquaTypeModel ConfigureRemoteLinq(this RuntimeTypeModel typeModel, bool configureDefaultSystemTypes = true)
            => (typeModel ?? throw new ArgumentNullException(nameof(typeModel)))
            .ConfigureAquaTypes(configureDefaultSystemTypes)
            .ConfigureRemoteLinqDynamicQueryTypes()
            .ConfigureRemoteLinqExpressionTypes();
    }
}
