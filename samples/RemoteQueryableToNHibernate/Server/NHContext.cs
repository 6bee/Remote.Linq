// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using NHibernate;
    using System;
    using System.Linq;

    public class NHContext : IDisposable
    {
        private static readonly ISessionFactory _sessionFactory = CreateSessionFactory();

        private readonly ISession _session;

        public NHContext()
        {
            _session = _sessionFactory.OpenSession();
        }

        public IQueryable GetQueryable(Type type)
        {
            System.Reflection.MethodInfo method = typeof(NHContext).GetMethods()
                .Single(x => x.Name == nameof(GetQueryable) && x.IsGenericMethod)
                .MakeGenericMethod(type);

            return (IQueryable)method.Invoke(this, null);
        }

        public IQueryable<T> GetQueryable<T>() => _session.Query<T>();

        public void Dispose() => _session.Dispose();

        private static ISessionFactory CreateSessionFactory()
            => Fluently.Configure()
              .Database(MsSqlConfiguration.MsSql2012.ConnectionString(c => c.FromConnectionStringWithKey("ConnectionString")))
              .Mappings(m => m.FluentMappings.AddFromAssembly(typeof(NHContext).Assembly))
              .BuildSessionFactory();
    }
}
