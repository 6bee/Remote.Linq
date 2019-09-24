// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

#if !NETSTANDARD1_X
[assembly: SecurityRules(SecurityRuleSet.Level2)]
#endif
