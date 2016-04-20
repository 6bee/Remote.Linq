// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using Remote.Linq.Expressions;
    using System;
    using Xunit;

    public abstract partial class When_using_simple_projection_to_single_member
    {
        public class AType
        {
            public string Value { get; set; }
        }

        private LambdaExpression _remoteExpression;

        private LambdaExpression _serializedRemoteExpression;

        protected When_using_simple_projection_to_single_member(Func<LambdaExpression, LambdaExpression> serialize)
        {
            System.Linq.Expressions.Expression<Func<AType, string>> expression = x => x.Value;

            _remoteExpression = expression.ToRemoteLinqExpression();
            
            _serializedRemoteExpression = serialize(_remoteExpression);
        }

        [Fact]
        public void Remote_expression_should_be_equal()
        {
            var str1 = _remoteExpression.ToString();
            var str2 = _serializedRemoteExpression.ToString();

            _remoteExpression.EqualsRemoteExpression(_serializedRemoteExpression);
        }

        [Fact]
        public void System_expresison_should_be_equal()
        {
            var exp1 = _remoteExpression.ToLinqExpression<AType, string>();
            var exp2 = _serializedRemoteExpression.ToLinqExpression<AType, string>();

            exp1.EqualsExpression(exp2);
        }
    }
}