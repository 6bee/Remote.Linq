// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.Tests
{
    using Remote.Linq.Async.Queryable.TestSupport;
    using Shouldly;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public abstract class When_querying_async
    {
        public class With_stream_provider_and_data_provider : When_querying_async
        {
            public With_stream_provider_and_data_provider(bool useDynamicObjectConversion = false)
                : base(useDynamicObjectConversion: useDynamicObjectConversion)
            {
            }
        }

        public class With_dynamic_object_stream_provider_and_data_provider : With_stream_provider_and_data_provider
        {
            public With_dynamic_object_stream_provider_and_data_provider()
                : base(useDynamicObjectConversion: true)
            {
            }
        }

        public class With_stream_provider_only : When_querying_async, IDisposable
        {
            public With_stream_provider_only(bool useDynamicObjectConversion = false)
                : base(createAsyncDataProvider: false, useDynamicObjectConversion: useDynamicObjectConversion)
            {
            }

            public void Dispose()
            {
                AsyncDataProviderInvocationCount.ShouldBe(0);
                AsyncStreamProviderInvocationCount.ShouldBe(1);
            }
        }

        public class With_dynamic_object_stream_provider_only : With_stream_provider_only
        {
            public With_dynamic_object_stream_provider_only()
                : base(useDynamicObjectConversion: true)
            {
            }
        }

        public class With_async_data_provider_only : When_querying_async, IDisposable
        {
            public With_async_data_provider_only(bool useDynamicObjectConversion = false)
                : base(createAsyncStreamProvider: false, useDynamicObjectConversion: useDynamicObjectConversion)
            {
            }

            public void Dispose()
            {
                AsyncStreamProviderInvocationCount.ShouldBe(0);
                AsyncDataProviderInvocationCount.ShouldBe(1);
            }
        }

        public class With_async_dynamic_object_data_provider_only : With_async_data_provider_only
        {
            public With_async_dynamic_object_data_provider_only()
                : base(useDynamicObjectConversion: true)
            {
            }
        }

        private readonly bool _hasStreamProviderSupport;
        private readonly bool _hasDataProviderSupport;

        public When_querying_async(bool createAsyncStreamProvider = true, bool createAsyncDataProvider = true, bool useDynamicObjectConversion = true)
        {
            _hasStreamProviderSupport = createAsyncStreamProvider;
            _hasDataProviderSupport = createAsyncDataProvider;
            AsyncStream = Enumerable.Range(0, 10)
                .Select(x => new Entity(x))
                .AsAsyncQueryable(
                    createAsyncStreamProvider,
                    createAsyncDataProvider,
                    useDynamicObjectConversion,
                    _ => AsyncStreamProviderInvocationCount++,
                    _ => AsyncDataProviderInvocationCount++);
        }

        protected int AsyncStreamProviderInvocationCount { get; private set; }

        protected int AsyncDataProviderInvocationCount { get; private set; }

        protected IAsyncQueryable<Entity> AsyncStream { get; }

        [Fact]
        public async Task Should_query_full_set_via_async_foreach_stream()
        {
            var count = 0;
            await foreach (var item in AsyncStream)
            {
                count++;
            }

            count.ShouldBe(10);

            AssertProviderInvocationForStream();
        }

        [Fact]
        public async Task Should_query_full_set_via_ForEachAsync()
        {
            var count = 0;
            await AsyncStream.ForEachAsync(_ => count++);

            count.ShouldBe(10);

            AssertProviderInvocationForStream();
        }

        [Fact]
        public async Task Should_query_async_enumerable_stream_via_ForEachAsync()
        {
            var count = 0;
            await AsyncStream.AsAsyncEnumerable().ForEachAsync(_ => count++);

            count.ShouldBe(10);

            AssertProviderInvocationForStream();
        }

        [Fact]
        public async Task Should_query_async_enumerable_streaming_CountAsync()
        {
            var count = await AsyncStream.AsAsyncEnumerable().CountAsync();

            count.ShouldBe(10);

            AssertProviderInvocationForStream();
        }

        [Fact]
        public async Task Should_query_non_streaming_CountAsync()
        {
            var count = await AsyncStream.CountAsync();

            count.ShouldBe(10);

            AssertProviderInvocationForNonStream();
        }

        [Fact]
        public async Task Should_query_filtered_set_via_async_foreach_stream()
        {
            var query =
                from x in AsyncStream
                where x.Id >= 5 && x.Id <= 7
                select x;

            var count = 0;
            await foreach (var item in query)
            {
                count++;
            }

            count.ShouldBe(3);

            AssertProviderInvocationForStream();
        }

        [Fact]
        public async Task Should_query_async_enumerable_streaming_single_item()
        {
            var single = await AsyncStream
                .AsAsyncEnumerable()
                .SingleAsync(x => x.Id == 5)
                .ConfigureAwait(false);

            single.Id.ShouldBe(5);

            AssertProviderInvocationForStream();
        }

        [Fact]
        public async Task Should_query_async_enumerable_streaming_AverageAsync()
        {
            var average = await AsyncStream.AsAsyncEnumerable().Select(x => x.Id).AverageAsync();

            average.ShouldBe(4.5);

            AssertProviderInvocationForStream();
        }

        [Fact]
        public async Task Should_query_non_streaming_SingleAsync()
        {
            var single = await AsyncStream.SingleAsync(x => x.Id == 5);

            single.Id.ShouldBe(5);

            AssertProviderInvocationForNonStream();
        }

        [Fact]
        public async Task Should_query_non_streaming_ToListAsync()
        {
            var list = await AsyncStream.ToListAsync();

            list.Count.ShouldBe(10);

            AssertProviderInvocationForNonStream();
        }

        [Fact]
        public async Task Should_query_non_streaming_ToArrayAsync()
        {
            var array = await AsyncStream.ToArrayAsync();

            array.Length.ShouldBe(10);

            AssertProviderInvocationForNonStream();
        }

        [Fact]
        public async Task Should_query_non_streaming_AverageAsync()
        {
            var average = await AsyncStream.Select(x => x.Id).AverageAsync();

            average.ShouldBe(4.5);

            AssertProviderInvocationForNonStream();
        }

        [Fact]
        public async Task Should_query_non_streaming_GroupBy()
        {
            var groupedResult = await AsyncStream
                .GroupBy(x => x.Id)
                .ToListAsync()
                .ConfigureAwait(false);

            groupedResult.Count.ShouldBe(10);

            AssertProviderInvocationForNonStream();
        }

        private void AssertProviderInvocationForStream()
        {
            if (_hasStreamProviderSupport)
            {
                AsyncDataProviderInvocationCount.ShouldBe(0);
                AsyncStreamProviderInvocationCount.ShouldBe(1);
            }
            else
            {
                AsyncStreamProviderInvocationCount.ShouldBe(0);
                AsyncDataProviderInvocationCount.ShouldBe(1);
            }
        }

        private void AssertProviderInvocationForNonStream()
        {
            if (_hasDataProviderSupport)
            {
                AsyncStreamProviderInvocationCount.ShouldBe(0);
                AsyncDataProviderInvocationCount.ShouldBe(1);
            }
            else
            {
                AsyncDataProviderInvocationCount.ShouldBe(0);
                AsyncStreamProviderInvocationCount.ShouldBe(1);
            }
        }
    }
}