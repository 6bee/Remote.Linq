// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using Remote.Linq.Expressions;

    partial class When_subquery_expression_use_same_parameter_name
    {
        public class JsonSerializer : When_subquery_expression_use_same_parameter_name
        {
            public JsonSerializer()
                : base(x => (Expression)JsonSerializationHelper.Serialize(x, x.GetType()))
            {
            }
        }
    }
}
