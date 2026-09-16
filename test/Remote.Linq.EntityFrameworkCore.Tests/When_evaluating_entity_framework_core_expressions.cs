// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests;

using Aqua.TypeSystem;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class When_evaluating_entity_framework_core_expressions
{
    [Fact]
    public void Should_not_evaluate_ef_functions_and_should_evaluate_regular_expressions()
    {
        var efFunctions = Expression.Property(null, typeof(EF), nameof(EF.Functions));

        ExpressionEvaluator.CanBeEvaluated(efFunctions).ShouldBeFalse();
        ExpressionEvaluator.CanBeEvaluated(Expression.Constant(1)).ShouldBeTrue();
    }

    [Fact]
    public void Should_combine_local_evaluation_predicate_with_ef_evaluator()
    {
        var context = new EntityFrameworkCoreExpressionTranslatorContext((ITypeInfoProvider)null, _ => false);
        var efFunctions = Expression.Property(null, typeof(EF), nameof(EF.Functions));

        context.CanBeEvaluatedLocally(Expression.Constant(1)).ShouldBeFalse();
        context.CanBeEvaluatedLocally(efFunctions).ShouldBeFalse();
    }
}
