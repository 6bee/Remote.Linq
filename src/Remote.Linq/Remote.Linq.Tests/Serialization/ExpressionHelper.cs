namespace Remote.Linq.Tests.Serialization
{
    using Xunit;

    public static class ExpressionHelper
    {
        public static void EqualsRemoteExpression<T>(this T expression1, T expression2) where T : Remote.Linq.Expressions.Expression
        {
            var expression1String = expression1.ToString();
            var expression2String = expression2.ToString();

            Assert.Equal(expression1String, expression2String);
        }

        public static void EqualsExpression<T>(this T expression1, T expression2) where T : System.Linq.Expressions.Expression
        {
            var expression1String = expression1.ToString();
            var expression2String = expression2.ToString();

            Assert.Equal(expression1String, expression2String);
        }
    }
}
