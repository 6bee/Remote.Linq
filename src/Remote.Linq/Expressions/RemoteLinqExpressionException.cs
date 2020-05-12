// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class RemoteLinqExpressionException : RemoteLinqException
    {
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

        protected RemoteLinqExpressionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Expression = (Expression)info.GetValue(nameof(Expression), typeof(Expression));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Expression), Expression);
        }

        public Expression Expression { get; }
    }
}
