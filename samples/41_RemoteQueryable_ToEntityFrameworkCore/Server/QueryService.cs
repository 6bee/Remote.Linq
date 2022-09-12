// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.EntityFrameworkCore.Query.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Remote.Linq.EntityFrameworkCore;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class QueryService
    {
        public async ValueTask<DynamicObject> ExecuteQueryAsync(Expression queryExpression, CancellationToken cancellation)
        {
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                using var efContext = new EFContext();

                ////var prod = (IQueryable<ProductGroup>)new QueryableSetProvider(efContext).GetQueryableSet(typeof(ProductGroup));
                ////var items = (IQueryable<OrderItem>)new QueryableSetProvider(efContext).GetQueryableSet(typeof(OrderItem));

                ////var prod = efContext.Set<ProductGroup>();
                ////var items = efContext.Set<OrderItem>();

                var prod = efContext.ProductGroups;
                var items = efContext.OrderItems;
                var q = from i in items
                        where i.Id == prod.Max(p => p.Id)
                        select i;
                var r = await q.ToListAsync();



                ////new QueryRootExpression(efContext, default(IEntityType) !);
                ////new QueryCompilationContext(new QueryCompilationContextDependencies(efContext.Model, ))

                ////var serviceProvider = ((IInfrastructure<IServiceProvider>)efContext).Instance;

                ////var queryContext = serviceProvider
                ////    .GetService<IQueryContextFactory>()
                ////    .Create();

                ////var compilationContext = serviceProvider
                ////    .GetService<IQueryCompilationContextFactory>()
                ////    .Create(async: false);

                ////var f = compilationContext.CreateQueryExecutor<IEnumerable<OrderItem>>(q.Expression);

                ////var r2 = f(queryContext);

                ////var queryCompiler = (QueryCompiler)serviceProvider.GetService<IQueryCompiler>();

                ////linqExpression = queryCompiler.ExtractParameters(
                ////    linqExpression,
                ////    queryContext,
                ////    dbContext.GetService<IDiagnosticsLogger<DbLoggerCategory.Query>>());

                ////linqExpression = dbContext
                ////    .GetService<IQueryTranslationPreprocessorFactory>()
                ////    .Create(compilationContext)
                ////    .Process(linqExpression);

                ////ShapedQueryExpression queryExpression = (ShapedQueryExpression)dbContext
                ////    .GetService<IQueryableMethodTranslatingExpressionVisitorFactory>()
                ////    .Create(dbContext.Model)
                ////    .Visit(linqExpression);

                ////queryExpression = (ShapedQueryExpression)dbContext
                ////    .GetService<IQueryTranslationPostprocessorFactory>()
                ////    .Create(compilationContext)
                ////    .Process(queryExpression);

                ////return ((SelectExpression)queryExpression.QueryExpression, queryContext.ParameterValues);

                return await queryExpression.ExecuteWithEntityFrameworkCoreAsync(efContext, cancellation).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw;
            }
#pragma warning restore CS0168 // Variable is declared but never used
        }
    }
}