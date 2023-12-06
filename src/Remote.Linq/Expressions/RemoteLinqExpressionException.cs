// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using System.Diagnostics.CodeAnalysis;

#if !NET8_0_OR_GREATER
    [Serializable]
#endif // NET8_0_OR_GREATER
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "This exception type requires an expression.")]
    public class RemoteLinqExpressionException : RemoteLinqException
    {
        private RemoteLinqExpressionException()
        {
            Expression = null!;
        }

        public RemoteLinqExpressionException(Expression expression)
        {
            Expression = expression;
        }

        public RemoteLinqExpressionException(Expression expression, string message)
            : base(message)
        {
            Expression = expression;
        }

        public RemoteLinqExpressionException(Expression expression, string message, Exception innerException)
            : base(message, innerException)
        {
            Expression = expression;
        }

#if !NET8_0_OR_GREATER
        protected RemoteLinqExpressionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            Expression = (Expression)info.GetValue(nameof(Expression), typeof(Expression))!;
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Expression), Expression);
        }
#endif // NET8_0_OR_GREATER

        public Expression Expression { get; }
    }
}