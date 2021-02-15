// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Expressions.ExpressionExtensions
{
    using Remote.Linq.Expressions;
    using Shouldly;
    using System;
    using Xunit;

    public class When_executing_expresson_with_explicit_result_type
    {
        private class Entity
        {
        }

        private class AnotherEntity
        {
        }

        [Fact]
        public void Should_cast_and_return_result()
        {
            var instance = new Entity();
            var exp = new ConstantExpression(instance);
            var result = exp.Execute<Entity>(_ => null);
            result.ShouldBeSameAs(instance);
        }

        [Fact]
        public void Should_keep_object_type_and_return_result()
        {
            var instance = new Entity();
            var exp = new ConstantExpression(instance);
            var result = exp.Execute<object>(_ => null);
            result.ShouldBeSameAs(instance);
        }

        [Fact]
        public void Should_throw_upon_invalid_type_cast()
        {
            var instance = new Entity();
            var exp = new ConstantExpression(instance);
            var ex = Should.Throw<InvalidCastException>(() => _ = exp.Execute<AnotherEntity>(_ => null));
            ex.Message.ShouldBe("Unable to cast object of type 'Entity' to type 'AnotherEntity'.");
        }

        [Theory]
        [MemberData(nameof(TestData.TestValues), MemberType = typeof(TestData))]
        public void Should_cast_any_result_to_object_and_return_value(Type type, object value)
        {
            var exp = new ConstantExpression(value);
            var result = exp.Execute<object>(_ => null);

            if (type.IsClass)
            {
                result.ShouldBeSameAs(value);
            }
            else
            {
                result.ShouldBe(value);

                if (result is null)
                {
                    type.ShouldBeNullable();
                }
                else
                {
                    var nonNullType = type.AsNonNullableType();
                    result.ShouldBeOfType(nonNullType);
                }
            }
        }
    }
}
