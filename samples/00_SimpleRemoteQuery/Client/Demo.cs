// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client;

using Common.Model;
using Common.ServiceContract;
using Remote.Linq;
using Remote.Linq.ExpressionVisitors;
using Remote.Linq.SimpleQuery;
using static CommonHelper;

public class Demo : IDemo
{
    private static readonly ModelFormatter _formatter = new ModelFormatter();

    public void Run()
    {
        TraditionalQuery();
        PrintSetup();

        LinqQuery();
        PrintSetup();

        LinqQueryWithOpenType();
        PrintSetup();
    }

    private static void TraditionalQuery()
    {
        PrintHeader("a) QUERY PRODUCTS AND ORDERS THE TRADITIONAL WAY");

        PrintNote("Retrieve orders for product 'Car' having an order quantity bigger than one.");
        PrintNote("ATTENTION: Filtering based on items count is performed on the client.");
        PrintNote("           Hence, all orders have to be retrieved from backend first.");

        using var serviceProxy = new ServiceProxy<ITraditionalDataService>("net.tcp://localhost:8080/traditionaldataservice");

        IEnumerable<Product> products = serviceProxy.Service.GetProductsByName("Car");

        foreach (Product product in products)
        {
            PrintLine(_formatter, $"  {product}");

            IEnumerable<Order> orders = serviceProxy.Service.GetOrdersByProductId(product.Id);

            orders = orders.Where(o => o.Items.Where(i => i.ProductId == product.Id).Sum(i => i.Quantity) > 1);

            PrintLine($"  Orders ({orders.Count()}):");
            foreach (Order order in orders)
            {
                PrintLine(_formatter, $"    {order}");
                foreach (OrderItem item in order.Items)
                {
                    PrintLine(_formatter, $"      {item}");
                }
            }

            PrintLine();
        }
    }

    private static void LinqQuery()
    {
        PrintHeader("b) QUERY PRODUCTS AND ORDERS THE LINQified WAY");
        PrintNote("Retrieve orders for product 'Car' having an order quantity bigger than one:");

        using var serviceProxy = new ServiceProxy<IRemoteLinqDataService>("net.tcp://localhost:8080/remotelinqdataservice");

        IQuery<Product> productQuery = serviceProxy
            .CreateQuery<Product>(service => service.GetProducts)
            .Where(p => p.Name == "Car");

        List<Product> products = productQuery.ToList();

        foreach (Product product in products)
        {
            PrintLine(_formatter, $"  {product}");

            IQuery<Order> orderQuery = serviceProxy
                .CreateQuery<Order>(service => service.GetOrders)
                .Where(o => o.Items.Where(i => i.ProductId == product.Id).Sum(i => i.Quantity) > 1);

            List<Order> orders = orderQuery.ToList();

            PrintLine($"  Orders ({orders.Count}):");
            foreach (Order order in orders)
            {
                PrintLine(_formatter, $"    {order}");
                foreach (OrderItem item in order.Items)
                {
                    PrintLine(_formatter, $"      {item}");
                }
            }

            PrintLine();
        }
    }

    private static void LinqQueryWithOpenType()
    {
        PrintHeader("c) QUERY AN OPEN TYPED DATA SERVICE THE LINQified WAY");
        PrintNote("Retrieve orders for product 'Car' having an order quantity bigger than one:");

        using var serviceProxy = new ServiceProxy<IRemoteLinqDataService>("net.tcp://localhost:8080/remotelinqdataservice");

        System.Linq.Expressions.Expression<Func<Product, bool>> productFilterLinqExpression = product => product.Name == "Car";
        Remote.Linq.Expressions.LambdaExpression productFilterRemoteExpression = productFilterLinqExpression.ToRemoteLinqExpression();

        IQuery productQuery = new Query(typeof(Product)).Where(productFilterRemoteExpression);

        IEnumerable<object> products = serviceProxy.Service.GetData(productQuery);

        foreach (Product product in products)
        {
            PrintLine(_formatter, $"  {product}");

            System.Linq.Expressions.Expression<Func<Order, bool>> orderFilterLinqExpression =
                order => order.Items
                    .Where(i => i.ProductId == product.Id)
                    .Sum(i => i.Quantity) > 1;

            Remote.Linq.Expressions.LambdaExpression orderFilterRemoteExpression = orderFilterLinqExpression
                .ToRemoteLinqExpression()
                .ReplaceGenericQueryArgumentsByNonGenericArguments();

            IQuery orderQuery = new Query(typeof(Order)).Where(orderFilterRemoteExpression);

            IEnumerable<object> orders = serviceProxy.Service.GetData(orderQuery);

            PrintLine($"  Orders ({orders.Count()}):");
            foreach (Order order in orders)
            {
                PrintLine(_formatter, $"    {order}");
                foreach (OrderItem item in order.Items)
                {
                    PrintLine(_formatter, $"      {item}");
                }
            }

            PrintLine();
        }
    }

    private sealed class ModelFormatter : ICustomFormatter, IFormatProvider
    {
        object IFormatProvider.GetFormat(Type formatType)
            => formatType == typeof(ICustomFormatter) ? this : null;

        string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg is Order order)
            {
                return $"Order: {order.Items.Count} Item{(order.Items.Count > 1 ? "s" : null)}  Total {order.TotalAmount:C}";
            }

            if (arg is OrderItem orderItem)
            {
                return $"Prod #{orderItem.ProductId}: {orderItem.Quantity} * {orderItem.UnitPrice:C} = {orderItem.Quantity * orderItem.UnitPrice:C}";
            }

            if (arg is Product product)
            {
                return $"Product #{product.Id} '{product.Name}' ({product.Price:C})";
            }

            return arg?.ToString() ?? string.Empty;
        }
    }
}