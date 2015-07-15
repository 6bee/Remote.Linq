// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.TypeSystem.TypeResolver
{
    using Remote.Linq.TypeSystem;
    using System;
    using Xunit;
    using Xunit.Should;

    public class When_resolving_unknown_anonymous_type
    {
        private readonly Type actualType;
        private readonly Type emitedType;

        public When_resolving_unknown_anonymous_type()
        {
            var instance = new { Int32Value= 0, StringValue = "" };

            var actualType = instance.GetType();

            var typeInfo = new TypeInfo(actualType);

            typeInfo.Name = "UnknowTestClass";
            typeInfo.Namespace = "Unkown.Test.Namespace";

            emitedType = TypeResolver.Instance.ResolveType(typeInfo);
        }

        [Fact]
        public void Type_should_be_different_from_actual_type()
        {
            emitedType.ShouldNotBe(actualType);
        }

        [Fact]
        public void Type_should_be_dynamically_emited_type()
        {
            emitedType.ShouldNotBeNull();

            emitedType.Namespace.ShouldBe("<In Memory Module>");

            emitedType.Assembly.GetName().Name.ShouldBe("Remote.Linq.TypeSystem.Emit.Types");

            emitedType.Assembly.IsDynamic.ShouldBeTrue();
        }

        [Fact]
        public void Emited_type_should_be_generic_type()
        {
            emitedType.IsGenericType.ShouldBeTrue();
        }

        [Fact]
        public void Emited_type_should_closed_generic_type()
        {
            emitedType.IsGenericTypeDefinition.ShouldBeFalse();
        }

        [Fact]
        public void Emited_type_should_have_two_generic_arguments()
        {
            emitedType.GetGenericArguments().Length.ShouldBe(2);
        }

        [Fact]
        public void Emited_type_should_have_two_readonly_properties()
        {
            var properties = emitedType.GetProperties();

            properties.Length.ShouldBe(2);

            foreach (var p in properties)
            {
                p.CanRead.ShouldBeTrue();
                p.CanWrite.ShouldBeFalse();
            }
        }

        [Fact]
        public void Emited_type_should_have_int_property()
        {
            emitedType.GetProperty("Int32Value").PropertyType.ShouldBe(typeof(int));
        }

        [Fact]
        public void Emited_type_should_have_string_property()
        {
            emitedType.GetProperty("StringValue").PropertyType.ShouldBe(typeof(string));
        }
    }
}
