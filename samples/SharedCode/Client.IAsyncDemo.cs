// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;

namespace Client
{
    using System.Threading.Tasks;

    public interface IAsyncDemo
    {
        Task RunAsync();
    }
}

public static class IAsyncDemoExtensions
{
    /// <summary>
    /// Run demo and print any unexpected exception to console.
    /// </summary>
    public static void RunAsyncDemo(this Client.IAsyncDemo demo)
    {
        try
        {
            demo.RunAsync().Wait();
        }
        catch (Exception ex)
        {
            CommonHelper.PrintError(ex);
            CommonHelper.Print("hit enter to continue");
            Console.ReadLine();
        }
    }
}