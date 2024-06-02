// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace BlazorApp.Client.Services;

using BlazorApp.Model;

public interface IClientRepository
{
    IQueryable<Person> People { get; }
}