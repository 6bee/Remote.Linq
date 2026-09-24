// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionVisitors.ExpressionTranslator;

/// <summary>
/// Records side effects triggered by compiled expression trees.
/// Exposed as a standalone public type (rather than a member of the test class) so that the
/// side-effect hook remains callable from lambdas compiled into a dedicated dynamic type on
/// net48, where cross-assembly access to non-public members is not possible.
/// </summary>
public static class SideEffectRecorder
{
    private static readonly ThreadLocal<int> _count = new(() => 0);

    public static int Count
        => _count.Value;

    public static void Reset()
        => _count.Value = 0;

    public static void RecordSideEffect()
        => _count.Value++;
}
