// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if NETFRAMEWORK

namespace Remote.Linq.Tests;

using System.Buffers;

/// <summary>
/// Polyfill type for <c>System.Buffers.ArrayBufferWriter&lt;T&gt;</c>.
/// </summary>
internal sealed class ArrayBufferWriter<T>(int initialSize = 256) : IBufferWriter<T>
{
    private T[] _buffer = new T[initialSize];
    private int _index;

    public void Advance(int count) => _index += count;

    public Memory<T> GetMemory(int sizeHint = 0)
    {
        Ensure(sizeHint);
        return _buffer.AsMemory(_index);
    }

    public Span<T> GetSpan(int sizeHint = 0)
    {
        Ensure(sizeHint);
        return _buffer.AsSpan(_index);
    }

    public ReadOnlyMemory<T> WrittenMemory => _buffer.AsMemory(0, _index);
    public ReadOnlySpan<T> WrittenSpan => _buffer.AsSpan(0, _index);

    private void Ensure(int sizeHint)
    {
        if (_index + sizeHint <= _buffer.Length)
        {
            return;
        }

        var newSize = Math.Max(_buffer.Length * 2, _index + sizeHint);
        Array.Resize(ref _buffer, newSize);
    }
}

#endif // NETFRAMEWORK
