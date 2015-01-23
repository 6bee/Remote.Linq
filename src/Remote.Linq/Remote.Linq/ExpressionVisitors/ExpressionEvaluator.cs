namespace Remote.Linq.ExpressionVisitors
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;

    /// <summary>  
    /// Enables the partial evalutation of queries.  
    /// From http://msdn.microsoft.com/en-us/library/bb546158.aspx  
    /// </summary>  
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionEvaluator
    {
        /// <summary>  
        /// Performs evaluation & replacement of independent sub-trees  
        /// </summary>  
        /// <param name="expression">The root of the expression tree.</param>  
        /// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>  
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>  
        public static Expression PartialEval(this Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
        }

        /// <summary>  
        /// Performs evaluation & replacement of independent sub-trees  
        /// </summary>  
        /// <param name="expression">The root of the expression tree.</param>  
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>  
        public static Expression PartialEval(this Expression expression)
        {
            return PartialEval(expression, ExpressionEvaluator.CanBeEvaluatedLocally);
        }

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Parameter)
            {
                return false;
            }

            var methodCallExpression = expression as MethodCallExpression;
            if (!ReferenceEquals(null, methodCallExpression))
            {
                if (methodCallExpression.Method.IsGenericMethod &&
                    methodCallExpression.Method.GetGenericMethodDefinition() == MethodInfos.QueryFuntion.Include)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>  
        /// Evaluates & replaces sub-trees when first candidate is reached (top-down)  
        /// </summary>  
        private sealed class SubtreeEvaluator : ExpressionVisitorBase
        {
            private readonly HashSet<Expression> _candidates;

            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                _candidates = candidates;
            }

            internal Expression Eval(Expression expression)
            {
                var expression2 = Visit(expression);
                return expression2;
            }

            protected override Expression Visit(Expression expression)
            {
                if (expression == null)
                {
                    return null;
                }

                if (_candidates.Contains(expression))
                {
                    return Evaluate(expression);
                }
                
                return base.Visit(expression);
            }

            private Expression Evaluate(Expression expression)
            {
                if (expression.NodeType == ExpressionType.Constant)
                {
                    return expression;
                }

                if (expression.NodeType == ExpressionType.Quote)
                {
                    return expression;
                }
                
                LambdaExpression lambda = Expression.Lambda(expression);
                Delegate fn = lambda.Compile();
                var value = fn.DynamicInvoke(null);
                return Expression.Constant(value, expression.Type);
            }
        }

        /// <summary>  
        /// Performs bottom-up analysis to determine which nodes can possibly  
        /// be part of an evaluated sub-tree.  
        /// </summary>  
        private sealed class Nominator : ExpressionVisitorBase
        {
            private Func<Expression, bool> _fnCanBeEvaluated;
            private HashSet<Expression> _candidates;
            private bool _cannotBeEvaluated;

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                _fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal HashSet<Expression> Nominate(Expression expression)
            {
                _candidates = new HashSet<Expression>();
                Visit(expression);
                return _candidates;
            }

            protected override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = _cannotBeEvaluated;
                    _cannotBeEvaluated = false;
                    
                    base.Visit(expression);
                    
                    if (!_cannotBeEvaluated)
                    {
                        if (_fnCanBeEvaluated(expression))
                        {
                            _candidates.Add(expression);
                        }
                        else
                        {
                            _cannotBeEvaluated = true;
                        }
                    }
                    
                    _cannotBeEvaluated |= saveCannotBeEvaluated;
                }

                return expression;
            }
        }
    }
}