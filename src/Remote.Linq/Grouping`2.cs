// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

/// <summary>
/// Represets a grouped sequence of elements that may be used for serialization.
/// </summary>
/// <typeparam name="TKey">Type of the grouping key.</typeparam>
/// <typeparam name="TElement">Element type of the grouped sequences.</typeparam>
[Serializable]
[DataContract]
public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
{
    /// <summary>
    /// Gets or sets the grouping key.
    /// </summary>
    public TKey Key { get; set; } = default!;

    /// <summary>
    /// Gets or sets the grouped sequence.
    /// </summary>
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Property serves as serialization contract")]
    public TElement[] Elements { get; set; } = default!;

    /// <inheritdoc/>
    public IEnumerator<TElement> GetEnumerator()
    {
        var elements = Elements ?? throw new InvalidOperationException($"{nameof(Elements)} property must not be null.");
        return ((IEnumerable<TElement>)elements).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}