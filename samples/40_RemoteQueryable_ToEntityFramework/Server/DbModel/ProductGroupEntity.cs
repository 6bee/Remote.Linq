// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server.DbModel;

public class ProductGroupEntity
{
    public int Id { get; set; }

    public string GroupName { get; set; }

    public ICollection<ProductEntity> Products { get; set; }
}