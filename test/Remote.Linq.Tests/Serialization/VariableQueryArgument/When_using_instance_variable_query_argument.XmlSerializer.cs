// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.VariableQueryArgument
{
    partial class When_using_instance_variable_query_argument
    {
        private class XmlSerializer : When_using_instance_variable_query_argument
        {
            public XmlSerializer()
                : base(XmlSerializationHelper.Serialize)
            {
            }
        }
    }
}