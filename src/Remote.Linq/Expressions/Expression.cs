// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    [KnownType(typeof(ParameterExpression))]
    [KnownType(typeof(MethodCallExpression))]
    [KnownType(typeof(PropertyAccessExpression))]
    [KnownType(typeof(ConstantExpression))]
    [KnownType(typeof(ConversionExpression))]
    [KnownType(typeof(BinaryExpression))]
    [KnownType(typeof(UnaryExpression))]
    [KnownType(typeof(CollectionExpression))]
    [KnownType(typeof(UnaryOperator))]
    [KnownType(typeof(BinaryOperator))]
    [KnownType(typeof(LambdaExpression))]
    public abstract partial class Expression
    {
        public abstract ExpressionType NodeType { get; }

        #region Factory methods

        public static PropertyAccessExpression PropertyAccess(Expression instance, PropertyInfo propertyInfo)
        {
            return new PropertyAccessExpression(instance, propertyInfo);
        }

        public static PropertyAccessExpression PropertyAccess(Expression instance, string propertyName, Type propertyType, Type declaringType)
        {
            return new PropertyAccessExpression(instance, propertyName, propertyType, declaringType);
        }

        public static MethodCallExpression MethodCall(Expression insatnce, MethodInfo methodInfo, IEnumerable<Expression> arguments)
        {
            return new MethodCallExpression(insatnce, methodInfo, arguments);
        }

        public static ConstantExpression Constant(object value)
        {
            return new ConstantExpression(value);
        }

        public static ConstantExpression Constant(object value, Type type)
        {
            return new ConstantExpression(value, type);
        }

        public static ConversionExpression Conversion(Expression operand, Type type)
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

        #endregion Factory methods
    }
}
