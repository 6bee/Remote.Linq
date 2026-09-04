// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests;

using System.Diagnostics.CodeAnalysis;

public static class Skip
{
    public static void If([DoesNotReturnIf(true)] bool condition, string reason) => Assert.SkipWhen(condition, reason);

    public static void IfNot([DoesNotReturnIf(false)] bool condition, string reason) => Assert.SkipUnless(condition, reason);
}
