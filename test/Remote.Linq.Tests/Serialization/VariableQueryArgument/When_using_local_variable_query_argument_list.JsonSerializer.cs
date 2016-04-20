// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.VariableQueryArgument
{
    partial class When_using_local_variable_query_argument_list
    {
        private class JsonSerializer : When_using_local_variable_query_argument_list
        {
            public JsonSerializer()
                : base(JsonSerializationHelper.Serialize)
            {
            }
        }
    }
}