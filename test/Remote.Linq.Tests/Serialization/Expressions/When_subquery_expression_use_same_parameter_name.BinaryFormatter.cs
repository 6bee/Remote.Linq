// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if !NETCOREAPP1_0

namespace Remote.Linq.Tests.Serialization.Expressions
{
    partial class When_subquery_expression_use_same_parameter_name
    {
        public class BinaryFormatter : When_subquery_expression_use_same_parameter_name
        {
            public BinaryFormatter()
                : base(BinarySerializationHelper.Serialize)
            {
            }
        }
    }
}

#endif