// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.VariableQueryArgument
{
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public abstract partial class When_using_local_variable_query_string_argument_list
    {
        private class AType
        {
            public string Key { get; set; }
        }

        private LambdaExpression _remoteExpression;

        private LambdaExpression _serializedRemoteExpression;

        protected When_using_local_variable_query_string_argument_list(Func<LambdaExpression, LambdaExpression> serialize)
        {
            var keys = new List<string>() { "K1", "K2" };

            System.Linq.Expressions.Expression<Func<AType, bool>> expression = x => keys.Contains(x.Key);

            _remoteExpression = expression.ToRemoteLinqExpression();

            _serializedRemoteExpression = serialize(_remoteExpression);
        }

        [Fact]
        public void Remote_expression_should_be_equal()
        {
            _remoteExpression.EqualsRemoteExpression(_serializedRemoteExpression);
        }

        [Fact]
        public void System_expresison_should_be_equal()
        {
            var exp1 = _remoteExpression.ToLinqExpression<AType, bool>();
            var exp2 = _serializedRemoteExpression.ToLinqExpression<AType, bool>();

            exp1.EqualsExpression(exp2);
        }
    }
}
