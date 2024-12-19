// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Remote.Linq;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using System;
using System.Linq;
using System.Runtime.Serialization;

[Serializable]
[DataContract]
public class AsyncGrouping<TKey, TElement> : AsyncEnumerable<TElement>, IAsyncGrouping<TKey, TElement>
{
    [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
    public TKey Key { get; set; } = default!;
}