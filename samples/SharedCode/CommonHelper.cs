// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Globalization;

internal static class CommonHelper
{
    private const string TitleBase = "(⌐■_■)  Remote.Linq Demo";

    static CommonHelper()
    {
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void Title(string title) => Console.Title = $"{TitleBase} | {title}";

    /// <summary>
    /// Writest a section header to std out.
    /// </summary>
    public static void PrintHeader(string header, bool underline = false, int linebreaks = 1)
        => PrintSection("\n" + header, underline, ConsoleColor.Blue, linebreaks);

    /// <summary>
    /// Writest a note std out.
    /// </summary>
    public static void PrintNote(string note)
        => PrintSection(note, false, ConsoleColor.DarkGreen, 1);

    private static void PrintSection(string text, bool underline, ConsoleColor color, int linebreaks)
    {
        using var c = TextColor(color);
        Print(text);
        if (!string.IsNullOrEmpty(text) && underline)
        {
            Print("\n" + new string('>', text.Length));
        }

        Print(new string('\n', linebreaks));
    }

    /// <summary>
    /// Writest the text to std out.
    /// </summary>
    public static void PrintServerReady(string text = "The query service is ready.")
    {
        using var bg = BackgroundColor(ConsoleColor.DarkGreen);
        PrintLine(text);
    }

    /// <summary>
    /// Writest the text to std out.
    /// </summary>
    public static void PrintSetup(string text = null)
    {
        using var c = TextColor(ConsoleColor.DarkGray);
        PrintLine(text);
    }

    /// <summary>
    /// Writest the error to std out.
    /// </summary>
    public static void PrintError(Exception error) => PrintError(error.ToString());

    /// <summary>
    /// Writest the error to std out.
    /// </summary>
    public static void PrintError(string error)
    {
        using var c = TextColor(ConsoleColor.DarkRed);
        PrintLine(error);
    }

    /// <summary>
    /// Writest the text to std out.
    /// </summary>
    public static void PrintLine(string text = null) => Print(text + "\n");

    /// <summary>
    /// Writest the text to std out.
    /// </summary>
    public static void PrintLine(IFormatProvider formatProvider, FormattableString formattable)
        => Print(Format(formatProvider, formattable) + "\n");

    public static void Print(IFormatProvider formatProvider, FormattableString formattable)
        => Print(Format(formatProvider, formattable));

    private static string Format(IFormatProvider formatProvider, FormattableString formattable)
        => formattable.ToString(formatProvider ?? CultureInfo.InvariantCulture);

    public static void Print(string text) => Console.Write(text);

    public static void WaitForEnterKey(string message = "Press <ENTER> to terminate.")
    {
        if (message != null)
        {
            using (BackgroundColor(ConsoleColor.Yellow))
            using (TextColor(ConsoleColor.Blue))
            {
                Print(message);
            }
        }

        Console.ReadLine();
    }

    public static IDisposable TextColor(ConsoleColor color) => new ForegroundColorScope(color);

    public static IDisposable BackgroundColor(ConsoleColor color) => new BackgroundColorScope(color);

    private sealed class ForegroundColorScope : IDisposable
    {
        private readonly ConsoleColor _previousColor;

        public ForegroundColorScope(ConsoleColor color)
        {
            _previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        public void Dispose() => Console.ForegroundColor = _previousColor;
    }

    private sealed class BackgroundColorScope : IDisposable
    {
        private readonly ConsoleColor _previousColor;

        public BackgroundColorScope(ConsoleColor color)
        {
            _previousColor = Console.BackgroundColor;
            Console.BackgroundColor = color;
        }

        public void Dispose() => Console.BackgroundColor = _previousColor;
    }
}
