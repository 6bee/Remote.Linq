// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.SimpleAsyncStreamProtocol
{
    public struct NextResponse<T>
    {
        public long SequenceNumber { get; set; }

        public bool HasNext { get; set; }

        public T Item { get; set; }
    }
}
