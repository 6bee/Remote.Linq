// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using System.ComponentModel;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static partial class ExpressionTranslator
    {
        private sealed class ResultWrapperExpression : SystemLinq.Expression
        {
            public ResultWrapperExpression(RemoteLinq.Expression result, Type type)
            {
                Result = result;
                Type = type;
            }

            public RemoteLinq.Expression Result { get; }

            public override Type Type { get; }

            public override bool CanReduce => false;

            public override SystemLinq.ExpressionType NodeType => (SystemLinq.ExpressionType)(-1);

            protected override SystemLinq.Expression Accept(SystemLinq.ExpressionVisitor visitor) => throw Exception;

            protected override SystemLinq.Expression VisitChildren(SystemLinq.ExpressionVisitor visitor) => throw Exception;

            public override SystemLinq.Expression Reduce() => throw Exception;

            private static Exception Exception => throw new RemoteLinqException($"{nameof(ResultWrapperExpression)} is meant for internal usage and must not be exposed externally.");
        }

        // TODO: make private and use as default
        public static IExpressionToRemoteLinqContext NoMappingContext => new NoMappingContextImpl();

        private sealed class NoMappingContextImpl : IExpressionToRemoteLinqContext, IDynamicObjectMapper
        {
            public ITypeInfoProvider TypeInfoProvider => new TypeInfoProvider();

            public Func<object, bool> NeedsMapping => _ => false;

            public IDynamicObjectMapper ValueMapper => this;

            public Func<SystemLinq.Expression, bool>? CanBeEvaluatedLocally => _ => false;

            object? IDynamicObjectMapper.Map(DynamicObject? obj, Type? targetType) => throw NotSupportedException;

            DynamicObject? IDynamicObjectMapper.MapObject(object? obj, Func<Type, bool>? setTypeInformation) => throw NotSupportedException;

            private NotSupportedException NotSupportedException => new NotSupportedException("operation must not be called as no value mapping should occure");
        }

        /// <summary>
        /// Translates a given expression into a remote linq expression.
        /// </summary>
        public static RemoteLinq.Expression ToRemoteLinqExpression(this SystemLinq.Expression expression, IExpressionToRemoteLinqContext? context = null)
            => new SystemToRemoteLinqTranslator(context ?? new ExpressionTranslatorContext()).ToRemoteExpression(expression);

        /// <summary>
        /// Translates a given expression into a remote linq expression.
        /// </summary>
        [Obsolete("Method will be removed in futur version, use overload instaed.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static RemoteLinq.Expression ToRemoteLinqExpression(this SystemLinq.Expression expression, ITypeInfoProvider? typeInfoProvider, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => ToRemoteLinqExpression(expression, GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Translates a given lambda expression into a remote linq expression.
        /// </summary>
        public static RemoteLinq.LambdaExpression ToRemoteLinqExpression(this SystemLinq.LambdaExpression expression, IExpressionToRemoteLinqContext? context = null)
        {
            var lambdaExpression = new SystemToRemoteLinqTranslator(context ?? new ExpressionTranslatorContext()).ToRemoteExpression(expression);
            return (RemoteLinq.LambdaExpression)lambdaExpression;
        }

        /// <summary>
        /// Translates a given lambda expression into a remote linq expression.
        /// </summary>
        [Obsolete("Method will be removed in futur version, use overload instaed.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static RemoteLinq.LambdaExpression ToRemoteLinqExpression(this SystemLinq.LambdaExpression expression, ITypeInfoProvider? typeInfoProvider, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => ToRemoteLinqExpression(expression, GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Translates a given remote linq expression into an system linq expression.
        /// </summary>
        public static SystemLinq.Expression ToLinqExpression(this RemoteLinq.Expression expression, IExpressionFromRemoteLinqContext? context = null)
            => new RemoteToSystemLinqTranslator(context).ToExpression(expression);

        /// <summary>
        /// Translates a given remote linq expression into an system linq expression.
        /// </summary>
        public static SystemLinq.Expression ToLinqExpression(this RemoteLinq.Expression expression, ITypeResolver? typeResolver)
            => ToLinqExpression(expression, GetExpressionTranslatorContextOrNull(typeResolver));

        /// <summary>
        /// Translates a given query expression into a lambda expression.
        /// </summary>
        public static SystemLinq.Expression<Func<T, TResult>> ToLinqExpression<T, TResult>(this RemoteLinq.LambdaExpression expression)
        {
            var exp = expression.ToLinqExpression();
            var lambdaExpression = SystemLinq.Expression.Lambda<Func<T, TResult>>(exp.Body, exp.Parameters);
            return lambdaExpression;
        }

        /// <summary>
        /// Translates a given query expression into a lambda expression.
        /// </summary>
        public static SystemLinq.Expression<Func<TResult>> ToLinqExpression<TResult>(this RemoteLinq.LambdaExpression expression)
        {
            var exp = expression.ToLinqExpression();
            var lambdaExpression = SystemLinq.Expression.Lambda<Func<TResult>>(exp.Body, exp.Parameters);
            return lambdaExpression;
        }

        /// <summary>
        /// Translates a given remote linq expression into a lambda expression.
        /// </summary>
        public static SystemLinq.LambdaExpression ToLinqExpression(this RemoteLinq.LambdaExpression expression, IExpressionFromRemoteLinqContext? context = null)
            => (SystemLinq.LambdaExpression)new RemoteToSystemLinqTranslator(context).ToExpression(expression);

        /// <summary>
        /// Translates a given remote linq expression into a lambda expression.
        /// </summary>
        public static SystemLinq.LambdaExpression ToLinqExpression(this RemoteLinq.LambdaExpression expression, ITypeResolver? typeResolver)
            => ToLinqExpression(expression, GetExpressionTranslatorContextOrNull(typeResolver));

        private static SystemLinq.ExpressionType ToExpressionType(this RemoteLinq.BinaryOperator binaryOperator)
            => (SystemLinq.ExpressionType)(int)binaryOperator;

        private static SystemLinq.ExpressionType ToExpressionType(this RemoteLinq.UnaryOperator unaryOperator)
            => (SystemLinq.ExpressionType)(int)unaryOperator;

        private static RemoteLinq.BinaryOperator ToBinaryOperator(this SystemLinq.ExpressionType expressionType)
            => (RemoteLinq.BinaryOperator)(int)expressionType;

        private static RemoteLinq.UnaryOperator ToUnaryOperator(this SystemLinq.ExpressionType expressionType)
            => (RemoteLinq.UnaryOperator)(int)expressionType;

        private static RemoteLinq.NewArrayType ToNewArrayType(this SystemLinq.ExpressionType expressionType)
            => (RemoteLinq.NewArrayType)(int)expressionType;

        private static SystemLinq.GotoExpressionKind ToGotoExpressionKind(this RemoteLinq.GotoExpressionKind kind)
            => (SystemLinq.GotoExpressionKind)(int)kind;

        private static RemoteLinq.GotoExpressionKind ToGotoExpressionKind(this SystemLinq.GotoExpressionKind kind)
            => (RemoteLinq.GotoExpressionKind)(int)kind;

        private static ResultWrapperExpression Wrap<T>(this T expression)
            where T : RemoteLinq.Expression
            => new ResultWrapperExpression(expression, typeof(T));

        /// <summary>
        /// Unwraps the resulting <see cref="RemoteLinq.Expression"/>. This method throws if expression is not an <see cref="SystemLinq.ConstantExpression"/> holding the expected type.
        /// </summary>
        private static RemoteLinq.Expression Unwrap(this SystemLinq.Expression? expression)
            => expression is ResultWrapperExpression resultWrapperExpression
            ? resultWrapperExpression.Result
            : throw new RemoteLinqException($"implementation error: expression is expected to be {nameof(SystemLinq.ConstantExpression)} but was {expression?.NodeType.ToString() ?? "<null>."}");

        /// <summary>
        /// Unwraps the resulting <see cref="RemoteLinq.Expression"/>. This method throws if expression is not an <see cref="SystemLinq.ConstantExpression"/> holding the expected type.
        /// </summary>
        private static T Unwrap<T>(this SystemLinq.Expression? expression)
            where T : RemoteLinq.Expression
            => (T)Unwrap(expression);

        /// <summary>
        /// Unwraps the resulting <see cref="RemoteLinq.Expression"/>. The expression may be null.
        /// </summary>
        private static RemoteLinq.Expression? UnwrapNullable(this SystemLinq.Expression? expression)
            => expression is ResultWrapperExpression resultWrapperExpression
            ? resultWrapperExpression.Result
            : null;

        private static IExpressionTranslatorContext? GetExpressionTranslatorContextOrNull(ITypeResolver? typeResolver)
            => typeResolver is null
            ? null
            : new ExpressionTranslatorContext(typeResolver);

        private static IExpressionTranslatorContext? GetExpressionTranslatorContextOrNull(ITypeInfoProvider? typeInfoProvider, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally)
            => typeInfoProvider is null && canBeEvaluatedLocally is null
            ? null
            : new ExpressionTranslatorContext(typeInfoProvider, canBeEvaluatedLocally);
    }
}