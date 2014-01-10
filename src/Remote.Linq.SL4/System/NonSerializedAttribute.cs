// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace System
{
    /// <summary>
    /// Placeholder attribute as a NONFUNCTIONAL placeholder of it's .NET framework version
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class NonSerializedAttribute : Attribute
    {
    }
}
