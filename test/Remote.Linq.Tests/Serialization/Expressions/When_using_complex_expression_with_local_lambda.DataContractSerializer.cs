// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    partial class When_using_complex_expression_with_local_lambda
    {
        private class DataContractSerializer : When_using_complex_expression_with_local_lambda
        {
            public DataContractSerializer()
                : base(DataContractSerializationHelper.Serialize)
            {
            }
        }
    }
}