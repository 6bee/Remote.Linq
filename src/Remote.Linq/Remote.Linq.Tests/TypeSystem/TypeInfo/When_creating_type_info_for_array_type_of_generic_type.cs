// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.TypeSystem.TypeInfo
{
    using Remote.Linq.TypeSystem;
    using Xunit;
    using Xunit.Should;

    public class When_creating_type_info_for_array_type_of_generic_type
    {
        class A<T>
        {

        }

        class B
        {

        }

        private readonly TypeInfo typeInfo;


        public When_creating_type_info_for_array_type_of_generic_type()
        {
            typeInfo = new TypeInfo(typeof(A<B>[]));
        }

        [Fact]
        public void Type_info_should_have_is_array_true()
        {
            typeInfo.IsArray.ShouldBeTrue();
        }

        [Fact]
        public void Type_info_should_have_is_generic_true()
        {
            typeInfo.IsGenericType.ShouldBeTrue();
        }

        [Fact]
        public void Type_info_should_have_is_nested_true()
        {
            typeInfo.IsNested.ShouldBeTrue();
        }

        [Fact]
        public void Type_info_name_should_have_array_brackets()
        {
            typeInfo.Name.ShouldBe("A`1[]");
        }
    }
}
