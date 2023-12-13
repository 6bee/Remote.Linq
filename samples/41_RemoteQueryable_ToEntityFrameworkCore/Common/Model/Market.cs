// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.Model;

using System.Collections.Generic;

public partial class Market
{
    public int Id { get; set; }

    public string Name { get; set; }

    public IList<ProductMarket> Products { get; set; }
}