// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.TypeSystem.TypeResolver
{
    using Remote.Linq.TypeSystem;
    using System;
    using Xunit;
    using Xunit.Should;

    public class When_resolving_array_type_of_generic_type
    {
        class A<T>
        {

        }

        class B
        {

        }

        private readonly Type type;


        public When_resolving_array_type_of_generic_type()
        {
            var typeInfo = new TypeInfo(typeof(A<B>[]));

            type = TypeResolver.Instance.ResolveType(typeInfo);
        }

        [Fact]
        public void Type_should_be_expected_array_type()
        {
            type.ShouldBe(typeof(A<B>[]));
        }
    }
}
