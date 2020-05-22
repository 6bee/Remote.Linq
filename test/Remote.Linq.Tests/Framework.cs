// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests
{
    using System.Runtime.InteropServices;

    public static class Framework
    {
        public static bool IsDotNetCore => RuntimeInformation.FrameworkDescription.ToUpper().Contains("CORE");
    }
}