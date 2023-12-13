// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client;

using Common;
using Common.SimpleAsyncQueryProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

internal sealed class AsyncStreamEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly Stream _stream;
    private readonly CancellationToken _cancellation;
    private Lazy<T> _current;
    private long _sequence = 0;

    public AsyncStreamEnumerator(Stream stream, CancellationToken cancellation)
    {
        SetError($"{nameof(MoveNextAsync)} has not completed yet.");
        _stream = stream;
        _cancellation = cancellation;
    }

    private void SetError(string error) => SetError(new InvalidOperationException(error));

    private void SetError(Exception exception)
    {
        _current = new Lazy<T>(() => throw exception);
    }

    private void SetCurrent(T current)
    {
        _current = new Lazy<T>(() => current);
    }

    public T Current => _current.Value;

    public ValueTask DisposeAsync() => _stream.DisposeAsync();

    public async ValueTask<bool> MoveNextAsync()
    {
        try
        {
            _cancellation.ThrowIfCancellationRequested();

            await _stream.WriteAsync(new NextRequest { SequenceNumber = Interlocked.Increment(ref _sequence) }, _cancellation).ConfigureAwait(false);
            await _stream.FlushAsync(_cancellation).ConfigureAwait(false);

            var response = await _stream.ReadAsync<NextResponse<T>>(_cancellation).ConfigureAwait(false);
            if (response.SequenceNumber != _sequence)
            {
                var exception = new InvalidOperationException("Async stream is out of bound.");
                SetError(exception);
                throw exception;
            }

            if (response.HasNext)
            {
                SetCurrent(response.Item);
                return true;
            }
            else
            {
                SetError("Reached end of stream.");
                return false;
            }
        }
        catch (Exception ex)
        {
            SetError(ex);
            throw;
        }
    }
}