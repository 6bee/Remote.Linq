// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

using Aqua.Dynamic;
using System;
using SystemLinq = System.Linq.Expressions;

/// <summary>
/// Denotes a provider for mapping values on translating expressions from <i>System.Linq</i> to <i>Remote.Linq</i> and vice versa.
/// </summary>
public interface IExpressionValueMapperProvider
{
    /// <summary>
    /// Gets a <see cref="IDynamicObjectMapper"/> to map values from and to <see cref="DynamicObject"/>.
    /// </summary>
    IDynamicObjectMapper ValueMapper { get; }

    /// <summary>
    /// Gets a function to check whether expressions or parts thereof are eligibly for local evaluation.
    /// </summary>
    Func<SystemLinq.Expression, bool>? CanBeEvaluatedLocally { get; }
}