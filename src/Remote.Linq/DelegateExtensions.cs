// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.ExceptionServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class DelegateExtensions
{
    internal static object? DynamicInvokeAndUnwrap(this Delegate @delegate, params object?[] args)
    {
        try
        {
            return @delegate.DynamicInvoke(args);
        }
        catch (TargetInvocationException ex)
        {
            ExceptionDispatchInfo.Capture(Unwrap(ex)).Throw();
            throw; // satisfy compiler for CS0161 : not all code paths return a value
        }

        static Exception Unwrap(Exception ex)
        {
            while (ex.InnerException is not null && ex is TargetInvocationException)
            {
                ex = ex.InnerException;
            }

            return ex;
        }
    }
}