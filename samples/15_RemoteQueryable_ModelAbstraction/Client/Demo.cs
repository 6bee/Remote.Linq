// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client;

using Common.Model;
using static CommonHelper;

public class Demo : IDemo
{
    private readonly Func<RemoteRepository> _repoProvider;
    private readonly ModelFormatter _formatter = new ModelFormatter();

    public Demo(Func<RemoteRepository> repoProvider)
        => _repoProvider = repoProvider;

    public void Run()
    {
        using var repo = _repoProvider();

        PrintHeader("GET ALL ENTITIES:");
        foreach (var entity in repo.Entities)
        {
            PrintLine(_formatter, $"  {entity.Id}: {entity}");
        }

        PrintHeader("GET ALL ENTITIES OF TYPE PRODUCT:");
        foreach (Product i in repo.Entities.OfType<Product>())
        {
            PrintLine(_formatter, $"  {i.Id}: {i.Name}");
        }

        PrintHeader("SELECT IDs:");
        var entityIdsQuery =
            from e in repo.Entities
            orderby e.Id ascending
            select new { EntityId = e.Id };
        foreach (var item in entityIdsQuery)
        {
            PrintLine(_formatter, $"  {item.EntityId}");
        }

        PrintHeader("COUNT:");
        PrintLine($"  Count = {repo.Entities.Count()}");
    }

    private sealed class ModelFormatter : ICustomFormatter, IFormatProvider
    {
        object IFormatProvider.GetFormat(Type formatType)
            => formatType == typeof(ICustomFormatter) ? this : null;

        string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg is OrderItem orderItem)
            {
                return $"Item #{orderItem.ProductId} -> {orderItem.Quantity} * {orderItem.UnitPrice:C} = {orderItem.Quantity * orderItem.UnitPrice:C}";
            }

            if (arg is Product product)
            {
                return $"Product #{product.Id} '{product.Name}' ({product.Price:C})";
            }

            if (arg is ProductGroup productGroup)
            {
                return $"Group #{productGroup.Id} '{productGroup.GroupName}'";
            }

            if (arg is ProductCategory productCategory)
            {
                return $"Category #{productCategory.Id} '{productCategory.Name}'";
            }

            return arg?.ToString() ?? string.Empty;
        }
    }
}