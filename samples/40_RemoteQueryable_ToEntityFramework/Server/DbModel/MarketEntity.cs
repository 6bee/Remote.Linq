// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server.DbModel;

using System.Collections.Generic;

public class MarketEntity
{
    public int Id { get; set; }

    public string Name { get; set; }

    public IList<ProductEntity> Products { get; set; }
}