// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.SimpleAsyncQueryProtocol
{
    public readonly struct NextRequest
    {
        public long SequenceNumber { get; init; }
    }
}