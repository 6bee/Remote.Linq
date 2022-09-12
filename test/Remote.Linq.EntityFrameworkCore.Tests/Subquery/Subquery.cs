using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

public sealed class Subquery
{
    [Fact]
    public void Test()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<EfContext>();
        //dbContextOptionsBuilder.UseSqlServer(@"Server=.;Database=SubqueryDB;User Id=Demo;Password=demo(!)Password;trustServerCertificate=true;");
        dbContextOptionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        using var context = new EfContext(dbContextOptionsBuilder.Options);

        //context.Products.Add(new Product { Id = 101, Name = "Apple" });
        //context.Products.Add(new Product { Id = 102, Name = "Pear" });
        //context.Products.Add(new Product { Id = 103, Name = "Pineapple" });
        //context.Products.Add(new Product { Id = 104, Name = "Car" });
        //context.Products.Add(new Product { Id = 105, Name = "Bicycle" });

        //context.OrderItems.Add(new OrderItem { Id = 10001, ProductId = 101, Quantity = 2 });
        //context.OrderItems.Add(new OrderItem { Id = 10002, ProductId = 102, Quantity = 3 });
        //context.OrderItems.Add(new OrderItem { Id = 10003, ProductId = 105, Quantity = 3 });

        //context.SaveChanges();

        IQueryable<Product> productQueryable = context.Products;
        IQueryable<OrderItem> orderItemQueryable = context.OrderItems;

        // - - - - - - - - - - - - -  - - -
        // LINQ to EF (successful)
        var maxQueryLinq2Ef = productQueryable.Where(p => p.Id == orderItemQueryable.Max(i => i.ProductId));
        // EXPRESSION:
        // .Call System.Linq.Queryable.Where(
        //     .Extension<Microsoft.EntityFrameworkCore.Query.QueryRootExpression>,
        //     '(.Lambda #Lambda1<System.Func`2[Product,System.Boolean]>))
        // 
        // .Lambda #Lambda1<System.Func`2[Product,System.Boolean]>(Product $p) {
        //     $p.Id == .Call System.Linq.Queryable.Max(
        //         .Constant<Subquery+<>c__DisplayClass3_0>(Subquery+<>c__DisplayClass3_0).orderItemQueryable,
        //         '(.Lambda #Lambda2<System.Func`2[OrderItem,System.Int32]>))
        // }
        // 
        // .Lambda #Lambda2<System.Func`2[OrderItem,System.Int32]>(OrderItem $i) {
        //     $i.ProductId
        // }

        var maxResultLinq2Ef = maxQueryLinq2Ef.ToList();
        // SQL:
        // SELECT [p].[Id], [p].[Name]
        // FROM [Products] AS [p]
        // WHERE [p].[Id] = (
        //     SELECT MAX([o].[ProductId])
        //     FROM [OrderItems] AS [o])

        var productQueryableExp = Expression.Constant(productQueryable);
        var orderItemQueryableExp = Expression.Constant(orderItemQueryable);

        var whereMethod = typeof(System.Linq.Queryable)
            .GetMethods()
            .Where(m => m.Name == nameof(System.Linq.Queryable.Where))
            .Where(m =>
            {
                var genericArguments = m.GetGenericArguments();
                var parameters = m.GetParameters();
                if (genericArguments.Length == 1 && parameters.Length == 2)
                {
                    var entity = genericArguments[0];
                    var predicate = parameters[1];
                    var predicateType = typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(entity, typeof(bool)));
                    return predicate.ParameterType == predicateType;
                }

                return false;
            })
            .Single()
            .MakeGenericMethod(typeof(Product));

        var toListMethod = typeof(System.Linq.Enumerable)
            .GetMethod(nameof(System.Linq.Enumerable.ToList))
            .MakeGenericMethod(typeof(Product));

        var productParameterExp = Expression.Parameter(typeof(Product), "p");
        var productIdProperty = typeof(Product).GetProperty(nameof(Product.Id));
        var productIdExp = Expression.MakeMemberAccess(productParameterExp, productIdProperty);

        // - - - - - - - - - - - - -  - - -
        // DYNAMIC EXPRESSION (execution successful)
        // var oneResult = productQueryable
        //     .Where(p => p.Id == 105)
        //     .ToList();
        //
        var oneValueExp = Expression.Constant(105);
        var compareOneExp = Expression.MakeBinary(ExpressionType.Equal, productIdExp, oneValueExp);
        var lambdaOneExp = Expression.Lambda<Func<Product, bool>>(compareOneExp, productParameterExp);
        var whereOneExp = Expression.Call(whereMethod, productQueryableExp, lambdaOneExp);
        // EXPRESSION:
        // .Call System.Linq.Queryable.Where(
        //     .Constant<Microsoft.EntityFrameworkCore.Internal.InternalDbSet`1[Product]>(Microsoft.EntityFrameworkCore.Internal.InternalDbSet`1[Product]),
        //     '(.Lambda #Lambda1<System.Func`2[Product,System.Boolean]>))
        // 
        // .Lambda #Lambda1<System.Func`2[Product,System.Boolean]>(Product $p) {
        //     $p.Id == 105
        // }

        var oneToListExp = Expression.Call(toListMethod, whereOneExp);
        var oneResult = Expression.Lambda(oneToListExp).Compile().DynamicInvoke();
        // SQL:
        // SELECT [p].[Id], [p].[Name]
        // FROM [Products] AS [p]
        // WHERE [p].[Id] = 105

        // - - - - - - - - - - - - -  - - -
        // DYNAMIC EXPRESSION (execution failing)
        // var maxResult = productQueryable
        //     .Where(p => p.Id == orderItemQueryable.Max(i => i.ProductId))
        //     .ToList();
        //
        var orderItemParameterExp = Expression.Parameter(typeof(OrderItem), "i");
        var orderItemProductIdProperty = typeof(OrderItem).GetProperty(nameof(OrderItem.ProductId));
        var orderItemProductIdExp = Expression.MakeMemberAccess(orderItemParameterExp, orderItemProductIdProperty);
        var maxMethod = typeof(System.Linq.Queryable)
            .GetMethods()
            .Where(m => m.Name == nameof(System.Linq.Queryable.Max))
            .Where(m =>
            {
                var genericArguments = m.GetGenericArguments();
                var parameters = m.GetParameters();
                if (genericArguments.Length == 2 && parameters.Length == 2)
                {
                    var entityType = genericArguments[0];
                    var resultType = genericArguments[1];
                    var predicate = parameters[1];
                    var predicateType = typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(entityType, resultType));
                    return predicate.ParameterType == predicateType;
                }

                return false;
            })
            .Single()
            .MakeGenericMethod(typeof(OrderItem), typeof(int));
        var lambdaOrderItemProcutIdExp = Expression.Lambda<Func<OrderItem, int>>(orderItemProductIdExp, orderItemParameterExp);
        var maxExp = Expression.Call(maxMethod, orderItemQueryableExp, lambdaOrderItemProcutIdExp);

        var compareMaxExp = Expression.MakeBinary(ExpressionType.Equal, productIdExp, maxExp);
        var lambdaMaxExp = Expression.Lambda<Func<Product, bool>>(compareMaxExp, productParameterExp);

        var whereMaxExp = Expression.Call(whereMethod, productQueryableExp, lambdaMaxExp);
        // EXPRESSION:
        // .Call System.Linq.Queryable.Where(
        //     .Constant<Microsoft.EntityFrameworkCore.Internal.InternalDbSet`1[Product]>(Microsoft.EntityFrameworkCore.Internal.InternalDbSet`1[Product]),
        //     '(.Lambda #Lambda1<System.Func`2[Product,System.Boolean]>))
        // 
        // .Lambda #Lambda1<System.Func`2[Product,System.Boolean]>(Product $p) {
        //     $p.Id == .Call System.Linq.Queryable.Max(
        //         .Constant<Microsoft.EntityFrameworkCore.Internal.InternalDbSet`1[OrderItem]>(Microsoft.EntityFrameworkCore.Internal.InternalDbSet`1[OrderItem]),
        //         '(.Lambda #Lambda2<System.Func`2[OrderItem,System.Int32]>))
        // }
        // 
        // .Lambda #Lambda2<System.Func`2[OrderItem,System.Int32]>(OrderItem $i) {
        //     $i.ProductId
        // }

        var toListMaxExp = Expression.Call(toListMethod, whereMaxExp);
        var maxResult = Expression.Lambda(toListMaxExp).Compile().DynamicInvoke(); // this line throws exception
        // EXCEPTION:
        // 
        // Message:
        //   System.InvalidOperationException : The LINQ expression 'InternalDbSet<OrderItem> { OrderItem { Id = 10001, ProductId = 101, Quantity = 2 }, OrderItem { Id = 10002, ProductId = 102, Quantity = 3 }, OrderItem { Id = 10003, ProductId = 105, Quantity = 3 } }
        //       .Max(i => i.ProductId)' could not be translated. Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly by inserting a call to 'AsEnumerable', 'AsAsyncEnumerable', 'ToList', or 'ToListAsync'. See https://go.microsoft.com/fwlink/?linkid=2101038 for more information.
        // 
        // Stack Trace:
        //   NavigationExpandingExpressionVisitor.VisitMethodCall(MethodCallExpression methodCallExpression)
        //   MethodCallExpression.Accept(ExpressionVisitor visitor)
        //   ExpressionVisitor.Visit(Expression node)
        //   ExpressionVisitor.VisitBinary(BinaryExpression node)
        //   BinaryExpression.Accept(ExpressionVisitor visitor)
        //   ExpressionVisitor.Visit(Expression node)
        //   NavigationExpandingExpressionVisitor.ExpandNavigationsForSource(NavigationExpansionExpression source, Expression expression)
        //   NavigationExpandingExpressionVisitor.ProcessLambdaExpression(NavigationExpansionExpression source, LambdaExpression lambdaExpression)
        //   NavigationExpandingExpressionVisitor.ProcessWhere(NavigationExpansionExpression source, LambdaExpression predicate)
        //   NavigationExpandingExpressionVisitor.VisitMethodCall(MethodCallExpression methodCallExpression)
        //   MethodCallExpression.Accept(ExpressionVisitor visitor)
        //   ExpressionVisitor.Visit(Expression node)
        //   NavigationExpandingExpressionVisitor.Expand(Expression query)
        //   QueryTranslationPreprocessor.Process(Expression query)
        //   QueryCompilationContext.CreateQueryExecutor[TResult](Expression query)
        //   Database.CompileQuery[TResult](Expression query, Boolean async)
        //   QueryCompiler.CompileQueryCore[TResult](IDatabase database, Expression query, IModel model, Boolean async)
        //   <>c__DisplayClass9_0`1.<Execute>b__0()
        //   CompiledQueryCache.GetOrAddQuery[TResult](Object cacheKey, Func`1 compiler)
        //   QueryCompiler.Execute[TResult](Expression query)
        //   EntityQueryProvider.Execute[TResult](Expression expression)
        //   EntityQueryable`1.GetEnumerator()
        //   List`1.ctor(IEnumerable`1 collection)
        //   Enumerable.ToList[TSource](IEnumerable`1 source)
    }

    public class EfContext : DbContext
    {
        public EfContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasKey(x => x.Id);
            modelBuilder.Entity<OrderItem>().HasKey(x => x.Id);
        }

        public DbSet<Product> Products { get; init; }

        public DbSet<OrderItem> OrderItems { get; init; }
    }

    public sealed record OrderItem
    {
        public int Id { get; init; }
        public int ProductId { get; init; }
        public int Quantity { get; set; }
    }

    public sealed record Product
    {
        public int Id { get; init; }
        public string Name { get; set; }
    }
}