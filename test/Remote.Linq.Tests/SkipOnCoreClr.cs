// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests
{
    using Xunit;

    public static class SkipOnCoreClr
    {
        /// <summary>
        /// Throws an exception that results in a "Skipped" result for the test.
        /// </summary>
        /// <param name="condition">The condition that must evaluate to true for the test to be skipped.</param>
        /// <param name="reason">The explanation for why the test is skipped.</param>
        public static void If(bool condition, string reason = null)
        {
            if (Helper.CoreClr)
            {
                Skip.If(condition, reason);
            }
        }

        /// <summary>
        /// Throws an exception that results in a "Skipped" result for the test.
        /// </summary>
        /// <param name="condition">The condition that must evaluate to false for the test to be skipped.</param>
        /// <param name="reason">The explanation for why the test is skipped.</param>
        public static void IfNot(bool condition, string reason = null)
        {
            if (Helper.CoreClr)
            {
                Skip.IfNot(condition, reason);
            }
        }
    }
}