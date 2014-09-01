// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.


namespace Common.Model
{
    public class Product
    {
        public int Id { get; set; }
        public int ProductCategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
