// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests
{
    using Xunit;

    public static class SkipOnNetCoreApp1_0
    {
        /// <summary>
        /// Throws an exception that results in a "Skipped" result for the test.
        /// </summary>
        /// <param name="condition">The condition that must evaluate to true for the test to be skipped.</param>
        /// <param name="reason">The explanation for why the test is skipped.</param>
        public static void If(bool condition, string reason = null)
        {
            if (Helper.NetCoreApp1_0)
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
            if (Helper.NetCoreApp1_0)
            {
                Skip.IfNot(condition, reason);
            }
        }
    }
}