// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.DynamicQuery;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class QueryableResourceDescriptorMapper : ProtoMapper<QueryableResourceDescriptor, Proto.QueryableResourceDescriptor>
{
    public static readonly QueryableResourceDescriptorMapper Instance = new();

    public override QueryableResourceDescriptor FromProto(Proto.QueryableResourceDescriptor proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Type = TypeInfoMapper.Instance.FromProto(proto.Type, context),
        };

    public override Proto.QueryableResourceDescriptor ToProto(QueryableResourceDescriptor value, ProtoContext context)
        => value is null ? null! : new()
        {
            Type = TypeInfoMapper.Instance.ToProto(value.Type, context),
        };
}
