// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    internal static class MemberInfoExtensions
    {
        internal static Remote.Linq.TypeSystem.MemberTypes GetMemberType(this System.Reflection.MemberInfo member)
        {
            var t = (Remote.Linq.TypeSystem.MemberTypes)member.MemberType;
            return t;
        }
    }
}