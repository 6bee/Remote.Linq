// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.ComponentModel;
    using System.Reflection;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class MethodInfoExtensions
    {
        public static object InvokeAndUnwrap(this MethodInfo methodInfo, object instance, params object[] args)
            => methodInfo.InvokeAndUnwrap<object>(instance, args);

        public static TResult InvokeAndUnwrap<TResult>(this MethodInfo methodInfo, object instance, params object[] args)
        {
            try
            {
                return (TResult)methodInfo.Invoke(instance, args);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
