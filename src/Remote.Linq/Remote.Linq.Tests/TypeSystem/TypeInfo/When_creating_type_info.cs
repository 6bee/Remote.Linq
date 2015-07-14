// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.TypeSystem.TypeInfo
{
    using Remote.Linq.TypeSystem;
    using Xunit;
    using Xunit.Should;

    public class When_creating_type_info
    {
        class A
        {

        }

        private readonly TypeInfo typeInfo;


        public When_creating_type_info()
        {
            typeInfo = new TypeInfo(typeof(A));
        }

        [Fact]
        public void Type_info_should_have_is_array_true()
        {
            typeInfo.IsArray.ShouldBeFalse();
        }

        [Fact]
        public void Type_info_should_have_is_generic_true()
        {
            typeInfo.IsGenericType.ShouldBeFalse();
        }

        [Fact]
        public void Type_info_should_have_is_nested_true()
        {
            typeInfo.IsNested.ShouldBeTrue();
        }

        [Fact]
        public void Type_info_name_should_have_array_brackets()
        {
            typeInfo.Name.ShouldBe("A");
        }
    }
}
