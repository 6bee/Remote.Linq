// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionExecution.ExpressionExecutionContext;

using Aqua.Dynamic;
using Remote.Linq.ExpressionExecution;
using Remote.Linq.Expressions;
using Shouldly;
using System;
using System.Threading;
using Xunit;
using static TestExpressionExecutionContext;

public class When_using_expression_execution_context_fluent_decorator : IDisposable
{
    private readonly TestExpressionExecutionContext _context = new();
    private int _callBackCount;

    [Fact]
    public void Should_call_prepare_remote_expression_decorator()
        => _context.With(PrepareRemoteExpressionStrategy).Execute();

    [Fact]
    public void Should_call_transform_remote_expression_decorator()
        => _context.With(TransformRemoteExpressionStrategy).Execute();

    [Fact]
    public void Should_call_prepare_system_expression_decorator()
        => _context.With(PrepareSystemExpressionStrategy).Execute();

    [Fact]
    public void Should_call_execute_expression_decorator()
        => _context.With(ExecuteStrategy).Execute();

    [Fact]
    public void Should_call_process_execution_result_decorator()
        => _context.With(ProcessExecutionResultStrategy).Execute();

    [Fact]
    public void Should_call_convert_execution_result_decorator()
        => _context.With(ConvertExecutionResultStrategy).Execute();

    [Fact]
    public void Should_call_process_converted_result_decorator()
        => _context.With(ProcessConvertedResultStrategy).Execute();

    public void Dispose()
        => _callBackCount.ShouldBe(1, "Decorator should have been invoked exactly once");

    private void Increment()
        => Interlocked.Increment(ref _callBackCount);

    private Expression PrepareRemoteExpressionStrategy(Expression exp)
    {
        Increment();
        return Step1_PreparedRemoteExpression;
    }

    private System.Linq.Expressions.Expression TransformRemoteExpressionStrategy(Expression exp)
    {
        Increment();
        return Step2_InitialTransformedSystemExpression;
    }

    private System.Linq.Expressions.Expression PrepareSystemExpressionStrategy(System.Linq.Expressions.Expression exp)
    {
        Increment();
        return Step3_PreparedSystemExpression;
    }

    private object ExecuteStrategy(System.Linq.Expressions.Expression exp)
    {
        Increment();
        return Step4_ExecutionResult;
    }

    private object ProcessExecutionResultStrategy(object result)
    {
        Increment();
        return Step5_ProcessedResult;
    }

    private DynamicObject ConvertExecutionResultStrategy(object result)
    {
        Increment();
        return Step6_ConvertedResult;
    }

    private DynamicObject ProcessConvertedResultStrategy(DynamicObject result)
    {
        Increment();
        return Step7_FinalResult;
    }
}