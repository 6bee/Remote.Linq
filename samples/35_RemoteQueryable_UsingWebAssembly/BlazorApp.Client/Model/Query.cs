// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace BlazorApp.Model;

using Remote.Linq.Expressions;

public sealed class Query(Expression expression)
{
    public Expression Expression { get; } = expression;
}