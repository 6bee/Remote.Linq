// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.Tests.TestInfrastructure
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    internal class ContextMock<TContext> : Mock<TContext>
        where TContext : DbContext
    {
        public ContextMock()
        {
            Setup(x => x.SaveChanges()).Returns(1);
            Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(1));
        }

        public ContextMock<TContext> WithSet<TEntity>(Expression<Func<TContext, DbSet<TEntity>>> selector, List<TEntity> inMemoryStore)
            where TEntity : class
        {
            var mockSet = new DbSetMock<TEntity>(inMemoryStore);
            Setup(c => c.Set(typeof(TEntity))).Returns(mockSet.AsNonGeneric().Object);
            Setup(c => c.Set<TEntity>()).Returns(mockSet.Object);
            Setup(selector).Returns(mockSet.Object);

            return this;
        }
    }
}