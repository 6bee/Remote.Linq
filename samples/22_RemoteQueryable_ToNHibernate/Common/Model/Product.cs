// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.Model
{
    public class Product
    {
        public virtual int Id { get; set; }

        public virtual int ProductCategoryId { get; set; }

        public virtual string Name { get; set; }

        public virtual decimal Price { get; set; }
    }
}
