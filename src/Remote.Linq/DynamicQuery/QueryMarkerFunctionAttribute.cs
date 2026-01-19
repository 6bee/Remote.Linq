// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

/// <summary>
/// Denotes annotated methods as query marker functions to prevent local evaluation (i.e. execution of the method) when translating expressions.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class QueryMarkerFunctionAttribute : Attribute;