// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if NET

namespace Remote.Linq.Tests.Serialization.Expressions
{
    partial class When_subquery_expression_use_same_parameter_name
    {
        public class NetDataContractSerializer : When_subquery_expression_use_same_parameter_name
        {
            public NetDataContractSerializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
    }
}

#endif