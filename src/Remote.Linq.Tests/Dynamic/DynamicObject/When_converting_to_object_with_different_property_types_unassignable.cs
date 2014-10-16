// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Dynamic.DynamicObject
{
    using Remote.Linq.Dynamic;
	using Xunit;
    using Xunit.Should;

    /// <summary>
    /// Covers mapping type missmatches for types which allow exlicit conversion only
    /// </summary>
    public class When_converting_to_object_with_different_property_types_unassignable
    {
        class A
        {
        }

        class B
        {
        }

        class C
        {
            public static explicit operator C(A a)
            {
                return new C();
            }
        }

        class D
        {
            public static implicit operator D(A a)
            {
                return new D();
            }

        }

        class CustomType
        {
            public int Int32Value { get; set; }
            public double? NullableDoubleValue { get; set; }
            public string StringValue { get; set; }
            public B BProperty { get; set; }
            public C CProperty { get; set; }
            public D DProperty { get; set; }
        }

        const long Longvalue = 12345L;
        const double DoubleValue = 12.3456789;
        const string StringValue = "eleven";

        CustomType obj;

        public When_converting_to_object_with_different_property_types_unassignable()
        {
            var dynamicObject = new DynamicObject
            {
                { "NumericValue", DoubleValue },
                { "NullableDoubleValue", Longvalue },
                { "StringValue", StringValue },
                { "BProperty", new A() },
                { "CProperty", new A() },
                { "DProperty", new A() },
            };

            obj = dynamicObject.CreateObject<CustomType>();
        }

        [Fact]
        public void Should_create_an_instance()
        {
            obj.ShouldNotBeNull();
        }

        [Fact]
        public void Should_have_the_int_property_not_set()
        {
            obj.Int32Value.ShouldBe(default(int)); // double cannot be automatically converted into int
        }

        [Fact]
        public void Should_have_the_nullabledouble_property_not_set()
        {
            obj.NullableDoubleValue.ShouldBeNull(); // long cannot be automatically converted into Nullable<double>
        }

        [Fact]
        public void Should_have_the_string_property_set()
        {
            obj.StringValue.ShouldBe(StringValue);
        }

        [Fact]
        public void Should_have_the_property_of_type_B_not_set()
        {
            obj.BProperty.ShouldBeNull(); // cannot assign A to B
        }

        [Fact]
        public void Should_have_the_property_of_type_C_not_set()
        {
            obj.CProperty.ShouldBeNull(); // cannot automatically assign A to C (despite explicit cast!)
        }

        [Fact]
        public void Should_have_the_property_of_type_D_not_set()
        {
            obj.DProperty.ShouldBeNull(); // cannot automatically assign A to D (despite implicit cast!)
        }
    }
}