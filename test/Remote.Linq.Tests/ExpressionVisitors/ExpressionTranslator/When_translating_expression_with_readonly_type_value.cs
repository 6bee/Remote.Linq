// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionVisitors.ExpressionTranslator
{
    using Aqua.Dynamic;
    using Shouldly;
    using System;
    using System.Linq.Expressions;
    using Xunit;

    public class When_translating_expression_with_readonly_type_value : ExpressionTranslatorTestBase
    {
        private sealed class Date
        {
            private readonly DateTime _value;

            private Date(DateTime value)
                => _value = value;

            public int Year => _value.Year;

            public int Month => _value.Month;

            public int Day => _value.Day;

            public override bool Equals(object obj)
                => obj is Date timestamp
                && timestamp._value.Equals(_value);

            public override int GetHashCode()
                => _value.GetHashCode();

            public override string ToString()
                => $"{_value:yyyy-MM-dd}";

            public static Date From(DateTime date)
                => new Date(date);

            public static Date From(int year, int month, int day)
                => new Date(new DateTime(year, month, day));
        }

        private static class DateHelper
        {
            public static Date ToDate(DynamicObject date)
            {
                var d = date.Get<int>("Day");
                var m = date.Get<int>("Month");
                var y = date.Get<int>("Year");
                return Date.From(y, m, d);
            }

            public static DynamicObject ToDynamicObject(Date date)
                => new DynamicObject(new PropertySet
                {
                    { "Day", date.Day },
                    { "Month", date.Month },
                    { "Year", date.Year },
                });
        }

        private sealed class CustomExpressionTranslatorContext : ExpressionTranslatorContext
        {
            private sealed class ObjectMapper : ExpressionTranslatorContextObjectMapper
            {
                public ObjectMapper(ExpressionTranslatorContext expressionTranslatorContext)
                    : base(expressionTranslatorContext)
                {
                }

                protected override DynamicObject MapToDynamicObjectGraph(object obj, Func<Type, bool> setTypeInformation)
                {
                    if (obj is Date date)
                    {
                        return DateHelper.ToDynamicObject(date);
                    }

                    return base.MapToDynamicObjectGraph(obj, setTypeInformation);
                }

                protected override object MapFromDynamicObjectGraph(object obj, Type targetType)
                {
                    if (targetType == typeof(Date) && obj is DynamicObject date)
                    {
                        return DateHelper.ToDate(date);
                    }

                    return base.MapFromDynamicObjectGraph(obj, targetType);
                }
            }

            public CustomExpressionTranslatorContext()
                => ValueMapper = new ObjectMapper(this);

            public override IDynamicObjectMapper ValueMapper { get; }
        }

        [Fact]
        public void Should_fail_without_custom_value_mapper()
        {
            var ex = Assert.Throws<DynamicObjectMapperException>(() => BackAndForth(Expression.Constant(Date.From(DateTime.Now))));
            ex.Message.ShouldBe($"Failed to pick matching constructor for type {typeof(Date).FullName}");
        }

        [Fact]
        public void Should_apply_custom_mapper_to_transform_value()
        {
            var context = new CustomExpressionTranslatorContext();
            BackAndForth(Expression.Constant(Date.From(DateTime.Now)), context).ShouldMatch();
        }
    }
}