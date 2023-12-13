// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.Model;

public class CrossJoinResultDto
{
    public string Category { get; set; }

    public string Name { get; set; }

    public override string ToString()
        => $"{nameof(CrossJoinResultDto)} {{ {nameof(Category)}: {Category}, {nameof(Name)}: {Name} }}";
}

public class InnerJoinResultDto
{
    public string Name { get; set; }

    public decimal Price { get; set; }

    public string XY { get; set; }

    public override string ToString()
        => $"{nameof(InnerJoinResultDto)} {{ {nameof(Name)}: {Name}, {nameof(Price)}: {Price}, {nameof(XY)}: {XY} }}";
}

public class TotalAmountByCategoryDto
{
    public string Category { get; set; }

    public decimal Amount { get; set; }

    public override string ToString()
        => $"{nameof(TotalAmountByCategoryDto)} {{ {nameof(Category)}: {Category}, {nameof(Amount)}: {Amount} }}";
}