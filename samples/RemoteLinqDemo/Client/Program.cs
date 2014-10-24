// Copyright (c) Christof Senn. All rights reserved. 

using System;
using System.Linq;
using System.Linq.Expressions;
using Common.DataContract;
using Common.ServiceContract;
using Remote.Linq;

namespace Client
{
    public class Program
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
            Console.WriteLine("QUERY PRODUCTS AND ORDERS THE LINQified WAY :)");
            Console.WriteLine("========================================================================");
            using (var serviceProxy = new ServiceProxy<IRemoteLinqDataService>("RemoteLinqDataService"))
            {
                var productQuery = serviceProxy
                    .CreateQuery<Product>(service => service.GetProducts)
                    .Where(p => p.Name == "Car");
                var products = productQuery.ToList();

                Console.WriteLine("For product 'Car' list orders having an order quantity bigger than one \nfor the corresponsing product:\n");
                foreach (var product in products)
                {
                    Console.WriteLine("\t{0}", product);

                    var orderQuery = serviceProxy
                        .CreateQuery<Order>(service => service.GetOrders)
                        .Where(o => o.Items.Where(i => i.ProductId == product.Id).Sum(i => i.Quantity) > 1);
                    var orders = orderQuery.ToList();

                    Console.WriteLine("\tOrders ({0}):", orders.Count());
                    foreach (var order in orders)
                    {
                        Console.WriteLine("\t\t{0}", order);
                        foreach (var item in order.Items)
                        {
                            Console.WriteLine("\t\t\t{0}", item);
                        }
                    }
                    Console.WriteLine("\n\t************************************************************************");
                }
            }
        }

        private static void LinqQueryWithOpenType()
        {
            Console.WriteLine("QUERY AN OPEN TYPE DATA SERVICE THE LINQified WAY :)");
            Console.WriteLine("========================================================================");
            using (var serviceProxy = new ServiceProxy<IRemoteLinqDataService>("RemoteLinqDataService"))
            {
                var service = serviceProxy.Channel;

                System.Linq.Expressions.Expression<Func<Product, bool>> productFilterLinqExpression = product => product.Name == "Car";
                Remote.Linq.Expressions.LambdaExpression productFilterRemoteExpression = productFilterLinqExpression.ToRemoteLinqExpression();
                var productQuery = new Query(typeof(Product)).Where(productFilterRemoteExpression) as Query;

                var products = service.GetData(productQuery);

                Console.WriteLine("For product 'Car' list orders having an order quantity bigger than one \nfor the corresponsing product:\n");
                foreach (Product product in products)
                {
                    Console.WriteLine("\t{0}", product);

                    System.Linq.Expressions.Expression<Func<Order, bool>> orderFilterLinqExpression = order => order.Items.Where(i => i.ProductId == product.Id).Sum(i => i.Quantity) > 1;
                    Remote.Linq.Expressions.LambdaExpression orderFilterRemoteExpression = orderFilterLinqExpression.ToRemoteLinqExpression();
                    var orderQuery = new Query(typeof(Order)).Where(orderFilterRemoteExpression) as Query;

                    var orders = service.GetData(orderQuery);

                    Console.WriteLine("\tOrders ({0}):", orders.Count());
                    foreach (Order order in orders)
                    {
                        Console.WriteLine("\t\t{0}", order);
                        foreach (var item in order.Items)
                        {
                            Console.WriteLine("\t\t\t{0}", item);
                        }
                    }
                    Console.WriteLine("\n\t************************************************************************");
                }
            }
        }

        private static void TraditionalQuery()
        {
            Console.WriteLine("QUERY PRODUCTS AND ORDERS THE TRADITIONAL WAY");
            Console.WriteLine("========================================================================");
            using (var serviceProxy = new ServiceProxy<IDataService>("DataService"))
            {
                var service = serviceProxy.Channel;

                var products = service.GetProductsByName("Car");

                Console.WriteLine("For product 'Car' list orders having an order quantity bigger than one \nfor the corresponsing product.\n");
                Console.WriteLine("Please note that filtering based on number of order items is perfomed on the client.\n");
                foreach (var product in products)
                {
                    Console.WriteLine("\t{0}", product);

                    var orders = service.GetOrdersByProductId(product.Id);

                    orders = orders.Where(o => o.Items.Where(i => i.ProductId == product.Id).Sum(i => i.Quantity) > 1);

                    Console.WriteLine("\tOrders ({0}):", orders.Count());
                    foreach (var order in orders)
                    {
                        Console.WriteLine("\t\t{0}", order);
                        foreach (var item in order.Items)
                        {
                            Console.WriteLine("\t\t\t{0}", item);
                        }
                    }
                    Console.WriteLine("\n\t************************************************************************");
                }
            }
        }
    }
}
