// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Remote.Linq
{
    internal static class AppDomainExtensions
    {
        // reflection
        //private static readonly MethodInfo GetAssembliesMethodInfo = typeof(AppDomain).GetMethod("GetAssemblies", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        // file access
        //private static readonly object _lock = new object();
        //private static Assembly[] _assemblies = null;

        // note: this extension method is used for SL4. Starting from SL5 this method is publicly accessible on AppDomain
        public static Assembly[] GetAssemblies(this AppDomain appDomain)
        {
            // dynamic (requires Microsoft.CSharp.dll, SL SDK 4)
            return ((dynamic)AppDomain.CurrentDomain).GetAssemblies() as Assembly[];

            // reflection (throws exception when invoking AppDomain.GetAssemblies())
            //var result = GetAssembliesMethodInfo.Invoke(appDomain, new object[0]);
            //return (Assembly[])result;

            // file access (throws exception when calling Assembly.Location)
            //if (_assemblies == null)
            //{
            //    lock (_lock)
            //    {
            //        if (_assemblies == null)
            //        {
            //            var fullPath = typeof(AppDomainExtensions).Assembly.Location;
            //            var directory = Path.GetDirectoryName(fullPath);

            //            var assemblyFiles = Directory.EnumerateFiles(directory, "*.dll").Union(Directory.EnumerateFiles(directory, "*.exe"));

            //            var assemblies =
            //                from file in assemblyFiles
            //                let assemblyName = new AssemblyName() { Name = file }
            //                select Assembly.Load(assemblyName);

            //            _assemblies = assemblies.ToArray();
            //        }
            //    }
            //}
            //return _assemblies;
        }
    }
}
