// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Common.DataContract;
    using Common.ServiceContract;
    using Remote.Linq;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Linq;

    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("This sample client retrieves data from the backend by using:");
            Console.WriteLine(" a) a traditional data service (get by id/name), and");
            Console.WriteLine(" b) a remote linq data service (dynamic filtering, sorting, and paging)");
            Console.WriteLine();
            Console.WriteLine("Wait for the server to indicate that it's started, then");
            Console.WriteLine("press <ENTER> to start the client.");
            Console.WriteLine();
            Console.ReadLine();

            TraditionalQuery();

            LinqQuery();

            LinqQueryWithOpenType();

            Console.WriteLine();
            Console.WriteLine("Done");
            Console.WriteLine("Press <ENTER> to terminate the client.");
            Console.ReadLine();
        }

        private static void LinqQuery()
        {
            Console.WriteLine();
            Console.WriteLine("QUERY PRODUCTS AND ORDERS THE LINQified WAY :)");
            Console.WriteLine("========================================================================");
            using (ServiceProxy<IRemoteLinqDataService> serviceProxy = new ServiceProxy<IRemoteLinqDataService>("RemoteLinqDataService"))
            {
                IQuery<Product> productQuery = serviceProxy
                    .CreateQuery<Product>(service => service.GetProducts)
                    .Where(p => p.Name == "Car");
                System.Collections.Generic.List<Product> products = productQuery.ToList();

                Console.WriteLine("List orders for product 'Car' having an order quantity bigger than one:\n");
                foreach (Product product in products)
                {
                    Console.WriteLine("\t{0}", product);

                    IQuery<Order> orderQuery = serviceProxy
                        .CreateQuery<Order>(service => service.GetOrders)
                        .Where(o => o.Items.Where(i => i.ProductId == product.Id).Sum(i => i.Quantity) > 1);
                    System.Collections.Generic.List<Order> orders = orderQuery.ToList();

                    Console.WriteLine("\tOrders ({0}):", orders.Count());
                    foreach (Order order in orders)
                    {
                        Console.WriteLine("\t\t{0}", order);
                        foreach (OrderItem item in order.Items)
                        {
                            Console.WriteLine("\t\t\t{0}", item);
                        }
                    }

                    Console.WriteLine();
                }
            }
        }

        private static void LinqQueryWithOpenType()
        {
            Console.WriteLine();
            Console.WriteLine("QUERY AN OPEN TYPE DATA SERVICE THE LINQified WAY :)");
            Console.WriteLine("========================================================================");
            using (ServiceProxy<IRemoteLinqDataService> serviceProxy = new ServiceProxy<IRemoteLinqDataService>("RemoteLinqDataService"))
            {
                IRemoteLinqDataService service = serviceProxy.Channel;

                System.Linq.Expressions.Expression<Func<Product, bool>> productFilterLinqExpression = product => product.Name == "Car";
                Remote.Linq.Expressions.LambdaExpression productFilterRemoteExpression = productFilterLinqExpression.ToRemoteLinqExpression();
                IQuery productQuery = new Query(typeof(Product)).Where(productFilterRemoteExpression);

                System.Collections.Generic.IEnumerable<object> products = service.GetData(productQuery);

                Console.WriteLine("List orders for product 'Car' having an order quantity bigger than one:\n");
                foreach (Product product in products)
                {
                    Console.WriteLine("\t{0}", product);

                    System.Linq.Expressions.Expression<Func<Order, bool>> orderFilterLinqExpression = order => order.Items.Where(i => i.ProductId == product.Id).Sum(i => i.Quantity) > 1;
                    Remote.Linq.Expressions.LambdaExpression orderFilterRemoteExpression = orderFilterLinqExpression.ToRemoteLinqExpression().ReplaceGenericQueryArgumentsByNonGenericArguments();
                    IQuery orderQuery = new Query(typeof(Order)).Where(orderFilterRemoteExpression);

                    System.Collections.Generic.IEnumerable<object> orders = service.GetData(orderQuery);

                    Console.WriteLine("\tOrders ({0}):", orders.Count());
                    foreach (Order order in orders)
                    {
                        Console.WriteLine("\t\t{0}", order);
                        foreach (OrderItem item in order.Items)
                        {
                            Console.WriteLine("\t\t\t{0}", item);
                        }
                    }

                    Console.WriteLine();
                }
            }
        }

        private static void TraditionalQuery()
        {
            Console.WriteLine();
            Console.WriteLine("QUERY PRODUCTS AND ORDERS THE TRADITIONAL WAY");
            Console.WriteLine("========================================================================");
            using (ServiceProxy<IDataService> serviceProxy = new ServiceProxy<IDataService>("DataService"))
            {
                IDataService service = serviceProxy.Channel;

                System.Collections.Generic.IEnumerable<Product> products = service.GetProductsByName("Car");

                Console.WriteLine("List orders for product 'Car' having an order quantity bigger than one.\n");
                Console.WriteLine("Note that filtering based on items count is performed on the client.\n");
                foreach (Product product in products)
                {
                    Console.WriteLine("\t{0}", product);

                    System.Collections.Generic.IEnumerable<Order> orders = service.GetOrdersByProductId(product.Id);

                    orders = orders.Where(o => o.Items.Where(i => i.ProductId == product.Id).Sum(i => i.Quantity) > 1);

                    Console.WriteLine("\tOrders ({0}):", orders.Count());
                    foreach (Order order in orders)
                    {
                        Console.WriteLine("\t\t{0}", order);
                        foreach (OrderItem item in order.Items)
                        {
                            Console.WriteLine("\t\t\t{0}", item);
                        }
                    }

                    Console.WriteLine();
                }
            }
        }
    }
}
