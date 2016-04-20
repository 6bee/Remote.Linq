// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if NET

namespace Remote.Linq.Tests.Serialization.Expressions
{
    partial class When_using_simple_projection_to_single_member
    {
        private class NetDataContractSerializer : When_using_simple_projection_to_single_member
        {
            public NetDataContractSerializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
    }
}

#endif