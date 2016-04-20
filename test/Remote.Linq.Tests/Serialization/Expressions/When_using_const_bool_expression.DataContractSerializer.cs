// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    partial class When_using_const_bool_expression
    {
        private class DataContractSerializer : When_using_const_bool_expression
        {
            public DataContractSerializer()
                : base(DataContractSerializationHelper.Serialize)
            {
            }
        }
    }
}