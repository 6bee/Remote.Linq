// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if NET

namespace Remote.Linq.Tests.Serialization.Expressions
{
    partial class When_using_complex_expression_with_local_lambda
    {
        public class NetDataContractSerializer : When_using_complex_expression_with_local_lambda
        {
            public NetDataContractSerializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
    }
}

#endif