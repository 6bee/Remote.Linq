// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using Remote.Linq.Expressions;
    using System;
    using Xunit;

    public abstract partial class When_using_complex_expression_with_local_lambda
    {
        private readonly LambdaExpression _remoteExpression;

        private readonly LambdaExpression _serializedRemoteExpression;

        protected When_using_complex_expression_with_local_lambda(Func<LambdaExpression, LambdaExpression> serialize)
        {
            Func<object, string> sufix = (x) => x + "ending";

            System.Linq.Expressions.Expression<Func<bool, bool>> expression = x => sufix("test").Length > 10;

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
            var exp1 = _remoteExpression.ToLinqExpression<bool, bool>();
            var exp2 = _serializedRemoteExpression.ToLinqExpression<bool, bool>();

            exp1.EqualsExpression(exp2);
        }
    }
}