// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server;

using Aqua.Dynamic;
using Common.Model;
using Microsoft.AspNetCore.Mvc;
using Remote.Linq.ExpressionExecution;

[ApiController]
[Route("api")]
public class QueryController : Controller
{
    private InMemoryDataStore DataStore => InMemoryDataStore.Instance;

    [HttpPost("query")]
    public DynamicObject Query([FromBody] Query query)
        => query.Expression.Execute(DataStore.QueryableByTypeProvider);
}