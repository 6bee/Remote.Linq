// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using Aqua.Dynamic;
    using Remote.Linq;
    using Remote.Linq.ExpressionExecution;
    using Shouldly;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Xunit;
    using Expression = Remote.Linq.Expressions.Expression;

    public abstract class When_subquery_expression_use_same_parameter_name
    {
        public class With_binary_formatter : When_subquery_expression_use_same_parameter_name
        {
            public With_binary_formatter()
                : base(BinarySerializationHelper.Serialize)
            {
            }
        }

        public class With_data_contract_serializer : When_subquery_expression_use_same_parameter_name
        {
            public With_data_contract_serializer()
                : base(DataContractSerializationHelper.Serialize)
            {
            }
        }

        public class With_newtonsoft_json_serializer : When_subquery_expression_use_same_parameter_name
        {
            public With_newtonsoft_json_serializer()
                : base(x => (Expression)NewtonsoftJsonSerializationHelper.Serialize(x, x.GetType()))
            {
            }
        }

        public class With_system_text_json_serializer : When_subquery_expression_use_same_parameter_name
        {
            public With_system_text_json_serializer()
                : base(x => (Expression)SystemTextJsonSerializationHelper.Serialize(x, x.GetType()))
            {
            }
        }

#if NETFRAMEWORK
        public class With_net_data_contract_serializer : When_subquery_expression_use_same_parameter_name
        {
            public With_net_data_contract_serializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
#endif // NETFRAMEWORK

        public class With_protobuf_net_serializer : When_using_LoopExpressions
        {
            public With_protobuf_net_serializer()
                : base(ProtobufNetSerializationHelper.Serialize)
            {
            }
        }

        public class With_xml_serializer : When_subquery_expression_use_same_parameter_name
        {
            public With_xml_serializer()
                : base(XmlSerializationHelper.Serialize)
            {
            }
        }

        private interface IValue
        {
            int Value { get; }
        }

        private class A : IValue
        {
            public int Value { get; set; }

            public override bool Equals(object obj) => Value == (obj as A)?.Value;

            public override int GetHashCode() => Value;
        }

        private class B : IValue
        {
            public int Value { get; set; }

            public override bool Equals(object obj) => Value == (obj as B)?.Value;

            public override int GetHashCode() => Value;
        }

        private readonly Func<Expression, Func<Type, IQueryable>, DynamicObject> _execute;

        protected When_subquery_expression_use_same_parameter_name(Func<Expression, Expression> serialize)
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
                if (t == typeof(A))
                {
                    return localQueryable1;
                }

                if (t == typeof(B))
                {
                    return localQueryable2;
                }

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

        private static IQueryable<T1> BuildQuery<T1, T2>(IQueryable<T1> queriable1, IQueryable<T2> queriable2)
            where T1 : IValue
            where T2 : IValue
        {
            Expression<Func<T2, bool>> subfilter =
                x => x.Value % 2 == 0;

            Expression<Func<T1, bool>> outerfilter =
                x => queriable2.Where(subfilter).Where(d => d.Value == x.Value).Any();

            return queriable1.Where(outerfilter);
        }
    }
}