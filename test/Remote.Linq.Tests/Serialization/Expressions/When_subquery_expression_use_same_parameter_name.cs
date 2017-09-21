// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using Aqua.Dynamic;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Xunit;
    using Expression = Remote.Linq.Expressions.Expression;

    public abstract partial class When_subquery_expression_use_same_parameter_name
    {
        public class NoSerialization : When_subquery_expression_use_same_parameter_name
        {
            public NoSerialization()
                : base(x => x)
            {
            }
        }

        interface IValue
        {
            int Value { get; }
        }

        class A : IValue
        {
            public int Value { get; set; }

            public override bool Equals(object obj) => Value == (obj as A)?.Value;

            public override int GetHashCode() => Value;
        }

        class B : IValue
        {
            public int Value { get; set; }

            public override bool Equals(object obj) => Value == (obj as B)?.Value;

            public override int GetHashCode() => Value;
        }

        private readonly Func<Expression, Func<Type, IQueryable>, IEnumerable<DynamicObject>> _execute;

        public When_subquery_expression_use_same_parameter_name(Func<Expression, Expression> serialize)
        {
            _execute = (expression, queryableProvider) => serialize(expression).Execute(queryableProvider: queryableProvider);
        }

        [Fact]
        public void Parameter_expression_should_be_resolved_by_instance_rather_then_by_name()
        {
            IQueryable<A> localQueryable1 = new[]
            {
                new A { Value = 1 },
                new A { Value = 2 },
                new A { Value = 3 },
                new A { Value = 4 },
            }.AsQueryable();

            IQueryable<B> localQueryable2 = new[]
            {
                new B { Value = 1 },
                new B { Value = 2 },
                new B { Value = 3 },
                new B { Value = 4 },
            }.AsQueryable();

            Func<Type, IQueryable> queryableProvider = t =>
            {
                if (t == typeof(A)) return localQueryable1;
                if (t == typeof(B)) return localQueryable2;
                return null;
            };

            IQueryable<A> remoteQueryable1 = CreateRemoteQueryable<A>(queryableProvider);
            IQueryable<B> remoteQueryable2 = CreateRemoteQueryable<B>(queryableProvider);
            
            A[] localResult = BuildQuery(localQueryable1, localQueryable2).ToArray();
            A[] remoteResult = BuildQuery(remoteQueryable1, remoteQueryable2).ToArray();

            remoteResult.SequenceEqual(localResult).ShouldBeTrue();
        }

        [Fact]
        public void Parameter_expression_should_be_resolved_by_instance_rather_then_by_name2()
        {
            IQueryable<A> localQueryable = new[]
            {
                new A { Value = 1 },
                new A { Value = 2 },
                new A { Value = 3 },
                new A { Value = 4 },
            }.AsQueryable();

            IQueryable<A> remoteQueryable = CreateRemoteQueryable<A>(t => localQueryable);

            A[] localResult = BuildQuery(localQueryable, localQueryable).ToArray();
            A[] remoteResult = BuildQuery(remoteQueryable, remoteQueryable).ToArray();

            remoteResult.SequenceEqual(localResult).ShouldBeTrue();
        }

        private IQueryable<T> CreateRemoteQueryable<T>(Func<Type, IQueryable> queryableProvider)
            => RemoteQueryable.Factory.CreateQueryable<T>(x => _execute(x, queryableProvider));

        private static IQueryable<T1> BuildQuery<T1,T2>(IQueryable<T1> queriable1, IQueryable<T2> queriable2)
            where T1: IValue
            where T2: IValue
        {
            Expression<Func<T2, bool>> subfilter = 
                x => x.Value % 2 == 0;

            Expression<Func<T1, bool>> outerfilter = 
                x => queriable2.Where(subfilter).Where(d => d.Value == x.Value).Any();

            return queriable1.Where(outerfilter);
        }
    }
}