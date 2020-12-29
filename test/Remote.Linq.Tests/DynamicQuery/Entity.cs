// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.DynamicQuery
{
    internal class Entity
    {
        public Entity(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}