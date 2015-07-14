// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.TypeSystem.TypeInfo
{
    using Remote.Linq.TypeSystem;
    using Xunit;
    using Xunit.Should;

    public class When_creating_type_info_of_anonymous_type
    {
        private readonly TypeInfo typeInfo;

        public When_creating_type_info_of_anonymous_type()
        {
            var instance = new { };
            var type = instance.GetType();
            typeInfo = new TypeInfo(type);
        }

        [Fact]
        public void Type_info_should_have_is_anonymous_true()
        {
            typeInfo.IsAnonymousType.ShouldBeTrue();
        }

        [Fact]
        public void Type_info_should_have_is_array_false()
        {
            typeInfo.IsArray.ShouldBeFalse();
        }

        [Fact]
        public void Type_info_should_have_is_generic_false()
        {
            typeInfo.IsGenericType.ShouldBeFalse();
        }

        [Fact]
        public void Type_info_should_have_is_nested_false()
        {
            typeInfo.IsNested.ShouldBeFalse();
        }
    }
}
