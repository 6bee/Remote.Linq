// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.DynamicQuery.RemoteQueryable
{
    using Remote.Linq;
    using Shouldly;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

#pragma warning disable SA1629 // Documentation text should end with a period
    /// <summary>
    /// I.e. without query execution without <code>ToList()</code>, <code>ToArray()</code>, etc.
    /// </summary>
#pragma warning restore SA1629 // Documentation text should end with a period
    public class When_executing_straight
    {
        private class Entity
        {
        }

        [Fact]
        public void Should_return_the_untransformed_instance()
        {
            var instance = new Entity();
            var queryable = RemoteQueryable.Factory.CreateQueryable<Entity>(exp => instance);
            var result = queryable.Execute<Entity>();
            result.ShouldBeSameAs(instance);
        }

        [Fact]
        public void Should_return_collection_with_the_the_untransformed_instance()
        {
            var instance = new Entity();
            var queryable = RemoteQueryable.Factory.CreateQueryable<Entity>(exp => new[] { instance });
            var result = queryable.Execute<IEnumerable<Entity>>();
            result.Single().ShouldBeSameAs(instance);
        }

        [Fact]
        public async Task Should_return_the_untransformed_instance_async()
        {
            var instance = new Entity();
            var queryable = RemoteQueryable.Factory.CreateQueryable<Entity>(exp => Task.FromResult<object>(instance));
            var result = await queryable.ExecuteAsync<Entity>();
            result.ShouldBeSameAs(instance);
        }

        [Fact]
        public async Task Should_return_collection_with_the_the_untransformed_instance_async()
        {
            var instance = new Entity();
            var queryable = RemoteQueryable.Factory.CreateQueryable<Entity>(exp => Task.FromResult<object>(new[] { instance }));
            var result = await queryable.ExecuteAsync<Entity[]>();
            result.Single().ShouldBeSameAs(instance);
        }

        [Fact]
        public void Should_return_the_untransformed_instance_from_untyped_queryable()
        {
            var instance = new Entity();
            var queryable = RemoteQueryable.Factory.CreateQueryable(typeof(object), exp => instance);
            var result = queryable.Execute<Entity>();
            result.ShouldBeSameAs(instance);
        }

        [Fact]
        public void Should_return_collection_with_the_the_untransformed_instance_from_untyped_queryable()
        {
            var instance = new Entity();
            var queryable = RemoteQueryable.Factory.CreateQueryable(typeof(object), exp => new[] { instance });
            var result = queryable.Execute<Entity[]>();
            result.Single().ShouldBeSameAs(instance);
        }

        [Fact]
        public async Task Should_return_the_untransformed_instance_from_untyped_queryable_async()
        {
            var instance = new Entity();
            var queryable = RemoteQueryable.Factory.CreateQueryable(typeof(object), exp => Task.FromResult<object>(instance));
            var result = await queryable.ExecuteAsync<Entity>();
            result.ShouldBeSameAs(instance);
        }

        [Fact]
        public async Task Should_return_collection_with_the_the_untransformed_instance_from_untyped_queryable_async()
        {
            var instance = new Entity();
            var queryable = RemoteQueryable.Factory.CreateQueryable(typeof(object), exp => Task.FromResult<object>(new[] { instance }));
            var result = await queryable.ExecuteAsync<IEnumerable<Entity>>();
            result.Single().ShouldBeSameAs(instance);
        }
    }
}