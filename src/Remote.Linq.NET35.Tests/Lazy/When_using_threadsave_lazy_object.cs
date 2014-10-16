// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.NET35.Tests.Lazy
{
    using System.Threading;
    using Xunit;
    using Xunit.Should;

    public class When_using_threadsave_lazy_object
    {
        int count = 0;
        Lazy<int> lazy;

        public When_using_threadsave_lazy_object()
        {
            lazy = new Lazy<int>(() => Interlocked.Increment(ref count), true);
        }

        [Fact]
        public void Should_not_execute_factory_delegate_if_value_property_was_not_accessed()
        {
            count.ShouldBe(0);
        }

        [Fact]
        public void Value_property_should_return_expected_value()
        {
            lazy.Value.ShouldBe(1);
            lazy.Value.ShouldBe(1);
            lazy.Value.ShouldBe(1);
        }

        [Fact]
        public void Should_execute_factory_delegate_only_once()
        {
            var v1 = lazy.Value;
            var v2 = lazy.Value;
            var v3 = lazy.Value;

            count.ShouldBe(1);            
        }
    }
}
