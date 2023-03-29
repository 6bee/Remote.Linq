// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Remote.Linq.ExpressionVisitors;
    using RemoteExpression = Remote.Linq.Expressions.Expression;
    using SystemExpression = System.Linq.Expressions.Expression;

    public class ExpressionTranslator : IExpressionTranslator
    {
        private readonly IExpressionToRemoteLinqContext? _context;

        public ExpressionTranslator(IExpressionToRemoteLinqContext? context = null)
            => _context = context;

        /// <summary>
        /// Default procedure to translatest a given <see cref="SystemExpression"/> into a <see cref="RemoteExpression"/>.
        /// </summary>
        public virtual RemoteExpression TranslateExpression(SystemExpression expression)
        {
            expression.AssertNotNull(nameof(expression));
            var slinq1 = PreProcess(expression);
            var rlinq1 = Translate(slinq1);
            var rlinq2 = PostProcess(rlinq1);
            return rlinq2;
        }

        protected virtual SystemExpression PreProcess(SystemExpression expression)
            => expression.SimplifyIncorporationOfRemoteQueryables();

        protected virtual RemoteExpression Translate(SystemExpression expression)
            => expression.ToRemoteLinqExpression(_context);

        protected virtual RemoteExpression PostProcess(RemoteExpression expression)
            => expression
            .ReplaceQueryableByResourceDescriptors(_context?.TypeInfoProvider)
            .ReplaceGenericQueryArgumentsByNonGenericArguments();
    }
}