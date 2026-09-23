// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionVisitors.ExpressionTranslator;

using System.Linq.Expressions;
using RemoteLinq = Remote.Linq.Expressions;

public class When_translating_complex_expression_nodes : ExpressionTranslatorTestBase
{
    private static readonly ThreadLocal<int> _sideEffectCounter = new(() => 0);

    private class ComplexContainer
    {
        public int Number { get; set; }

        public NestedContainer Nested { get; set; } = new();

        public List<int> Values { get; set; } = [];
    }

    private class NestedContainer
    {
        public string Name { get; set; }
    }

    [Fact]
    public void Should_roundtrip_switch_with_default_and_multiple_test_values()
    {
        var p = Expression.Parameter(typeof(int), "p");
        var switchExpression = Expression.Switch(
            p,
            defaultBody: Expression.Constant(-1),
            cases: [
                Expression.SwitchCase(
                    Expression.Constant(100),
                    [Expression.Constant(1), Expression.Constant(2)]),
            ]);

        var remote = switchExpression.ToRemoteLinqExpression();
        remote.NodeType.ShouldBe(RemoteLinq.ExpressionType.Switch);
        remote.ShouldBeAssignableTo<RemoteLinq.SwitchExpression>()
            .With(s =>
            {
                s.Cases!.Count.ShouldBe(1);
                s.Cases[0]!.TestValues!.Count.ShouldBe(2);
            });

        var (original, roundTrip) = BackAndForth(Expression.Lambda<Func<int, int>>(switchExpression, p));
        var originalFunc = original.Compile();
        var roundTripFunc = roundTrip.Compile();

        foreach (var input in new[] { 0, 1, 2, 3 })
        {
            var expected = input switch
            {
                1 => 100,
                2 => 100,
                _ => -1,
            };
            originalFunc(input).ShouldBe(expected);
            roundTripFunc(input).ShouldBe(expected);
        }
    }

    [Fact]
    public void Should_roundtrip_try_catch_finally_and_fault_expressions()
    {
        var recordSideEffect = typeof(When_translating_complex_expression_nodes).GetMethod(nameof(RecordSideEffect), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;

        // Catch with filter
        var p = Expression.Parameter(typeof(int), "p");
        var exceptionParameter = Expression.Parameter(typeof(Exception), "ex");
        var body = Expression.Block(
            Expression.IfThen(Expression.Equal(p, Expression.Constant(0)), Expression.Throw(Expression.Constant(new DivideByZeroException()))),
            Expression.IfThen(Expression.Equal(p, Expression.Constant(10)), Expression.Throw(Expression.Constant(new InvalidOperationException()))),
            p);
        var filter = Expression.TypeIs(exceptionParameter, typeof(DivideByZeroException));
        var catchBlock = Expression.MakeCatchBlock(
            typeof(Exception),
            exceptionParameter,
            Expression.Block(Expression.Call(null, recordSideEffect), Expression.Constant(0)),
            filter);
        var catchExpression = Expression.MakeTry(typeof(int), body, @finally: null, fault: null, [catchBlock]);

        _sideEffectCounter.Value = 0;
        var (originalCatch, roundTripCatch) = BackAndForth(Expression.Lambda<Func<int, int>>(catchExpression, p));
        var originalCatchFunc = originalCatch.Compile();
        var roundTripCatchFunc = roundTripCatch.Compile();
        originalCatchFunc(0).ShouldBe(0);
        roundTripCatchFunc(0).ShouldBe(0);
        originalCatchFunc(5).ShouldBe(5);
        roundTripCatchFunc(5).ShouldBe(5);
        Should.Throw<InvalidOperationException>(() => originalCatchFunc(10));
        Should.Throw<InvalidOperationException>(() => roundTripCatchFunc(10));
        _sideEffectCounter.Value.ShouldBe(2);

        // Finally
        var p2 = Expression.Parameter(typeof(int), "p2");
        var finallyBody = Expression.Block(
            Expression.IfThen(Expression.Equal(p2, Expression.Constant(0)), Expression.Throw(Expression.Constant(new DivideByZeroException()))),
            p2);
        var finallyExpression = Expression.MakeTry(
            typeof(int),
            finallyBody,
            @finally: Expression.Call(null, recordSideEffect),
            fault: null,
            handlers: null);

        _sideEffectCounter.Value = 0;
        var (originalFinally, roundTripFinally) = BackAndForth(Expression.Lambda<Func<int, int>>(finallyExpression, p2));
        var originalFinallyFunc = originalFinally.Compile();
        var roundTripFinallyFunc = roundTripFinally.Compile();
        originalFinallyFunc(3).ShouldBe(3);
        roundTripFinallyFunc(3).ShouldBe(3);
        Should.Throw<DivideByZeroException>(() => originalFinallyFunc(0));
        Should.Throw<DivideByZeroException>(() => roundTripFinallyFunc(0));

        // The finally block executes on every invocation, including the ones that complete normally,
        // so the side effect is recorded once per call: 4 calls -> 4 side effects.
        _sideEffectCounter.Value.ShouldBe(4);

        // Fault
        var p3 = Expression.Parameter(typeof(int), "p3");
        var faultBody = Expression.IfThen(Expression.Equal(p3, Expression.Constant(0)), Expression.Throw(Expression.Constant(new InvalidOperationException())));
        var faultExpression = Expression.MakeTry(
            typeof(void),
            faultBody,
            @finally: null,
            fault: Expression.Block(Expression.Call(null, recordSideEffect)),
            handlers: null);

        _sideEffectCounter.Value = 0;
        var (originalFault, roundTripFault) = BackAndForth(Expression.Lambda<Action<int>>(faultExpression, p3));
        var originalFaultFunc = originalFault.Compile();
        var roundTripFaultFunc = roundTripFault.Compile();
        originalFaultFunc(5);
        roundTripFaultFunc(5);
        Should.Throw<InvalidOperationException>(() => originalFaultFunc(0));
        Should.Throw<InvalidOperationException>(() => roundTripFaultFunc(0));
        _sideEffectCounter.Value.ShouldBe(2);
    }

    [Fact]
    public void Should_roundtrip_new_array_bounds_and_initializers()
    {
        var initExpression = Expression.NewArrayInit(typeof(int), Expression.Constant(7), Expression.Constant(8), Expression.Constant(9));
        var (originalInit, roundTripInit) = BackAndForth(Expression.Lambda<Func<int[]>>(initExpression));
        originalInit.Compile().Invoke().ShouldBe([7, 8, 9]);
        roundTripInit.Compile().Invoke().ShouldBeSequenceEqual(originalInit.Compile().Invoke());

        var boundsExpression = Expression.NewArrayBounds(typeof(int), Expression.Constant(3), Expression.Constant(2));
        var (originalBounds, roundTripBounds) = BackAndForth(Expression.Lambda<Func<int[,]>>(boundsExpression));
        originalBounds.Compile().Invoke().ShouldBeOfType<int[,]>()
            .With(a =>
            {
                a.GetLength(0).ShouldBe(3);
                a.GetLength(1).ShouldBe(2);
            });
        roundTripBounds.Compile().Invoke().ShouldBeOfType<int[,]>()
            .With(a =>
            {
                a.GetLength(0).ShouldBe(3);
                a.GetLength(1).ShouldBe(2);
            });
    }

    [Fact]
    public void Should_roundtrip_member_init_assignment_member_and_list_bindings()
    {
        var addMethod = typeof(List<int>).GetMethod(nameof(List<>.Add), [typeof(int)])!;
        var numberProperty = typeof(ComplexContainer).GetProperty(nameof(ComplexContainer.Number))!;
        var nestedProperty = typeof(ComplexContainer).GetProperty(nameof(ComplexContainer.Nested))!;
        var valuesProperty = typeof(ComplexContainer).GetProperty(nameof(ComplexContainer.Values))!;
        var nameProperty = typeof(NestedContainer).GetProperty(nameof(NestedContainer.Name))!;

        var newExpression = Expression.New(typeof(ComplexContainer));
        var bindings = new MemberBinding[]
        {
            Expression.Bind(numberProperty, Expression.Constant(42)),
            Expression.MemberBind(nestedProperty, [Expression.Bind(nameProperty, Expression.Constant("nested"))]),
            Expression.ListBind(
                valuesProperty,
                [
                    Expression.ElementInit(addMethod, Expression.Constant(1)),
                    Expression.ElementInit(addMethod, Expression.Constant(2)),
                ]),
        };
        var memberInitExpression = Expression.MemberInit(newExpression, bindings);

        // Note: the remote expression is not required to mirror the source structure.
        // Self-contained subtrees may be locally evaluated or wrapped by the translator
        // to preserve semantics across serialization, so only behavioral equivalence
        // of the roundtripped expression is asserted here.
        var (original, roundTrip) = BackAndForth(Expression.Lambda<Func<ComplexContainer>>(memberInitExpression));
        var originalResult = original.Compile().Invoke();
        var roundTripResult = roundTrip.Compile().Invoke();

        foreach (var result in new[] { originalResult, roundTripResult })
        {
            result.Number.ShouldBe(42);
            result.Nested.Name.ShouldBe("nested");
            result.Values.ShouldBe([1, 2]);
        }
    }

    [Fact]
    public void Should_roundtrip_conditional_if_then_label_goto_and_loop()
    {
        var recordSideEffect = typeof(When_translating_complex_expression_nodes).GetMethod(nameof(RecordSideEffect), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;

        // IfThen
        var p = Expression.Parameter(typeof(int), "p");
        var ifThenExpression = Expression.IfThen(Expression.GreaterThan(p, Expression.Constant(5)), Expression.Call(null, recordSideEffect));
        _sideEffectCounter.Value = 0;
        var (originalIfThen, roundTripIfThen) = BackAndForth(Expression.Lambda<Action<int>>(ifThenExpression, p));
        originalIfThen.Compile().Invoke(6);
        originalIfThen.Compile().Invoke(3);
        roundTripIfThen.Compile().Invoke(6);
        roundTripIfThen.Compile().Invoke(3);
        _sideEffectCounter.Value.ShouldBe(2);

        // Goto/label
        var p2 = Expression.Parameter(typeof(int), "p2");
        var target = Expression.Label(typeof(string), "res");
        var gotoExpression = Expression.Block(
            Expression.IfThen(Expression.GreaterThan(p2, Expression.Constant(0)), Expression.Goto(target, Expression.Constant("pos"))),
            Expression.IfThen(Expression.LessThan(p2, Expression.Constant(0)), Expression.Goto(target, Expression.Constant("neg"))),
            Expression.Goto(target, Expression.Constant("zero")),
            Expression.Label(target, Expression.Constant("default")));
        var (originalGoto, roundTripGoto) = BackAndForth(Expression.Lambda<Func<int, string>>(gotoExpression, p2));
        var originalGotoFunc = originalGoto.Compile();
        var roundTripGotoFunc = roundTripGoto.Compile();
        originalGotoFunc(1).ShouldBe("pos");
        roundTripGotoFunc(1).ShouldBe("pos");
        originalGotoFunc(-1).ShouldBe("neg");
        roundTripGotoFunc(-1).ShouldBe("neg");
        originalGotoFunc(0).ShouldBe("zero");
        roundTripGotoFunc(0).ShouldBe("zero");

        // Bounded loop
        var counter = Expression.Variable(typeof(int), "i");
        var sum = Expression.Variable(typeof(int), "sum");
        var continueTarget = Expression.Label("continue");
        var breakTarget = Expression.Label("break");
        var loopBody = Expression.Block(
            Expression.Assign(sum, Expression.Add(sum, counter)),
            Expression.IfThen(Expression.GreaterThanOrEqual(counter, Expression.Constant(4)), Expression.Goto(breakTarget)),
            Expression.Assign(counter, Expression.Add(counter, Expression.Constant(1))),
            Expression.Goto(continueTarget));
        var loopExpression = Expression.Loop(loopBody, breakTarget, continueTarget);
        var loopBlockExpression = Expression.Block([counter, sum], loopExpression, sum);
        var (originalLoop, roundTripLoop) = BackAndForth(Expression.Lambda<Func<int>>(loopBlockExpression));
        originalLoop.Compile().Invoke().ShouldBe(10);
        roundTripLoop.Compile().Invoke().ShouldBe(10);
    }

    [Fact]
    public void Should_preserve_shared_parameter_and_label_identity()
    {
        var p = Expression.Parameter(typeof(int), "p");
        var sharedParameterExpression = Expression.Equal(
            Expression.Add(p, p),
            Expression.Multiply(p, Expression.Constant(2)));
        var (original, roundTrip) = BackAndForth(Expression.Lambda<Func<int, bool>>(sharedParameterExpression, p));
        var body = (BinaryExpression)roundTrip.Body;
        var leftParameters = new[] { ((BinaryExpression)body.Left).Left, ((BinaryExpression)body.Left).Right };
        var rightParameter = ((BinaryExpression)body.Right).Left;
        leftParameters[0].ShouldBeSameAs(leftParameters[1]);
        leftParameters[1].ShouldBeSameAs(rightParameter);
        var roundTripFunc = roundTrip.Compile();
        for (var i = -3; i <= 3; i++)
        {
            original.Compile().Invoke(i).ShouldBeTrue();
            roundTripFunc(i).ShouldBeTrue();
        }

        var p2 = Expression.Parameter(typeof(int), "p2");
        var sharedLabelTarget = Expression.Label(typeof(string), "res");
        var sharedLabelExpression = Expression.Block(
            Expression.IfThen(Expression.GreaterThan(p2, Expression.Constant(0)), Expression.Goto(sharedLabelTarget, Expression.Constant("pos"))),
            Expression.IfThen(Expression.LessThan(p2, Expression.Constant(0)), Expression.Goto(sharedLabelTarget, Expression.Constant("neg"))),
            Expression.Goto(sharedLabelTarget, Expression.Constant("zero")),
            Expression.Label(sharedLabelTarget, Expression.Constant("default")));
        var (originalLabel, roundTripLabel) = BackAndForth(Expression.Lambda<Func<int, string>>(sharedLabelExpression, p2));
        var originalLabelFunc = originalLabel.Compile();
        var roundTripLabelFunc = roundTripLabel.Compile();
        originalLabelFunc(1).ShouldBe("pos");
        roundTripLabelFunc(1).ShouldBe("pos");
        originalLabelFunc(-1).ShouldBe("neg");
        roundTripLabelFunc(-1).ShouldBe("neg");
        originalLabelFunc(0).ShouldBe("zero");
        roundTripLabelFunc(0).ShouldBe("zero");
    }

    private static void RecordSideEffect()
        => _sideEffectCounter.Value++;
}
