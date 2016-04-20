// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    partial class When_using_simple_projection_to_single_member
    {
        private class XmlSerializer : When_using_simple_projection_to_single_member
        {
            public XmlSerializer()
                : base(XmlSerializationHelper.Serialize)
            {
            }
        }
    }
}