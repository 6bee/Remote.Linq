// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

/// <summary>
/// Denotes a context for translating expressions from <i>System.Linq</i> to <i>Remote.Linq</i> and vice versa.
/// </summary>
public interface IExpressionTranslatorContext : IExpressionFromRemoteLinqContext, IExpressionToRemoteLinqContext
{
}