// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.Tests.TestInfrastructure
{
    using Moq;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class DbSetMock<T> : Mock<DbSet<T>> where T : class
    {
        private readonly IQueryable<T> _queryable;

        public DbSetMock(List<T> inMemoryStore)
        {
            _queryable = inMemoryStore.AsQueryable();
            As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<T>(_queryable.Provider));
            As<IQueryable<T>>().Setup(m => m.Expression).Returns(_queryable.Expression);
            As<IQueryable<T>>().Setup(m => m.ElementType).Returns(_queryable.ElementType);
            As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(_queryable.GetEnumerator());

            As<IDbAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator()).Returns(() =>
            {
                var asyncmock = new Mock<IDbAsyncEnumerator<T>>();
                var enumerator = _queryable.GetEnumerator();
                asyncmock.Setup(x => x.Current).Returns(() => enumerator.Current);
                asyncmock.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(enumerator.MoveNext()));
                return asyncmock.Object;
            });

            Setup(m => m.Add(It.IsAny<T>())).Callback<T>(inMemoryStore.Add).Returns<T>(x => x);
        }

        public Mock<DbSet> AsNonGeneric()
        {
            var mock = new Mock<DbSet>();
            As<IQueryable>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<T>(_queryable.Provider));
            As<IQueryable>().Setup(m => m.Expression).Returns(_queryable.Expression);
            As<IQueryable>().Setup(m => m.ElementType).Returns(_queryable.ElementType);
            As<IQueryable>().Setup(m => m.GetEnumerator()).Returns(_queryable.GetEnumerator());

            As<IDbAsyncEnumerable>().Setup(m => m.GetAsyncEnumerator()).Returns(() =>
            {
                var asyncmock = new Mock<IDbAsyncEnumerator>();
                var enumerator = _queryable.GetEnumerator();
                asyncmock.Setup(x => x.Current).Returns(() => enumerator.Current);
                asyncmock.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(enumerator.MoveNext()));
                return asyncmock.Object;
            });

            return mock;
        }
    }
}
