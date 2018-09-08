// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.RemoteQueryable.QueryTestData
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public CategoryType CategoryType { get; set; }

        public CategorySourceType? CategorySourceType { get; set; }
    }
}
