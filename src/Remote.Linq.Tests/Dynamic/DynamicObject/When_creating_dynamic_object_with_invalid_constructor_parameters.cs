// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
    using System;
    using Xunit;

    public class When_creating_dynamic_object_with_invalid_constructor_parameters
    {
        [Fact]
        public void Should_throw_if_passing_null_object_reference()
        {
            Assert.Throws<ArgumentNullException>(() => new DynamicObject((object)null));
        }
    }
}
