// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [Serializable]
    [DataContract]
    [KnownType(typeof(BinaryExpression)), XmlInclude(typeof(BinaryExpression))]
    [KnownType(typeof(BlockExpression)), XmlInclude(typeof(BlockExpression))]
    [KnownType(typeof(ConditionalExpression)), XmlInclude(typeof(ConditionalExpression))]
    [KnownType(typeof(ConstantExpression)), XmlInclude(typeof(ConstantExpression))]
    [KnownType(typeof(DefaultExpression)), XmlInclude(typeof(DefaultExpression))]
    [KnownType(typeof(GotoExpression)), XmlInclude(typeof(GotoExpression))]
    [KnownType(typeof(InvokeExpression)), XmlInclude(typeof(InvokeExpression))]
    [KnownType(typeof(LabelExpression)), XmlInclude(typeof(LabelExpression))]
    [KnownType(typeof(LambdaExpression)), XmlInclude(typeof(LambdaExpression))]
    [KnownType(typeof(ListInitExpression)), XmlInclude(typeof(ListInitExpression))]
    [KnownType(typeof(LoopExpression)), XmlInclude(typeof(LoopExpression))]
    [KnownType(typeof(MemberExpression)), XmlInclude(typeof(MemberExpression))]
    [KnownType(typeof(MemberInitExpression)), XmlInclude(typeof(MemberInitExpression))]
    [KnownType(typeof(MethodCallExpression)), XmlInclude(typeof(MethodCallExpression))]
    [KnownType(typeof(NewExpression)), XmlInclude(typeof(NewExpression))]
    [KnownType(typeof(NewArrayExpression)), XmlInclude(typeof(NewArrayExpression))]
    [KnownType(typeof(ParameterExpression)), XmlInclude(typeof(ParameterExpression))]
    [KnownType(typeof(SwitchExpression)), XmlInclude(typeof(SwitchExpression))]
    [KnownType(typeof(TryExpression)), XmlInclude(typeof(TryExpression))]
    [KnownType(typeof(TypeBinaryExpression)), XmlInclude(typeof(TypeBinaryExpression))]
    [KnownType(typeof(UnaryExpression)), XmlInclude(typeof(UnaryExpression))]
    public abstract partial class Expression
    {
        public abstract ExpressionType NodeType { get; }
        
        [Obsolete("Use actual constructor instead", false)]
        public static MemberExpression MakeMemberAccess(Expression expression, Aqua.TypeSystem.MemberInfo member)
        {
            return new MemberExpression(expression, member);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static MemberExpression MakeMemberAccess(Expression expression, System.Reflection.MemberInfo member)
        {
            return new MemberExpression(expression, member);
        }

        [Obsolete("Method renamed to Call. This method will be removed in a future version.", true)]
        public static MethodCallExpression MethodCall(Expression insatnce, MethodInfo methodInfo, IEnumerable<Expression> arguments)
            => Call(insatnce, methodInfo, arguments);

        [Obsolete("Use actual constructor instead", false)]
        public static MethodCallExpression Call(Expression insatnce, MethodInfo methodInfo, IEnumerable<Expression> arguments)
        {
            return new MethodCallExpression(insatnce, methodInfo, arguments);
        }

        [Obsolete("Method renamed to Call. This method will be removed in a future version.", true)]
        public static MethodCallExpression MethodCall(Expression insatnce, Aqua.TypeSystem.MethodInfo methodInfo, IEnumerable<Expression> arguments)
            => Call(insatnce, methodInfo, arguments);

        [Obsolete("Use actual constructor instead", false)]
        public static MethodCallExpression Call(Expression insatnce, Aqua.TypeSystem.MethodInfo methodInfo, IEnumerable<Expression> arguments)
        {
            return new MethodCallExpression(insatnce, methodInfo, arguments);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static ConditionalExpression Conditional(Expression test, Expression ifTrue, Expression ifFalse)
        {
            return new ConditionalExpression(test, ifTrue, ifFalse);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static ConstantExpression Constant(object value)
        {
            return new ConstantExpression(value);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static ConstantExpression Constant(object value, Type type)
        {
            return new ConstantExpression(value, type);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static ConstantExpression Constant(object value, Aqua.TypeSystem.TypeInfo type)
        {
            return new ConstantExpression(value, type);
        }

        [Obsolete("Parameter list changed order. This method will be removed in a future version.", true)]
        public static ParameterExpression Parameter(string parameterName, Type type)
            => Parameter(type, parameterName, 0);

        [Obsolete("Use actual constructor instead", false)]
        public static ParameterExpression Parameter(Type parameterType, string parameterName, int instanceId)
        {
            return new ParameterExpression(parameterType, parameterName, instanceId);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static BinaryExpression MakeBinary(BinaryOperator binaryOperator, Expression leftOperand, Expression rightOperand)
        {
            return new BinaryExpression(binaryOperator, leftOperand, rightOperand);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static BinaryExpression MakeBinary(BinaryOperator binaryOperator, Expression leftOperand, Expression rightOperand, bool liftToNull, System.Reflection.MethodInfo method, LambdaExpression conversion = null)
        {
            return new BinaryExpression(binaryOperator, leftOperand, rightOperand, liftToNull, method, conversion);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static UnaryExpression MakeUnary(UnaryOperator unaryOperator, Expression operand, Type type = null, System.Reflection.MethodInfo method = null)
        {
            return new UnaryExpression(unaryOperator, operand, type, method);
        }

        [Obsolete("Use actual constructor instead", false)]
        internal static SortExpression Sort(LambdaExpression operand, SortDirection sortDirection)
        {
            return new SortExpression(operand, sortDirection);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static TypeBinaryExpression TypeIs(Expression expression, Type type)
        {
            return new TypeBinaryExpression(expression, type);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static LambdaExpression Lambda(Expression expression, IEnumerable<ParameterExpression> parameters)
        {
            return new LambdaExpression(expression, parameters);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static LambdaExpression Lambda(Expression expression, params ParameterExpression[] parameters)
        {
            return Lambda(expression, (IEnumerable<ParameterExpression>)parameters);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static LambdaExpression Lambda(Type lambdaType, Expression expression, params ParameterExpression[] parameters)
        {
            return Lambda(lambdaType, expression, (IEnumerable<ParameterExpression>)parameters);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static LambdaExpression Lambda(Type lambdaType, Expression expression, IEnumerable<ParameterExpression> parameters)
        {
            return new LambdaExpression(lambdaType, expression, parameters);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments)
        {
            return new NewExpression(constructor, arguments);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static NewExpression New(ConstructorInfo constructor, params Expression[] arguments)
        {
            return New(constructor, (IEnumerable<Expression>)arguments);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static ListInitExpression ListInit(NewExpression expression, IEnumerable<ElementInit> initializers)
        {
            return new ListInitExpression(expression, initializers);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static MemberInitExpression MemberInit(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
        {
            return new MemberInitExpression(newExpression, bindings);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static MemberInitExpression MemberInit(NewExpression newExpression, params MemberBinding[] bindings)
        {
            return MemberInit(newExpression, (IEnumerable<MemberBinding>)bindings);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static MemberAssignment Bind(Aqua.TypeSystem.MemberInfo member, Expression expression)
        {
            return new MemberAssignment(member, expression);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static MemberMemberBinding MemberBind(Aqua.TypeSystem.MemberInfo member, IEnumerable<MemberBinding> bindings)
        {
            return new MemberMemberBinding(member, bindings);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static MemberListBinding ListBind(Aqua.TypeSystem.MemberInfo member, IEnumerable<ElementInit> initializers)
        {
            return new MemberListBinding(member, initializers);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static ElementInit ElementInit(Aqua.TypeSystem.MethodInfo addMethod, System.Collections.ObjectModel.ReadOnlyCollection<Expression> arguments)
        {
            return new ElementInit(addMethod, arguments);
        }

        [Obsolete("Use actual constructor instead", false)]
        public static ElementInit ElementInit(System.Reflection.MethodInfo addMethod, IEnumerable<Expression> arguments)
        {
            return new ElementInit(addMethod, arguments);
        }

        // TODO: replace binding flags by bool flags    
        [Obsolete("Use actual constructor instead", false)]
        internal static ElementInit ElementInit(string methodName, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes, IEnumerable<Expression> arguments)
        {
            return new ElementInit(methodName, declaringType, bindingFlags, genericArguments, parameterTypes, arguments);
        }
    }
}
