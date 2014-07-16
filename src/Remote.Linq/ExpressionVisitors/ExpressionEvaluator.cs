using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Remote.Linq
{
    /// <summary>  
    /// Enables the partial evalutation of queries.  
    /// From http://msdn.microsoft.com/en-us/library/bb546158.aspx  
    /// </summary>  
    internal static class ExpressionEvaluator
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
            return expression.NodeType != ExpressionType.Parameter;
        }

        /// <summary>  
        /// Evaluates & replaces sub-trees when first candidate is reached (top-down)  
        /// </summary>  
        private class SubtreeEvaluator : ExpressionVisitorBase
        {
            private HashSet<Expression> candidates;

            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
            }

            internal Expression Eval(Expression exp)
            {
                return this.Visit(exp);
            }

            protected override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }
                if (this.candidates.Contains(exp))
                {
                    return this.Evaluate(exp);
                }
                return base.Visit(exp);
            }

            private Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                {
                    return e;
                }
                LambdaExpression lambda = Expression.Lambda(e);
                Delegate fn = lambda.Compile();
                return Expression.Constant(fn.DynamicInvoke(null), e.Type);
            }
        }

        /// <summary>  
        /// Performs bottom-up analysis to determine which nodes can possibly  
        /// be part of an evaluated sub-tree.  
        /// </summary>  
        private class Nominator : ExpressionVisitorBase
        {
            private Func<Expression, bool> fnCanBeEvaluated;
            private HashSet<Expression> candidates;
            private bool cannotBeEvaluated;

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal HashSet<Expression> Nominate(Expression expression)
            {
                this.candidates = new HashSet<Expression>();
                this.Visit(expression);
                return this.candidates;
            }

            protected override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = this.cannotBeEvaluated;
                    this.cannotBeEvaluated = false;
                    base.Visit(expression);
                    if (!this.cannotBeEvaluated)
                    {
                        if (this.fnCanBeEvaluated(expression))
                        {
                            this.candidates.Add(expression);
                        }
                        else
                        {
                            this.cannotBeEvaluated = true;
                        }
                    }
                    this.cannotBeEvaluated |= saveCannotBeEvaluated;
                }
                return expression;
            }
        }
    }
}