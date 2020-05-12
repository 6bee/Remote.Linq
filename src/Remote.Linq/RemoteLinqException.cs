// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class RemoteLinqException : Exception
    {
        public RemoteLinqException()
        {
        }

        public RemoteLinqException(string message)
            : base(message)
        {
        }

        public RemoteLinqException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RemoteLinqException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
