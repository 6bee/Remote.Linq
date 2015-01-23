// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    internal static class AppDomainExtensions
    {
        // note: this extension method is used for SL4. Starting from SL5 this method is publicly accessible on AppDomain
        public static Assembly[] GetAssemblies(this AppDomain appDomain)
        {
            // dynamic (requires Microsoft.CSharp.dll, SL SDK 4)
            return ((dynamic)AppDomain.CurrentDomain).GetAssemblies() as Assembly[];
        }
    }
}
