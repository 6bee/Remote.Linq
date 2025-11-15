// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace BlazorApp.Model;

public sealed record class Person
{
    public required string UserName { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public List<string> Emails { get; private init; } = new();
}