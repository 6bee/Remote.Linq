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
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
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

        /// <summary>
        /// Combines two predicates with boolean AND. In case of one predicate is null, the other is returned without being combined.
        /// </summary>
        public static Func<T, bool>? And<T>(this Func<T, bool>? predicate1, Func<T, bool>? predicate2)
        {
            if (predicate1 is null)
            {
                return predicate2;
            }

            if (predicate2 is null)
            {
                return predicate1;
            }

            return x => predicate1(x) && predicate2(x);
        }

        /// <summary>
        /// Combines two predicates with boolean OR. In case of one predicate is null, the other is returned without being combined.
        /// </summary>
        public static Func<T, bool>? Or<T>(this Func<T, bool>? predicate1, Func<T, bool>? predicate2)
        {
            if (predicate1 is null)
            {
                return predicate2;
            }

            if (predicate2 is null)
            {
                return predicate1;
            }

            return x => predicate1(x) || predicate2(x);
        }

        /// <summary>
        /// Translates a given expression into a remote linq expression.
        /// </summary>
        public static RemoteLinq.Expression ToRemoteLinqExpression(this SystemLinq.Expression expression, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => new SystemToRemoteLinqTranslator(typeInfoProvider, canBeEvaluatedLocally).ToRemoteExpression(expression);

        /// <summary>
        /// Translates a given lambda expression into a remote linq expression.
        /// </summary>
        public static RemoteLinq.LambdaExpression ToRemoteLinqExpression(this SystemLinq.LambdaExpression expression, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
        {
            var lambdaExpression = new SystemToRemoteLinqTranslator(typeInfoProvider, canBeEvaluatedLocally).ToRemoteExpression(expression);
            return (RemoteLinq.LambdaExpression)lambdaExpression;
        }

        /// <summary>
        /// Translates a given query expression into an expression.
        /// </summary>
        public static SystemLinq.Expression ToLinqExpression(this RemoteLinq.Expression expression)
            => ToLinqExpression(expression, null);

        /// <summary>
        /// Translates a given query expression into an expression.
        /// </summary>
        public static SystemLinq.Expression ToLinqExpression(this RemoteLinq.Expression expression, ITypeResolver? typeResolver)
            => new RemoteToSystemLinqTranslator(typeResolver).ToExpression(expression);

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
        /// Translates a given query expression into a lambda expression.
        /// </summary>
        public static SystemLinq.LambdaExpression ToLinqExpression(this RemoteLinq.LambdaExpression expression)
            => ToLinqExpression(expression, null);

        /// <summary>
        /// Translates a given query expression into a lambda expression.
        /// </summary>
        public static SystemLinq.LambdaExpression ToLinqExpression(this RemoteLinq.LambdaExpression expression, ITypeResolver? typeResolver)
            => (SystemLinq.LambdaExpression)new RemoteToSystemLinqTranslator(typeResolver).ToExpression(expression);

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

        private static bool KeepMarkerFunctions(SystemLinq.Expression expression)
        {
            if (expression is SystemLinq.MethodCallExpression methodCallExpression &&
                methodCallExpression.Method.IsGenericMethod &&
                methodCallExpression.Method.GetGenericMethodDefinition() == MethodInfos.QueryFuntion.Include)
            {
                return false;
            }

            return true;
        }

        private sealed class ConstantValueMapper : DynamicObjectMapper
        {
            private static readonly Func<Type, bool> _isPrimitiveType = new[]
                {
                    typeof(string),
                    typeof(int),
                    typeof(uint),
                    typeof(byte),
                    typeof(sbyte),
                    typeof(short),
                    typeof(ushort),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal),
                    typeof(char),
                    typeof(bool),
                    typeof(Guid),
                    typeof(DateTime),
                    typeof(TimeSpan),
                    typeof(DateTimeOffset),
                    typeof(System.Numerics.BigInteger),
                    typeof(System.Numerics.Complex),
                }
                .SelectMany(x => x.IsValueType ? new[] { x, typeof(Nullable<>).MakeGenericType(x) } : new[] { x })
                .ToDictionary(x => x, x => (object?)null)
                .ContainsKey;

            private static readonly Type[] _unmappedTypes = new[]
                {
                    typeof(CancellationToken),
                    typeof(ConstantQueryArgument),
                    typeof(VariableQueryArgument),
                    typeof(VariableQueryArgumentList),
                    typeof(QueryableResourceDescriptor),
                    typeof(VariableQueryArgument<>),
                    typeof(SystemLinq.Expression),
                    typeof(IQueryable),
                    typeof(IRemoteResource),
                };

            private static readonly Type[] _excludeFromUnmappedTypes = new[]
                {
                    typeof(EnumerableQuery),
                };

            private sealed class IsKnownTypeProvider : IIsKnownTypeProvider
            {
                private readonly bool _includePrimitiveType;

                public IsKnownTypeProvider(bool includePrimitiveType)
                {
                    _includePrimitiveType = includePrimitiveType;
                }

                public bool IsKnownType(Type type) => !TypeNeedsWrapping(type, _includePrimitiveType);
            }

            private ConstantValueMapper(ITypeResolver? typeResolver, ITypeInfoProvider? typeInfoProvider, IIsKnownTypeProvider isKnownTypeProvider)
                : base(typeResolver, typeInfoProvider, isKnownTypeProvider: isKnownTypeProvider)
            {
            }

            public static ConstantValueMapper ForSubstitution(ITypeInfoProvider typeInfoProvider)
                => new ConstantValueMapper(null, typeInfoProvider, new IsKnownTypeProvider(true));

            public static ConstantValueMapper ForReconstruction(ITypeResolver typeResolver)
                => new ConstantValueMapper(typeResolver, null, new IsKnownTypeProvider(false));

            public static bool TypeNeedsWrapping(Type type, bool includePrimitiveType = true)
            {
                if (includePrimitiveType && _isPrimitiveType(type))
                {
                    return false;
                }

                if (IsUnmappedType(type))
                {
                    return false;
                }

                return true;
            }

            private static bool IsUnmappedType(Type type)
            {
                var t = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                return _unmappedTypes.Any(x => x.IsAssignableFrom(t))
                    && !_excludeFromUnmappedTypes.Any(x => x.IsAssignableFrom(t));
            }
        }
    }
}