// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.TypeSystem.TypeResolver
{
    using Remote.Linq.TypeSystem;
    using System;
    using Xunit;
    using Xunit.Should;

    public class When_resolving_anonymous_type
    {
        private readonly Type actualType;
        private readonly Type resolvedType;
        
        public When_resolving_anonymous_type()
        {
            var instance = new { Int32Value = 0, StringValue = "" };

            actualType = instance.GetType();

            var typeInfo = new TypeInfo(actualType);

            resolvedType = TypeResolver.Instance.ResolveType(typeInfo);
        }

        [Fact]
        public void Resolved_type_should_be_actual_type()
        {
            resolvedType.ShouldBe(actualType);
        }
    }
}
