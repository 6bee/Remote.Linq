using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remote.Linq.TestClassLibrary;
using Remote.Linq.UnitTest.Extensions;

namespace Remote.Linq.UnitTest
{
    [TestClass]
    public class ExpressionTranslationTests
    {
        [TestMethod]
        public void TestMethod()
        {
            TranslateAndExecuteAndCompareResult(s => s == "A", "B");
            TranslateAndExecuteAndCompareResult(s => s == "A", "A");
            TranslateAndExecuteAndCompareResult(s => s.Length == 1, "");
            TranslateAndExecuteAndCompareResult(s => s.Length == 1, "A");
            TranslateAndExecuteAndCompareResult(s => s.Length == 1, "AB");
            TranslateAndExecuteAndCompareResult(s => s.Where(c => c == 'a').Sum(c => c) == 3, "");
            TranslateAndExecuteAndCompareResult(s => s.Where(c => c == 'a').Sum(c => c) == 3, "aaa");
            TranslateAndExecuteAndCompareResult(s => s.Where(c => c == 'a').Sum(c => c) == 3, "bbb");
            TranslateAndExecuteAndCompareResult(s => s.Where(c => c == 'a').Sum(c => c) == 3, "abba");

            TranslateAndExecuteAndCompareResult(p => p.Name == "The Never Ending Story", new Product());
            TranslateAndExecuteAndCompareResult(p => p.Name != "The Never Ending Story", new Product());

            var productName = "Constant Product";
            TranslateAndExecuteAndCompareResult(p => p.Name == productName, new Product());
            TranslateAndExecuteAndCompareResult(p => p.Name != productName, new Product());
            TranslateAndExecuteAndCompareResult(p => p.Name.EndsWith(productName), new Product { Name = "This is a constant product name" });
            TranslateAndExecuteAndCompareResult(p => p.Name.IndexOf(productName) > -1, new Product { Name = "This is a constant product name" });
            TranslateAndExecuteAndCompareResult(p => string.Compare(p.Name, productName, true) >= 0, new Product { Name = "This is a constant product name" });

            TranslateAndExecuteAndCompareResult(o => o.Id == 99, new Order());
            TranslateAndExecuteAndCompareResult(o => o.Items.Where(i => i.ProductId == 99).Sum(i => i.TotalAmount) > 0m, new Order());
            TranslateAndExecuteAndCompareResult(o => o.Items.Where(i => i.ProductId == 99).Sum(i => i.TotalAmount) == 0m, new Order());

            TranslateAndExecuteAndCompareResult(n => n == 0, 1.0);
            TranslateAndExecuteAndCompareResult(n => n >= 0, 1.0);
            TranslateAndExecuteAndCompareResult(n => n <= 0, 1.0);
            TranslateAndExecuteAndCompareResult(n => n > 0, 1.0);
            TranslateAndExecuteAndCompareResult(n => n < 0, 1.0);
            TranslateAndExecuteAndCompareResult(n => (n | 1) == 0, 1);
            TranslateAndExecuteAndCompareResult(n => (n & 1) == 0, 1);

            var numbers = new[] { -1.0, -0.1, 0, 0.1, 0.2, 0.9, 1.0, 1.000000000001 };
            TranslateAndExecuteAndCompareResult(n => numbers.Contains(n), 1.0);
            TranslateAndExecuteAndCompareResult(n => numbers.Where(i => i < 0).Sum() == n, -1.1); // note: this test fails with DataContractSerializer as double[] 'is not expected'
        }

        private static void TranslateAndExecuteAndCompareResult<T>(Expression<Func<T, bool>> originalLinqExpression, T value)
        {
            TestByClone<T>(originalLinqExpression, value);
            TestByCloneWithNetDataContractSerializer<T>(originalLinqExpression, value);
            //TestByCloneWithDataContractSerializer<T>(originalLinqExpression, value);
        }

        private static void TestByClone<T>(Expression<Func<T, bool>> originalLinqExpression, T value)
        {
            var queryExpression = originalLinqExpression.ToRemoteLinqExpression();
            var queryExpressionClone = queryExpression.Clone();
            var recreatedLinqExpression = queryExpressionClone.ToLinqExpression<T, bool>();
            var result1 = originalLinqExpression.Compile()(value);
            var result2 = recreatedLinqExpression.Compile()(value);
            Assert.AreEqual(result1, result2, "[Clone] lambda expression was not transleted correclty: [{0}]", originalLinqExpression);
        }

        private static void TestByCloneWithDataContractSerializer<T>(Expression<Func<T, bool>> originalLinqExpression, T value)
        {
            var queryExpression = originalLinqExpression.ToRemoteLinqExpression();
            var queryExpressionClone = queryExpression.CloneWithDataContractSerializer();
            var recreatedLinqExpression = queryExpressionClone.ToLinqExpression<T, bool>();
            var result1 = originalLinqExpression.Compile()(value);
            var result2 = recreatedLinqExpression.Compile()(value);
            Assert.AreEqual(result1, result2, "[DataContractSerializer] lambda expression was not transleted correclty: [{0}]", originalLinqExpression);
        }

        private static void TestByCloneWithNetDataContractSerializer<T>(Expression<Func<T, bool>> originalLinqExpression, T value)
        {
            var queryExpression = originalLinqExpression.ToRemoteLinqExpression();
            var queryExpressionClone = queryExpression.CloneWithNetDataContractSerializer();
            var recreatedLinqExpression = queryExpressionClone.ToLinqExpression<T, bool>();
            var result1 = originalLinqExpression.Compile()(value);
            var result2 = recreatedLinqExpression.Compile()(value);
            Assert.AreEqual(result1, result2, "[NetDataContractSerializer] lambda expression was not transleted correclty: [{0}]", originalLinqExpression);
        }
    }
}
