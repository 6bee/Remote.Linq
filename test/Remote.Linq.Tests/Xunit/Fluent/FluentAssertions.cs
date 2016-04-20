// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Xunit.Fluent
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public static class FluentAssertions
    {
        public static void AllCollectionItemsShouldPass<T>(this IEnumerable<T> collection, Action<T> action)
        {
            Assert.All<T>(collection, action);
        }

        public static void CollectionItemsShouldMeetCriteria<T>(this IEnumerable<T> collection, params Action<T>[] elementInspectors)
        {
            Assert.Collection<T>(collection, elementInspectors);
        }

        public static void ShouldContain(this string actualString, string expectedSubstring)
        {
            Assert.Contains(expectedSubstring, actualString);
        }

        public static void ShouldContain(this string actualString, string expectedSubstring, StringComparison comparisonType)
        {
            Assert.Contains(expectedSubstring, actualString, comparisonType);
        }

        public static void ShouldContain<T>(this IEnumerable<T> collection, Predicate<T> filter)
        {
            Assert.Contains<T>(collection, filter);
        }

        public static void ShouldContain<T>(this IEnumerable<T> collection, T expected)
        {
            Assert.Contains<T>(expected, collection);
        }

        public static void ShouldContain<T>(this IEnumerable<T> collection, T expected, IEqualityComparer<T> comparer)
        {
            Assert.Contains<T>(expected, collection, comparer);
        }

        public static void ShouldNotContain(this string actualString, string expectedSubstring)
        {
            Assert.DoesNotContain(expectedSubstring, actualString);
        }

        public static void ShouldNotContainthis(string actualString, string expectedSubstring, StringComparison comparisonType)
        {
            Assert.DoesNotContain(expectedSubstring, actualString, comparisonType);
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> collection, T expected)
        {
            Assert.DoesNotContain<T>(expected, collection);
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> collection, Predicate<T> filter)
        {
            Assert.DoesNotContain<T>(collection, filter);
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> collection, T expected, IEqualityComparer<T> comparer)
        {
            Assert.DoesNotContain<T>(expected, collection, comparer);
        }

        public static void ShouldNotMatch(this string actualString, string expectedRegexPattern)
        {
            Assert.DoesNotMatch(expectedRegexPattern, actualString);
        }

        public static void ShouldNotMatch(this string actualString, Regex expectedRegex)
        {
            Assert.DoesNotMatch(expectedRegex, actualString);
        }

        public static void ShouldBeEmpty(this IEnumerable collection)
        {
            Assert.Empty(collection);
        }

        public static void ShouldEndWith(this string actualString, string expectedEndString)
        {
            Assert.EndsWith(expectedEndString, actualString);
        }

        public static void ShouldEndWith(this string actualString, string expectedEndString, StringComparison comparisonType)
        {
            Assert.EndsWith(expectedEndString, actualString, comparisonType);
        }

        public static void ShouldBe(this string actual, string expected)
        {
            Assert.Equal(expected, actual);
        }

        public static void ShouldBe(this decimal actual, decimal expected, int precision)
        {
            Assert.Equal(expected, actual, precision);
        }

        public static void ShouldBe(this double actual, double expected, int precision)
        {
            Assert.Equal(expected, actual, precision);
        }

        public static void ShouldBe(this string actual, string expected, bool ignoreCase = false, bool ignoreLineEndingDifferences = false, bool ignoreWhiteSpaceDifferences = false)
        {
            Assert.Equal(expected, actual, ignoreCase, ignoreLineEndingDifferences, ignoreWhiteSpaceDifferences);
        }

        public static void ShouldBe<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            Assert.Equal<T>(expected, actual);
        }

        public static void ShouldBe<T>(this T actual, T expected)
        {
            Assert.Equal<T>(expected, actual);
        }

        public static void ShouldBe<T>(this IEnumerable<T> actual, IEnumerable<T> expected, IEqualityComparer<T> comparer)
        {
            Assert.Equal<T>(expected, actual, comparer);
        }

        public static void ShouldBe<T>(this T actual, T expected, IEqualityComparer<T> comparer)
        {
            Assert.Equal<T>(expected, actual, comparer);
        }

        public static void ShouldBeFalse(this bool? condition)
        {
            Assert.False(condition);
        }

        public static void ShouldBeFalse(this bool condition)
        {
            Assert.False(condition);
        }

        public static void ShouldBeFalse(this bool? condition, string userMessage)
        {
            Assert.False(condition, userMessage);
        }

        public static void ShouldBeFalse(this bool condition, string userMessage)
        {
            Assert.False(condition, userMessage);
        }

        public static void ShouldBeInRange<T>(this T actual, T low, T high) where T : IComparable
        {
            Assert.InRange<T>(actual, low, high);
        }

        public static void ShouldBeInRange<T>(this T actual, T low, T high, IComparer<T> comparer)
        {
            Assert.InRange<T>(actual, low, high, comparer);
        }

        public static void ShouldBeAssignableFrom(this object @object, Type expectedType)
        {
            Assert.IsAssignableFrom(expectedType, @object);
        }

        public static T ShouldBeAssignableFrom<T>(this object @object)
        {
            return Assert.IsAssignableFrom<T>(@object);
        }

        public static void ShouldNotBeOfType(this object @object, Type expectedType)
        {
            Assert.IsNotType(expectedType, @object);
        }

        public static void ShouldNotBeOfType<T>(this object @object)
        {
            Assert.IsNotType<T>(@object);
        }

        public static void ShouldBeOfType(this object @object, Type expectedType)
        {
            Assert.IsType(expectedType, @object);
        }

        public static T ShouldBeOfType<T>(this object @object)
        {
            return Assert.IsType<T>(@object);
        }

        public static void ShouldMatch(this string actualString, Regex expectedRegex)
        {
            Assert.Matches(expectedRegex, actualString);
        }

        public static void ShouldMatch(this string actualString, string expectedRegexPattern)
        {
            Assert.Matches(expectedRegexPattern, actualString);
        }

        public static void ShouldNotBeEmpty(this IEnumerable collection)
        {
            Assert.NotEmpty(collection);
        }

        public static void ShouldNotBe(this decimal actual, decimal expected, int precision)
        {
            Assert.NotEqual(expected, actual, precision);
        }

        public static void ShouldNotBe(this double actual, double expected, int precision)
        {
            Assert.NotEqual(expected, actual, precision);
        }

        public static void ShouldNotBe<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            Assert.NotEqual<T>(expected, actual);
        }

        public static void ShouldNotBe<T>(this T actual, T expected)
        {
            Assert.NotEqual<T>(expected, actual);
        }

        public static void ShouldNotBe<T>(this T actual, T expected, IEqualityComparer<T> comparer)
        {
            Assert.NotEqual<T>(expected, actual, comparer);
        }

        public static void ShouldNotBe<T>(this IEnumerable<T> actual, IEnumerable<T> expected, IEqualityComparer<T> comparer)
        {
            Assert.NotEqual<T>(expected, actual, comparer);
        }

        public static void ShouldNotBeInRange<T>(this T actual, T low, T high) where T : IComparable
        {
            Assert.NotInRange<T>(actual, low, high);
        }

        public static void ShouldNotBeInRange<T>(this T actual, T low, T high, IComparer<T> comparer)
        {
            Assert.NotInRange<T>(actual, low, high, comparer);
        }

        public static void ShouldNotBeNull(this object @object)
        {
            Assert.NotNull(@object);
        }

        public static void ShouldNotBeSameInstance(this object actual, object expected)
        {
            Assert.NotSame(expected, actual);
        }

        public static void ShouldNotBeStrictlyEqual<T>(this T actual, T expected)
        {
            Assert.NotStrictEqual<T>(expected, actual);
        }

        public static void ShouldBeNull(this object @object)
        {
            Assert.Null(@object);
        }

        public static void ShouldBeProperSubset<T>(this ISet<T> actual, ISet<T> expectedSuperset)
        {
            Assert.ProperSubset<T>(expectedSuperset, actual);
        }

        public static void ShouldBeProperSuperset<T>(this ISet<T> actual, ISet<T> expectedSubset)
        {
            Assert.ProperSuperset<T>(expectedSubset, actual);
        }

        public static void ShouldRaisePropertyChanged(this INotifyPropertyChanged @object, string propertyName, Action testCode)
        {
            Assert.PropertyChanged(@object, propertyName, testCode);
        }

        public static void ShouldBeSameInstance(this object actual, object expected)
        {
            Assert.Same(expected, actual);
        }

        public static object ShouldBeSingle(this IEnumerable collection)
        {
            return Assert.Single(collection);
        }

        public static void ShouldBeSingle(this IEnumerable collection, object expected)
        {
            Assert.Single(collection, expected);
        }

        public static T ShouldBeSingle<T>(this IEnumerable<T> collection)
        {
            return Assert.Single<T>(collection);
        }

        public static T ShouldBeSingle<T>(this IEnumerable<T> collection, Predicate<T> predicate)
        {
            return Assert.Single<T>(collection, predicate);
        }

        public static void ShouldStartWith(this string actualString, string expectedStartString)
        {
            Assert.StartsWith(expectedStartString, actualString);
        }

        public static void ShouldStartWith(this string actualString, string expectedStartString, StringComparison comparisonType)
        {
            Assert.StartsWith(expectedStartString, actualString, comparisonType);
        }

        public static void ShouldBeStrictlyEqual<T>(this T actual, T expected)
        {
            Assert.StrictEqual<T>(expected, actual);
        }

        public static void ShouldBeSubset<T>(this ISet<T> actual, ISet<T> expectedSuperset)
        {
            Assert.Subset<T>(expectedSuperset, actual);
        }

        public static void ShouldBeSuperset<T>(this ISet<T> actual, ISet<T> expectedSubset)
        {
            Assert.Superset<T>(expectedSubset, actual);
        }

        public static Exception ShouldThrow(this Action testCode, Type exceptionType)
        {
            return Assert.Throws(exceptionType, testCode);
        }

        public static Exception ShouldThrow(this Func<object> testCode, Type exceptionType)
        {
            return Assert.Throws(exceptionType, testCode);
        }

        public static T ShouldThrow<T>(this Func<object> testCode) where T : Exception
        {
            return Assert.Throws<T>(testCode);
        }

        public static T ShouldThrow<T>(this Action testCode) where T : Exception
        {
            return Assert.Throws<T>(testCode);
        }

        public static T ShouldThrow<T>(this Action testCode, string paramName) where T : ArgumentException
        {
            return Assert.Throws<T>(paramName, testCode);
        }

        public static T ShouldThrow<T>(this Func<object> testCode, string paramName) where T : ArgumentException
        {
            return Assert.Throws<T>(paramName, testCode);
        }

        public static T ShouldThrowAny<T>(this Action testCode) where T : Exception
        {
            return Assert.ThrowsAny<T>(testCode);
        }

        public static T ShouldThrowAny<T>(this Func<object> testCode) where T : Exception
        {
            return Assert.ThrowsAny<T>(testCode);
        }

        public static Task<T> ShouldThrowAnyAsync<T>(this Func<Task> testCode) where T : Exception
        {
            return Assert.ThrowsAnyAsync<T>(testCode);
        }

        public static Task<Exception> ShouldThrowAsync(this Func<Task> testCode, Type exceptionType)
        {
            return Assert.ThrowsAsync(exceptionType, testCode);
        }

        public static Task<T> ShouldThrowAsync<T>(this Func<Task> testCode) where T : Exception
        {
            return Assert.ThrowsAsync<T>(testCode);
        }

        public static Task<T> ShouldThrowAsync<T>(this Func<Task> testCode, string paramName) where T : ArgumentException
        {
            return Assert.ThrowsAsync<T>(paramName, testCode);
        }

        public static void ShouldBeTrue(this bool? condition)
        {
            Assert.True(condition);
        }

        public static void ShouldBeTrue(this bool condition)
        {
            Assert.True(condition);
        }

        public static void ShouldBeTrue(this bool? condition, string userMessage)
        {
            Assert.True(condition, userMessage);
        }

        public static void ShouldBeTrue(this bool condition, string userMessage)
        {
            Assert.True(condition, userMessage);
        }

        public static void ShouldBeAnnotatedWith<T>(this Type type) where T : Attribute
        {
            if (type.GetTypeInfo().GetCustomAttributes<T>().Count() < 1)
            {
                throw new ExpectedAnnotation(type, typeof(T));
            }
        }

        private class ExpectedAnnotation : Xunit.Sdk.XunitException
        {
            public ExpectedAnnotation(Type type, Type attributeType)
            {
                Type = type;
                AttributeType = attributeType;
            }

            public Type Type{ get; }


            public Type AttributeType{ get; }

            public override string Message
            {
                get
                {
                    return $"Missing custom attribute annotation {Environment.NewLine}Type: {Type.Name} {Environment.NewLine}Expected: {AttributeType.Name} {Environment.NewLine}Found: {Environment.NewLine}{string.Join(Environment.NewLine, Type.GetTypeInfo().GetCustomAttributes().Select(_=> "- "+_.GetType().Name))}";
                }
            }

            public override string ToString()
            {
                return Message;
            }
        }
    }
}