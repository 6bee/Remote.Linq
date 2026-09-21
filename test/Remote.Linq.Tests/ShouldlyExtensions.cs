// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#nullable enable
namespace Remote.Linq.Tests;

using Shouldly;
using System;

public static class ShouldlyExtensions
{
    extension(Type? actual)
    {
        public void ShouldBeAssignableTypeTo<T>(string? customMessage = null)
            => actual.ShouldBeAssignableTypeTo(typeof(T), customMessage);

        public void ShouldBeAssignableTypeTo(Type expected, string? customMessage = null)
            => actual.AssertAwesomely(
            actualType => actualType != null && expected.IsAssignableFrom(actualType),
            actual,
            expected,
            customMessage);
    }
}
