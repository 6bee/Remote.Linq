// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.VariableQueryArgument
{
    using System.Collections.Generic;

    partial class When_using_local_variable_query_argument_list
    {
        public class XmlSerializer : When_using_local_variable_query_argument_list
        {
            public XmlSerializer()
                : base(x => XmlSerializationHelper.SerializeExpression(x, new[] { typeof(List<int>) }))
            {
            }
        }
    }
}