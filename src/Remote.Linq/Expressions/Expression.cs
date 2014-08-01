// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    [KnownType(typeof(BinaryExpression))]
    [KnownType(typeof(CollectionExpression))]
    [KnownType(typeof(ConditionalExpression))]
    [KnownType(typeof(ConstantExpression))]
    [KnownType(typeof(ConversionExpression))]
    [KnownType(typeof(LambdaExpression))]
    [KnownType(typeof(ListInitExpression))]
    [KnownType(typeof(MemberExpression))]
    [KnownType(typeof(MemberInitExpression))]
    [KnownType(typeof(MethodCallExpression))]
    [KnownType(typeof(NewExpression))]
    [KnownType(typeof(NewArrayExpression))]
    [KnownType(typeof(ParameterExpression))]
    [KnownType(typeof(UnaryExpression))]
    [KnownType(typeof(UnaryOperator))]
    [KnownType(typeof(Remote.Linq.Dynamic.QueryableResourceDescriptor))]
    public abstract partial class Expression
    {
        public abstract ExpressionType NodeType { get; }

        #region Factory methods

        public static MemberExpression MakeMemberAccess(Expression expression, Remote.Linq.TypeSystem.MemberInfo member)
        {
            return new MemberExpression(expression, member);
        }

        public static MemberExpression MakeMemberAccess(Expression expression, System.Reflection.MemberInfo member)
        {
            return new MemberExpression(expression, member);
        }

        public static MethodCallExpression MethodCall(Expression insatnce, MethodInfo methodInfo, IEnumerable<Expression> arguments)
        {
            return new MethodCallExpression(insatnce, methodInfo, arguments);
        }

        public static ConditionalExpression Conditional(Expression test, Expression ifTrue, Expression ifFalse)
        {
            return new ConditionalExpression(test, ifTrue, ifFalse);
        }

        public static ConstantExpression Constant(object value)
        {
            return new ConstantExpression(value);
        }

        public static ConstantExpression Constant(object value, Type type)
        {
            return new ConstantExpression(value, type);
        }

        public static ConversionExpression Convert(Expression operand, Type type)
        {
            return new ConversionExpression(operand, type);
        }

        public static ParameterExpression Parameter(string parameterName, Type type)
        {
            return new ParameterExpression(parameterName, type);
        }

        public static BinaryExpression Binary(Expression leftOperand, Expression rightOperand, BinaryOperator @operator)
        {
            return new BinaryExpression(leftOperand, rightOperand, @operator);
        }

        public static BinaryExpression Binary(Expression leftOperand, Expression rightOperand, BinaryOperator @operator, bool liftToNull, System.Reflection.MethodInfo method, LambdaExpression conversion = null)
        {
            return new BinaryExpression(leftOperand, rightOperand, @operator, liftToNull, method, conversion);
        }

        public static UnaryExpression Unary(Expression operand, UnaryOperator @operator)
        {
            return new UnaryExpression(operand, @operator);
        }

        public static CollectionExpression Collection(IEnumerable<ConstantExpression> list, Type elementType)
        {
            return new CollectionExpression(list, elementType);
        }

        public static SortExpression Sort(LambdaExpression operand, SortDirection sortDirection)
        {
            return new SortExpression(operand, sortDirection);
        }

        public static LambdaExpression Lambda(Expression expression, IEnumerable<ParameterExpression> parameters)
        {
            return new LambdaExpression(expression, parameters);
        }

        public static LambdaExpression Lambda(Expression expression, params ParameterExpression[] parameters)
        {
            return Lambda(expression, (IEnumerable<ParameterExpression>)parameters);
        }

        public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments)
        {
            return new NewExpression(constructor, arguments);
        }

        public static NewExpression New(ConstructorInfo constructor, params Expression[] arguments)
        {
            return New(constructor, (IEnumerable<Expression>)arguments);
        }

        public static ListInitExpression ListInit(NewExpression expression, IEnumerable<ElementInit> initializers)
        {
            return new ListInitExpression(expression, initializers);
        }

        public static MemberInitExpression MemberInit(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
        {
            return new MemberInitExpression(newExpression, bindings);
        }

        public static MemberInitExpression MemberInit(NewExpression newExpression, params MemberBinding[] bindings)
        {
            return MemberInit(newExpression, (IEnumerable<MemberBinding>)bindings);
        }

        public static MemberAssignment Bind(TypeSystem.MemberInfo member, Expression expression)
        {
            return new MemberAssignment(member, expression);
        }

        public static MemberMemberBinding MemberBind(TypeSystem.MemberInfo member, IEnumerable<MemberBinding> bindings)
        {
            return new MemberMemberBinding(member, bindings);
        }

        public static MemberListBinding ListBind(TypeSystem.MemberInfo member, IEnumerable<ElementInit> initializers)
        {
            return new MemberListBinding(member, initializers);
        }

        public static ElementInit ElementInit(TypeSystem.MethodInfo addMethod, System.Collections.ObjectModel.ReadOnlyCollection<Expression> arguments)
        {
            return new ElementInit(addMethod, arguments);
        }

        public static ElementInit ElementInit(System.Reflection.MethodInfo addMethod, IEnumerable<Expression> arguments)
        {
            return new ElementInit(addMethod, arguments);
        }

        // TODO: replace binding flags by bool flags        
        internal static ElementInit ElementInit(string methodName, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes, IEnumerable<Expression> arguments)
        {
            return new ElementInit(methodName, declaringType, bindingFlags, genericArguments, parameterTypes, arguments);
        }

        #endregion Factory methods
    }
}
