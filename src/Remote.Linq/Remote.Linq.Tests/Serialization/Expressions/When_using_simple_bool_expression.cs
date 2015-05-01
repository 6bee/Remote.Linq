// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions
{
    using Remote.Linq.Expressions;
    using System;
    using Xunit;

    public class When_using_simple_bool_expression
    {
        private LambdaExpression _remoteExpression;

        private LambdaExpression _serializedRemoteExpression;

        public When_using_simple_bool_expression()
        {
            System.Linq.Expressions.Expression<Func<bool, bool>> expression = x => !x;

            _remoteExpression = expression.ToRemoteLinqExpression();

            // HINT: since this test is used in multiple assemblies as linked file, 
            //       use serialize extension method have context specific serialization applied
            _serializedRemoteExpression = _remoteExpression.SerializeExpression();
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