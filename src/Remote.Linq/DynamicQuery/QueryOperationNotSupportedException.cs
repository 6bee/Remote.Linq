// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class QueryOperationNotSupportedException : RemoteLinqException
    {
        public QueryOperationNotSupportedException()
        {
        }

        public QueryOperationNotSupportedException(string message)
            : base(message)
        {
        }

        public QueryOperationNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected QueryOperationNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
