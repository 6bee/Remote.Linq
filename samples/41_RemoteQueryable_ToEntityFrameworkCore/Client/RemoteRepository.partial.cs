// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client;

using Common.Model;
using Remote.Linq;
using System.Linq;

partial class RemoteRepository
{
    public IQueryable<Market> Markets => RemoteQueryable.Factory.CreateAsyncQueryable<Market>(_asyncDataProvider);
}