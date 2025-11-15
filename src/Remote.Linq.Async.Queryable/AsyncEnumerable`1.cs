// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Remote.Linq;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

[Serializable]
[DataContract]
public class AsyncEnumerable<T> : IAsyncEnumerable<T>
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Property serves as serialization contract")]
    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public T[] Elements { get; set; } = null!;

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var elements = Elements ?? throw new InvalidOperationException($"{nameof(Elements)} property must not be null.");
        return elements.ToAsyncEnumerable().GetAsyncEnumerator(cancellationToken);
    }
}