// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.EnumerableExtensions;
    using Aqua.TypeSystem;
    using Aqua.Utils;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    partial class ExpressionTranslationExtensions
    {
        private sealed class SystemToRemoteLinqTranslator : SystemExpressionVisitorBase
        {
            private readonly Dictionary<SystemLinq.ParameterExpression, RemoteLinq.ParameterExpression> _parameterExpressionCache =
                new Dictionary<SystemLinq.ParameterExpression, RemoteLinq.ParameterExpression>(ReferenceEqualityComparer<SystemLinq.ParameterExpression>.Default);

            private readonly Dictionary<SystemLinq.LabelTarget, RemoteLinq.LabelTarget> _labelTargetCache =
                new Dictionary<SystemLinq.LabelTarget, RemoteLinq.LabelTarget>(ReferenceEqualityComparer<SystemLinq.LabelTarget>.Default);

            private readonly Dictionary<object, ConstantQueryArgument> _constantQueryArgumentCache =
                new Dictionary<object, ConstantQueryArgument>(ReferenceEqualityComparer<object>.Default);

            private readonly Func<SystemLinq.Expression, bool>? _canBeEvaluatedLocally;
            private readonly Func<object, bool> _needsMapping;
            private readonly ITypeInfoProvider _typeInfoProvider;
            private readonly IDynamicObjectMapper _dynamicObjectMapper;

            public SystemToRemoteLinqTranslator(IExpressionToRemoteLinqContext expressionTranslatorContext)
            {
                expressionTranslatorContext.AssertNotNull();

                _canBeEvaluatedLocally = expressionTranslatorContext.CanBeEvaluatedLocally;

                _needsMapping = expressionTranslatorContext.NeedsMapping;

                _typeInfoProvider = expressionTranslatorContext.TypeInfoProvider
                    ?? throw new ArgumentException($"{nameof(expressionTranslatorContext.TypeInfoProvider)} property must not be null.", nameof(expressionTranslatorContext));

                _dynamicObjectMapper = expressionTranslatorContext.ValueMapper
                    ?? throw new ArgumentException($"{nameof(expressionTranslatorContext.ValueMapper)} property must not be null.", nameof(expressionTranslatorContext));
            }

            public RemoteLinq.Expression ToRemoteExpression(SystemLinq.Expression expression)
            {
                var partialEvalExpression = expression.CheckNotNull().PartialEval(_canBeEvaluatedLocally);
                var constExpression = Visit(partialEvalExpression);
                return constExpression.Unwrap();
            }

            [return: NotNullIfNotNull("node")]
            protected override SystemLinq.Expression? Visit(SystemLinq.Expression? node)
                => node?.NodeType switch
                {
                    SystemLinq.ExpressionType.New => VisitNew((SystemLinq.NewExpression)node).Wrap(),
                    _ => base.Visit(node),
                };

            protected override SystemLinq.Expression VisitSwitch(SystemLinq.SwitchExpression node)
            {
                var defaultExpression = Visit(node.DefaultBody).UnwrapNullable();
                var switchValue = Visit(node.SwitchValue).Unwrap();
                var cases = (node.Cases ?? Enumerable.Empty<SystemLinq.SwitchCase>()).Select(VisitSwitchCase).ToList();
                return new RemoteLinq.SwitchExpression(switchValue, node.Comparison, defaultExpression, cases).Wrap();
            }

            private new RemoteLinq.SwitchCase VisitSwitchCase(SystemLinq.SwitchCase switchCase)
            {
                var body = Visit(switchCase.Body).Unwrap();
                var testValues = switchCase.TestValues.Select(Visit).Select(Unwrap).ToList();
                return new RemoteLinq.SwitchCase(body, testValues);
            }

            protected override SystemLinq.Expression VisitTry(SystemLinq.TryExpression node)
            {
                var body = Visit(node.Body).Unwrap();
                var fault = Visit(node.Fault).UnwrapNullable();
                var @finally = Visit(node.Finally).UnwrapNullable();
                var handlers = node.Handlers?.Select(VisitCatch);
                return new RemoteLinq.TryExpression(_typeInfoProvider.GetTypeInfo(node.Type), body, fault, @finally, handlers).Wrap();
            }

            private new RemoteLinq.CatchBlock VisitCatch(SystemLinq.CatchBlock catchBlock)
            {
                var body = Visit(catchBlock.Body).Unwrap();
                var filter = Visit(catchBlock.Filter).UnwrapNullable();
                var variable = Visit(catchBlock.Variable).UnwrapNullable() as RemoteLinq.ParameterExpression;
                return new RemoteLinq.CatchBlock(_typeInfoProvider.GetTypeInfo(catchBlock.Test), variable, body, filter);
            }

            protected override SystemLinq.Expression VisitListInit(SystemLinq.ListInitExpression node)
            {
                var n = VisitNew(node.NewExpression);
                var initializers = VisitElementInitializerList(node.Initializers);
                return new RemoteLinq.ListInitExpression(n, initializers).Wrap();
            }

            private new IEnumerable<RemoteLinq.ElementInit> VisitElementInitializerList(ReadOnlyCollection<SystemLinq.ElementInit> original)
                => original.Select(VisitElementInitializer);

            private new RemoteLinq.ElementInit VisitElementInitializer(SystemLinq.ElementInit initializer)
            {
                var arguments = VisitExpressionList(initializer.Arguments).Select(Unwrap);
                return new RemoteLinq.ElementInit(_typeInfoProvider.GetMethodInfo(initializer.AddMethod), arguments);
            }

            private new RemoteLinq.NewExpression VisitNew(SystemLinq.NewExpression node)
            {
                IEnumerable<RemoteLinq.Expression>? arguments = null;
                if (node.Arguments?.Count > 0)
                {
                    arguments = node.Arguments
                        .Select(Visit)
                        .Select(Unwrap);
                }

                return node.Constructor is null
                    ? new RemoteLinq.NewExpression(_typeInfoProvider.GetTypeInfo(node.Type))
                    : new RemoteLinq.NewExpression(_typeInfoProvider.GetConstructorInfo(node.Constructor), arguments, node.Members?.Select(x => _typeInfoProvider.GetMemberInfo(x)));
            }

            protected override SystemLinq.Expression VisitConstant(SystemLinq.ConstantExpression node)
            {
                var type = node.Type;
                var value = node.Value;
                RemoteLinq.ConstantExpression exp;
                if (type is not null && typeof(SystemLinq.Expression).IsAssignableFrom(type) && value is SystemLinq.Expression expressionValue)
                {
                    var expValue = Visit(expressionValue).Unwrap();
                    exp = new RemoteLinq.ConstantExpression(expValue, _typeInfoProvider.GetTypeInfo(type));
                }
                else if (type is not null && typeof(IEnumerable<SystemLinq.Expression>).IsAssignableFrom(type) && value is IEnumerable<SystemLinq.Expression> expressionCollection)
                {
                    var list = VisitExpressionList(new ReadOnlyCollection<SystemLinq.Expression>(expressionCollection.ToArray())).Select(Unwrap<RemoteLinq.Expression>).ToArray();
                    exp = new RemoteLinq.ConstantExpression(list, _typeInfoProvider.GetTypeInfo(type));
                }
                else if (type == typeof(Type) && value is Type typeValue)
                {
                    exp = new RemoteLinq.ConstantExpression(typeValue.AsTypeInfo(), type);
                }
                else if (value is not null && _needsMapping(value))
                {
                    var key = new { value, type };
                    if (!_constantQueryArgumentCache.TryGetValue(key, out var constantQueryArgument))
                    {
                        var dynamicObject = _dynamicObjectMapper.MapObject(value);
                        var copy = new DynamicObject(dynamicObject.Type ?? _typeInfoProvider.GetTypeInfo(type, false, false));

                        foreach (var property in dynamicObject.Properties.AsEmptyIfNull())
                        {
                            var propertyValue = property.Value;
                            if (propertyValue is SystemLinq.Expression expValue)
                            {
                                propertyValue = Visit(expValue).UnwrapNullable();
                            }

                            copy.Add(property.Name, propertyValue);
                        }

                        constantQueryArgument = new ConstantQueryArgument(copy);
                        _constantQueryArgumentCache.Add(key, constantQueryArgument);
                    }

                    exp = type is null
                        ? new RemoteLinq.ConstantExpression(constantQueryArgument)
                        : type == constantQueryArgument.Value.Type?.ToType()
                        ? new RemoteLinq.ConstantExpression(constantQueryArgument, constantQueryArgument.Value.Type)
                        : new RemoteLinq.ConstantExpression(constantQueryArgument, _typeInfoProvider.GetTypeInfo(type));
                }
                else
                {
                    exp = type is null
                        ? new RemoteLinq.ConstantExpression(value)
                        : new RemoteLinq.ConstantExpression(value, _typeInfoProvider.GetTypeInfo(type));
                }

                return exp.Wrap();
            }

            protected override SystemLinq.Expression VisitParameter(SystemLinq.ParameterExpression node)
            {
                lock (_parameterExpressionCache)
                {
                    if (!_parameterExpressionCache.TryGetValue(node, out var exp))
                    {
                        exp = new RemoteLinq.ParameterExpression(_typeInfoProvider.GetTypeInfo(node.Type), node.Name, _parameterExpressionCache.Count + 1);
                        _parameterExpressionCache.Add(node, exp);
                    }

                    return exp.Wrap();
                }
            }

            protected override SystemLinq.Expression VisitBinary(SystemLinq.BinaryExpression node)
            {
                var binaryOperator = node.NodeType.ToBinaryOperator();
                var left = Visit(node.Left).Unwrap();
                var right = Visit(node.Right).Unwrap();
                var conversion = Visit(node.Conversion).UnwrapNullable() as RemoteLinq.LambdaExpression;
                return new RemoteLinq.BinaryExpression(binaryOperator, left, right, node.IsLiftedToNull, _typeInfoProvider.GetMethodInfo(node.Method), conversion).Wrap();
            }

            protected override SystemLinq.Expression VisitTypeIs(SystemLinq.TypeBinaryExpression node)
            {
                var expression = Visit(node.Expression).Unwrap();
                return new RemoteLinq.TypeBinaryExpression(expression, _typeInfoProvider.GetTypeInfo(node.TypeOperand)).Wrap();
            }

            protected override SystemLinq.Expression VisitMemberAccess(SystemLinq.MemberExpression node)
            {
                var instance = Visit(node.Expression).UnwrapNullable();
                return new RemoteLinq.MemberExpression(instance, _typeInfoProvider.GetMemberInfo(node.Member)).Wrap();
            }

            protected override SystemLinq.Expression VisitMemberInit(SystemLinq.MemberInitExpression node)
            {
                var n = VisitNew(node.NewExpression);
                var bindings = VisitBindingList(node.Bindings);
                return new RemoteLinq.MemberInitExpression(n, bindings).Wrap();
            }

            private new IEnumerable<RemoteLinq.MemberBinding> VisitBindingList(ReadOnlyCollection<SystemLinq.MemberBinding> original)
                => original.Select(VisitMemberBinding);

            private new RemoteLinq.MemberBinding VisitMemberBinding(SystemLinq.MemberBinding binding)
                => binding.BindingType switch
                {
                    SystemLinq.MemberBindingType.Assignment => VisitMemberAssignment((SystemLinq.MemberAssignment)binding),
                    SystemLinq.MemberBindingType.MemberBinding => VisitMemberMemberBinding((SystemLinq.MemberMemberBinding)binding),
                    SystemLinq.MemberBindingType.ListBinding => VisitMemberListBinding((SystemLinq.MemberListBinding)binding),
                    _ => throw new NotSupportedException($"Unhandled binding type '{binding.BindingType}'"),
                };

            private new RemoteLinq.MemberAssignment VisitMemberAssignment(SystemLinq.MemberAssignment assignment)
            {
                var expression = Visit(assignment.Expression).Unwrap();
                var member = _typeInfoProvider.GetMemberInfo(assignment.Member);
                return new RemoteLinq.MemberAssignment(member, expression);
            }

            private new RemoteLinq.MemberMemberBinding VisitMemberMemberBinding(SystemLinq.MemberMemberBinding binding)
            {
                var bindings = VisitBindingList(binding.Bindings);
                var m = _typeInfoProvider.GetMemberInfo(binding.Member);
                return new RemoteLinq.MemberMemberBinding(m, bindings);
            }

            private new RemoteLinq.MemberListBinding VisitMemberListBinding(SystemLinq.MemberListBinding binding)
            {
                var initializers = VisitElementInitializerList(binding.Initializers);
                var m = _typeInfoProvider.GetMemberInfo(binding.Member);
                return new RemoteLinq.MemberListBinding(m, initializers);
            }

            protected override SystemLinq.Expression VisitMethodCall(SystemLinq.MethodCallExpression node)
            {
                var instance = Visit(node.Object).UnwrapNullable();
                var arguments = node.Arguments
                    .Select(Visit)
                    .Select(Unwrap);
                return new RemoteLinq.MethodCallExpression(instance, _typeInfoProvider.GetMethodInfo(node.Method), arguments).Wrap();
            }

            protected override SystemLinq.Expression VisitLambda(SystemLinq.LambdaExpression node)
            {
                var body = Visit(node.Body).Unwrap();
                var parameters = node.Parameters
                    .Select(VisitParameter)
                    .Select(Unwrap<RemoteLinq.ParameterExpression>);
                return new RemoteLinq.LambdaExpression(_typeInfoProvider.GetTypeInfo(node.Type), body, parameters).Wrap();
            }

            protected override SystemLinq.Expression VisitUnary(SystemLinq.UnaryExpression node)
            {
                var unaryOperator = node.NodeType.ToUnaryOperator();
                var operand = Visit(node.Operand).Unwrap();
                return new RemoteLinq.UnaryExpression(unaryOperator, operand, _typeInfoProvider.GetTypeInfo(node.Type), _typeInfoProvider.GetMethodInfo(node.Method)).Wrap();
            }

            protected override SystemLinq.Expression VisitConditional(SystemLinq.ConditionalExpression node)
            {
                var test = Visit(node.Test).Unwrap();
                var ifTrue = Visit(node.IfTrue).Unwrap();
                var ifFalse = Visit(node.IfFalse).Unwrap();
                return new RemoteLinq.ConditionalExpression(test, ifTrue, ifFalse).Wrap();
            }

            protected override SystemLinq.Expression VisitNewArray(SystemLinq.NewArrayExpression node)
            {
                var newArrayType = node.NodeType.ToNewArrayType();
                var expressions = VisitExpressionList(node.Expressions).Select(Unwrap);
                var elementType = TypeHelper.GetElementType(node.Type) ?? throw new RemoteLinqException($"Failed to get element type of {node.Type}.");
                return new RemoteLinq.NewArrayExpression(newArrayType, _typeInfoProvider.GetTypeInfo(elementType), expressions).Wrap();
            }

            protected override SystemLinq.Expression VisitInvocation(SystemLinq.InvocationExpression node)
            {
                var expression = Visit(node.Expression).Unwrap();
                var arguments = VisitExpressionList(node.Arguments)?.Select(Unwrap);
                return new RemoteLinq.InvokeExpression(expression, arguments).Wrap();
            }

            protected override SystemLinq.Expression VisitBlock(SystemLinq.BlockExpression node)
            {
                var expressions = VisitExpressionList(node.Expressions)?.Select(Unwrap);
                IEnumerable<RemoteLinq.ParameterExpression>? variables = null;
                if (node.Variables is not null)
                {
                    var nodeVariables = node.Variables.Cast<SystemLinq.Expression>().ToList().AsReadOnly();
                    variables = VisitExpressionList(nodeVariables)?.Select(Unwrap<RemoteLinq.ParameterExpression>);
                }

                var type = node.Type == node.Result.Type ? null : node.Type;
                return new RemoteLinq.BlockExpression(_typeInfoProvider.GetTypeInfo(type), variables, expressions).Wrap();
            }

            protected override SystemLinq.Expression VisitDefault(SystemLinq.DefaultExpression node)
            {
                return new RemoteLinq.DefaultExpression(_typeInfoProvider.GetTypeInfo(node.Type)).Wrap();
            }

            protected override SystemLinq.Expression VisitLabel(SystemLinq.LabelExpression node)
            {
                var target = VisitTarget(node.Target);
                var defaultValue = Visit(node.DefaultValue).UnwrapNullable();
                return new RemoteLinq.LabelExpression(target, defaultValue).Wrap();
            }

            protected override SystemLinq.Expression VisitLoop(SystemLinq.LoopExpression node)
            {
                var body = Visit(node.Body).Unwrap();
                var breakLabel = VisitTarget(node.BreakLabel);
                var continueLabel = VisitTarget(node.ContinueLabel);
                return new RemoteLinq.LoopExpression(body, breakLabel, continueLabel).Wrap();
            }

            protected override SystemLinq.Expression VisitGoto(SystemLinq.GotoExpression node)
            {
                var kind = node.Kind.ToGotoExpressionKind();
                var target = VisitTarget(node.Target);
                var type = node.Target.Type == node.Type ? null : node.Type;
                var value = Visit(node.Value).UnwrapNullable();
                return new RemoteLinq.GotoExpression(kind, target, _typeInfoProvider.GetTypeInfo(type), value).Wrap();
            }

            [return: NotNullIfNotNull("labelTarget")]
            private RemoteLinq.LabelTarget? VisitTarget(SystemLinq.LabelTarget? labelTarget)
            {
                if (labelTarget is null)
                {
                    return null;
                }

                lock (_labelTargetCache)
                {
                    if (!_labelTargetCache.TryGetValue(labelTarget, out var target))
                    {
                        target = new RemoteLinq.LabelTarget(labelTarget.Name, _typeInfoProvider.GetTypeInfo(labelTarget.Type), _labelTargetCache.Count + 1);
                        _labelTargetCache.Add(labelTarget, target);
                    }

                    return target;
                }
            }
        }
    }
}