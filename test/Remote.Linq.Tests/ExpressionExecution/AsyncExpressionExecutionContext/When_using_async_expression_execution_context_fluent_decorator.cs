// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionExecution.AsyncExpressionExecutionContext;

using Aqua.Dynamic;
using Remote.Linq.ExpressionExecution;
using Remote.Linq.Expressions;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static AsyncTestExpressionExecutionContext;

public class When_using_async_expression_execution_context_fluent_decorator : IDisposable
{
    private readonly AsyncTestExpressionExecutionContext _context = new();
    private int _callBackCount;

    [Fact]
    public async Task Should_call_prepare_remote_expression_decorator()
        => await _context.With(PrepareRemoteExpressionStrategy).ExecuteAsync();

    [Fact]
    public async Task Should_call_transform_remote_expression_decorator()
        => await _context.With(TransformRemoteExpressionStrategy).ExecuteAsync();

    [Fact]
    public async Task Should_call_prepare_system_expression_decorator()
        => await _context.With(PrepareSystemExpressionStrategy).ExecuteAsync();

    [Fact]
    public async Task Should_call_prepare_system_expression_for_async_execution_decorator()
        => await _context.With(PrepareSystemExpressionForAsyncExecutionStrategy).ExecuteAsync();

    [Fact]
    public void Should_call_execute_expression_decorator()
        => _context.With(ExecuteStrategy).Execute();

    [Fact]
    public async Task Should_call_async_execute_expression_decorator()
        => await _context.With(ExecuteAsyncStrategy).ExecuteAsync();

    [Fact]
    public async Task Should_call_process_execution_result_decorator()
        => await _context.With(ProcessExecutionResultStrategy).ExecuteAsync();

    [Fact]
    public async Task Should_call_convert_execution_result_decorator()
        => await _context.With(ConvertExecutionResultStrategy).ExecuteAsync();

    [Fact]
    public async Task Should_call_process_converted_result_decorator()
        => await _context.With(ProcessConvertedResultStrategy).ExecuteAsync();

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

    private System.Linq.Expressions.Expression PrepareSystemExpressionForAsyncExecutionStrategy(System.Linq.Expressions.Expression exp, CancellationToken ct)
    {
        Increment();
        return Step4_PreparedForAsyncExpression;
    }

    private object ExecuteStrategy(System.Linq.Expressions.Expression exp)
    {
        Increment();
        return Step5_ExecutionResult;
    }

    private ValueTask<object> ExecuteAsyncStrategy(System.Linq.Expressions.Expression exp, CancellationToken ct)
    {
        Increment();
        return new(Step5_ExecutionResult);
    }

    private object ProcessExecutionResultStrategy(object result)
    {
        Increment();
        return Step6_ProcessedResult;
    }

    private DynamicObject ConvertExecutionResultStrategy(object result)
    {
        Increment();
        return Step7_ConvertedResult;
    }

    private DynamicObject ProcessConvertedResultStrategy(DynamicObject result)
    {
        Increment();
        return Step8_FinalResult;
    }
}