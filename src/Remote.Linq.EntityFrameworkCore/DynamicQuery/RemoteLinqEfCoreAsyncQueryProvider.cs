// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.DynamicQuery;

using Aqua.TypeExtensions;
using System.Threading.Tasks;
using MethodInfo = System.Reflection.MethodInfo;

internal static class RemoteLinqEfCoreAsyncQueryProvider
{
    public static readonly MethodInfo TaskFromResultMethodInfo =
        typeof(Task).GetMethodEx(nameof(Task.FromResult));
}