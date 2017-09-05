// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.RemoteQueryable
{
    public class DbFunctions
    {
        internal DbFunctions()
        {
        }

        public bool Like(string matchExpression, string pattern) => true;
    }

    public static class Db
    {
        public static DbFunctions Functions { get; } = new DbFunctions();
    }

}