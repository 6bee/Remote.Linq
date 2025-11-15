// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#pragma warning disable IDE0161 // Convert to file-scoped namespace
namespace Client
#pragma warning restore IDE0161 // Convert to file-scoped namespace
{
    public interface IDemo
    {
        void Run();
    }
}

public static class IDemoExtensions
{
    /// <summary>
    /// Run demo and print any unexpected exception to console.
    /// </summary>
    public static void RunDemo(this Client.IDemo demo)
    {
        try
        {
            demo.Run();
        }
        catch (Exception ex)
        {
            CommonHelper.PrintError(ex);
            CommonHelper.Print("hit enter to continue");
            Console.ReadLine();
        }
    }
}