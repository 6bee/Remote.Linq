// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract]
    [KnownType(typeof(PropertyAccessExpression))]
    [KnownType(typeof(ConstantValueExpression))]
    [KnownType(typeof(ConversionExpression))]
    [KnownType(typeof(BinaryExpression))]
    [KnownType(typeof(UnaryExpression))]
    [KnownType(typeof(CollectionExpression))]
    [KnownType(typeof(UnaryOperator))]
    [KnownType(typeof(BinaryOperator))]
    public abstract class Expression
    {
        public abstract ExpressionType NodeType { get; }

        #region Factory methods

        public static PropertyAccessExpression PropertyAccess(PropertyInfo propertyInfo, PropertyAccessExpression parent = null)
        {
            return new PropertyAccessExpression(propertyInfo, parent);
        }
        
        public static ConstantValueExpression ConstantValue(object value)
        {
            return new ConstantValueExpression(value);
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
        
        public static CollectionExpression Collection(IEnumerable<ConstantValueExpression> list)
        {
            return new CollectionExpression(list);
        }

        public static SortExpression Sort(Expression operand, SortDirection sortDirection)
        {
            return new SortExpression(operand, sortDirection);
        }

        #endregion Factory methods
    }
}
