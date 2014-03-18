// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Remote.Linq
{
    partial class TypeResolver
    {
        private readonly static object _assembliesLock = new object();
        private static IEnumerable<Assembly> _assemblies;

        private static IEnumerable<Assembly> GetAssemblies()
        {
            if (ReferenceEquals(null, _assemblies))
            {
                lock (_assembliesLock)
                {
                    if (ReferenceEquals(null, _assemblies))
                    {
                        var assemblies = new List<Assembly>();
                        try
                        {
                            var files = Windows.ApplicationModel.Package.Current.InstalledLocation.GetFilesAsync().GetResults(); // blocking
                            foreach (var file in files)
                            {
                                if ((file.FileType == ".dll") || (file.FileType == ".exe"))
                                {
                                    var assemblyName = new AssemblyName { Name = Path.GetFileNameWithoutExtension(file.Name) };
                                    var assembly = Assembly.Load(assemblyName);
                                    assemblies.Add(assembly);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                        }
                        _assemblies = assemblies;
                    }
                }
            }
            return _assemblies;
        }
    }
}
