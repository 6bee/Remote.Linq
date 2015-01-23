// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace System.Reflection //Remote.Linq.TypeSystem
{
    using System;

    /// <summary>
    /// Internal attribute as a NONFUNCTIONAL placeholder of it's .NET framework version
    /// </summary>
    [Flags]
    // TODO: change visibility to internal
    public enum BindingFlags
    {
        /// <summary>
        /// Specifies no binding flag.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Specifies that instance members are to be included in the search.
        /// </summary>
        Instance = 4,
        /// <summary>
        /// Specifies that static members are to be included in the search.
        /// </summary>
        Static = 8,
        /// <summary>
        /// Specifies that public members are to be included in the search.
        /// </summary>
        Public = 16,
        /// <summary>
        /// Specifies that non-public members are to be included in the search.
        /// </summary>
        NonPublic = 32,
    }
}
