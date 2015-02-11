// Copyright (c) Christof Senn. All rights reserved. 

namespace WcfService
{
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using NHibernate;
    using NHibernate.Linq;
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
            var method = typeof(NHContext).GetMethods()
                .Single(x => x.Name == "GetQueryable" && x.IsGenericMethod)
                .MakeGenericMethod(type);

            return (IQueryable)method.Invoke(this, null);
        }

        public IQueryable<T> GetQueryable<T>()
        {
            return _session.Query<T>();
        }

        public void Dispose()
        {
            _session.Dispose();
        }

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
              .Database(MsSqlConfiguration.MsSql2012.ConnectionString(c => c.FromConnectionStringWithKey("ConnectionString")))
              .Mappings(m => m.FluentMappings.AddFromAssembly(typeof(NHContext).Assembly))
              .BuildSessionFactory();
        }
    }
}
