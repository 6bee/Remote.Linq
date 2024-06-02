// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace BlazorApp.Services;

using Aqua.Dynamic;
using Remote.Linq.Expressions;

public interface IDataProvider
{
    DynamicObject Execute(Expression expression);
}