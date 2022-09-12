// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.ExceptionServices;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class MethodInfoExtensions
    {
        /// <summary>
        /// Invokes the method and unwraps potential <see cref="TargetInvocationException"/> in case any gets thrown.
        /// </summary>
        /// <param name="methodInfo">The method to be invoked.</param>
        /// <param name="instance">The instance in case of an instance method or <see langword="null"/> for static methods.</param>
        /// <param name="args">The argument list.</param>
        /// <returns>The return value of the method invocation.</returns>
        internal static object? InvokeAndUnwrap(this MethodInfo methodInfo, object? instance, params object?[] args)
        {
            try
            {
                return methodInfo.Invoke(instance, args);
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

        /// <summary>
        /// Invokes the method and unwraps potential <see cref="TargetInvocationException"/> in case any gets thrown.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="methodInfo">The method to be invoked.</param>
        /// <param name="instance">The instance in case of an instance method or <see langword="null"/> for static methods.</param>
        /// <param name="args">The argument list.</param>
        /// <returns>The return value of the method invocation.</returns>
        internal static TResult InvokeAndUnwrap<TResult>(this MethodInfo methodInfo, object? instance, params object?[] args)
            => (TResult)methodInfo.InvokeAndUnwrap(instance, args) !;
    }
}