# Remote.Linq

[![GitHub license][lic-badge]][lic-link]
[![Github Workflow][pub-badge]][pub-link]

| branch | AppVeyor                         | Travis CI                      | Codecov.io         | Codacy            | CodeFactor             |
| ---    | ---                              | ---                            | ---                | ---               | ---                    |
| `main` | [![AppVeyor Build Status][1]][2] | [![Travis Build Status][3]][4] | [![codecov][5]][6] | [![Codacy][7]][8] | [![CodeFactor][9]][10] |

| package                           | nuget                    | myget                          |
| ---                               | ---                      | ---                            |
| `Remote.Linq`                     | [![NuGet Badge][13]][14] | [![MyGet Pre Release][15]][16] |
| `Remote.Linq.Async.Queryable`     | [![NuGet Badge][33]][34] | [![MyGet Pre Release][35]][36] |
| `Remote.Linq.EntityFramework`     | [![NuGet Badge][17]][18] | [![MyGet Pre Release][19]][20] |
| `Remote.Linq.EntityFrameworkCore` | [![NuGet Badge][21]][22] | [![MyGet Pre Release][23]][24] |
| `Remote.Linq.Newtonsoft.Json`     | [![NuGet Badge][25]][26] | [![MyGet Pre Release][27]][28] |
| `Remote.Linq.protobuf-net`        | [![NuGet Badge][29]][30] | [![MyGet Pre Release][31]][32] |

## Description

_Remote.Linq_ is a small and easy to use - yet very powerful - library to translate LINQ expression trees to strongly typed, serializable expression trees and vice versa. It provides functionality to send arbitrary LINQ queries to a remote service to be applied and executed against any enumerable or queryable data collection.

Building a LINQ interface for custom services is made a breeze by using _Remote.Linq_.

## Features

* Translate LINQ expressions into serializable expression trees (remote LINQ expression) and vice versa.
* Build remote single-type query services (paging, sorting, filtering).
* Build remote complex LINQ query services (arbitrary LINQ query including joins, groupings, aggregations, projections, etc.).

## Scope

In contrast to _[re-linq][re-linq-repo]_, this project enables serialization and deserialization of expression trees and applying LINQ expressions to other LINQ providers e.g. linq-to-object, linq-to-entity, etc.

Remote.Linq makes it super easy to implement a service that executes LINQ queries defined on a client against a data source on a remote server.

Write operations (insert/update/delete) have to be implemented by other means if needed. _[InfoCarrier.Core][infocarrier-repo]_ or _[EfCore.Client][efcore-client-repo]_ might be interesting for such scenarios.

## How to Use

Check-out _Remote.Linq.Samples.sln_ and _samples_ folder for a number of sample use cases.

### Client Code Sample

Implement a repository class to set-up server connection and expose the queryable data sets (`IQueryable<>`)

```C#
public class ClientDataRepository
{
    private readonly Func<Expression, DynamicObject> _dataProvider;

    public RemoteRepository(string uri)
    {
        _dataProvider = expression =>
        {
            // setup service connectivity
            using IQueryService service = CreateServerConnection(uri);
            // send expression to service and get back results
            DynamicObject result = service.ExecuteQuery(expression);
            return result;
        };
    }

    public IRemoteQueryable<Blog> Blogs => RemoteQueryable.Factory.CreateQueryable<Blog>(_dataProvider);
   
    public IRemoteQueryable<Post> Posts => RemoteQueryable.Factory.CreateQueryable<Post>(_dataProvider);
   
    public IRemoteQueryable<User> Users => RemoteQueryable.Factory.CreateQueryable<User>(_dataProvider);
    
    // where IRemoteQueryable<out T> is IQueryable<out T>
}
```

Use your repository to compose LINQ query and let the data be retrieved from the backend service

```C#
var repository = new ClientDataRepository("https://myserver/queryservice");

var myBlogPosts = (
    from blog in repository.Blogs
    from post in blog.Posts
    join owner in repository.Users on blog.OwnerId equals owner.Id
    where owner.login == "hi-its-me"
    select new 
    {
        post.Title,
        post.Date,
        Preview = post.Text.Substring(0, 50)
    }).ToList();
```

### Server Code Sample

Implement the backend service to handle the client's query expression by applying it to a data source e.g. an ORM

```C#
public interface IQueryService : IDisposable
{
    DynamicObject ExecuteQuery(Expression queryExpression);
}

public class QueryService : IQueryService
{
    // any linq provider e.g. entity framework, nhibernate, ...
    private IDataProvider _datastore = new ObjectRelationalMapper();

    // you need to be able to retrieve an IQueryable by type
    private Func<Type, IQueryable> _queryableProvider = type => _datastore.GetQueryableByType(type);

    public DynamicObject ExecuteQuery(Expression queryExpression)
    {
        // `Execute` is an extension method provided by Remote.Linq
        // it applies an expression to a data source and returns the result
        return queryExpression.Execute(queryableProvider: _queryableProvider);
    }

    public void Dispose() => _datastore.Dispose();
}
```

### Async Code Sample

```C#
IAsyncRemoteQueryable<TEntity> asyncQuery =
  RemoteQueryable.Factory.CreateAsyncQueryable<TEntity>(...);
TEntity[] result = await asyncQuery.ToArrayAsync();

// where interface IAsyncRemoteQueryable<out T> is IRemoteQueryable<out T> is IQueryable<out T>
```

### Async Stream Code Sample

```C#
IAsyncRemoteStreamQueryable<TEntity> asyncStreamQuery =
  RemoteQueryable.Factory.CreateAsyncStreamQueryable<TEntity>(...);
await foreach (TEntity item in asyncStreamQuery)
{
}

// where interface IAsyncRemoteStreamQueryable<out T> is IQueryable<out T>
```

See _[MS tutorial on async streams][async-stream-ms-doc]_ for more info.

# Remote.Linq.Async.Queryable

Provides interoperability with _Interactive Extensions ([Ix.NET][ix-net-repo] / [System.Linq.Async.Queryable][ix-net-async-queryable-package])_.

## How to Use

```C#
System.Linq.IAsyncQueryable<TEntity> asyncQuery =
  RemoteQueryable.Factory.CreateAsyncQueryable<TEntity>(...);
await foreach (TEntity item in asyncQuery)
{
}
```

# Remote.Linq.EntityFramework / Remote.Linq.EntityFrameworkCore

Remote linq extensions for _[Entity Framework][ef6-package]_ and _[Entity Framework Core][efcore-package]_.

These packages are used on server side to apply queries to EFs `DbContext`.

The only reason to include one of these packages in a client side project is to enable utilization of query features specific to _EF6_ and _EF Core_:

* Apply eager-loading (`Include`-expressions)

* Make use of DB functions</br>
  e.g. `queryable.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Name, "%fruit%"))`

## How to Use

### Client Code Sample

Query blogs including (i.e. eager loading) posts and owners

```C#
using var repository = new RemoteRepository();
var blogs = repository.Blogs
    .Include(x => x.Posts)
    .ThenInclude(x => x.Owner)
    .ToList();
```

### Server Code Sample

Execute query on database via _EF Core_

```C#
public DynamicObject ExecuteQuery(Expression queryExpression)
{
    using var dbContext = new DbContext();
    return queryExpression.ExecuteWithEntityFrameworkCore(dbContext);
}
```

# Remote.Linq.Newtonsoft.Json

Provides _[Json.NET][json-net-package]_ serialization settings for _Remote.Linq_ types.

## How to Use

```C#
// Serialization
TValue value = ...;
JsonSerializerSettings serializerSettings = new JsonSerializerSettings().ConfigureRemoteLinq(); 
string json = JsonConvert.SerializeObject(value, serializerSettings); 
TValue copy = JsonConvert.DeserializeObject<TValue>(json, serializerSettings); 
```

# Remote.Linq.Text.Json

Provides _[System.Text.Json][json-text-package]_ serialization settings for _Remote.Linq_ types.

## How to Use

```C#
// Serialization
TValue value = ...;
JsonSerializerOptions serializerOptions = new JsonSerializerOptions().ConfigureRemoteLinq();
string json = JsonSerializer.Serialize(value, serializerOptions);
TValue copy = JsonSerializer.Deserialize<TValue>(json, serializerOptions);
```

```C#
// Microsoft.AspNetCore.Hosting [WebHostBuilder]
new WebHostBuilder()
  .ConfigureServices(services => services
    .AddMvcCore()
    .AddJsonOptions(options => options.JsonSerializerOptions.ConfigureRemoteLinq()));
```

```C#
// Microsoft.AspNetCore.Hosting [WebApplicationBuilder]
var builder = WebApplication.CreateBuilder();
builder.Services
  .AddMvcCore()
  .AddJsonOptions(options => options.JsonSerializerOptions.ConfigureRemoteLinq());
```

```C#
// System.Net.Http.HttpClient
TValue value = ...;
Uri uri = ...;
var serializerOptions = new JsonSerializerOptions().ConfigureRemoteLinq();
using var httpClient = new HttpClient();
await httpClient.PostAsJsonAsync(uri, value, serializerOptions);
```

[1]: https://ci.appveyor.com/api/projects/status/64kw6dsuvfwyrdtl/branch/main?svg=true
[2]: https://ci.appveyor.com/project/6bee/remote-linq/branch/main
[3]: https://api.travis-ci.com/6bee/Remote.Linq.svg?branch=main
[4]: https://travis-ci.com/github/6bee/Remote.Linq?branch=main
[5]: https://codecov.io/gh/6bee/Remote.Linq/branch/main/graph/badge.svg
[6]: https://codecov.io/gh/6bee/Remote.Linq
[7]: https://app.codacy.com/project/badge/Grade/e13355ef6833454daa3860963025f270
[8]: https://app.codacy.com/gh/6bee/Remote.Linq/dashboard
[9]: https://www.codefactor.io/repository/github/6bee/Remote.Linq/badge
[10]: https://www.codefactor.io/repository/github/6bee/Remote.Linq
[13]: https://img.shields.io/nuget/v/Remote.Linq.svg
[14]: https://www.nuget.org/packages/Remote.Linq
[15]: https://img.shields.io/myget/aqua/vpre/Remote.Linq.svg?label=myget
[16]: https://www.myget.org/feed/aqua/package/nuget/Remote.Linq
[17]: https://img.shields.io/nuget/v/Remote.Linq.EntityFramework.svg
[18]: https://www.nuget.org/packages/Remote.Linq.EntityFramework
[19]: https://img.shields.io/myget/aqua/vpre/Remote.Linq.EntityFramework.svg?label=myget
[20]: https://www.myget.org/feed/aqua/package/nuget/Remote.Linq.EntityFramework
[21]: https://img.shields.io/nuget/v/Remote.Linq.EntityFrameworkCore.svg
[22]: https://www.nuget.org/packages/Remote.Linq.EntityFrameworkCore
[23]: https://img.shields.io/myget/aqua/vpre/Remote.Linq.EntityFrameworkCore.svg?label=myget
[24]: https://www.myget.org/feed/aqua/package/nuget/Remote.Linq.EntityFrameworkCore
[25]: https://img.shields.io/nuget/v/Remote.Linq.Newtonsoft.Json.svg
[26]: https://www.nuget.org/packages/Remote.Linq.Newtonsoft.Json
[27]: https://img.shields.io/myget/aqua/vpre/Remote.Linq.Newtonsoft.Json.svg?label=myget
[28]: https://www.myget.org/feed/aqua/package/nuget/Remote.Linq.Newtonsoft.Json
[29]: https://img.shields.io/nuget/v/Remote.Linq.protobuf-net.svg
[30]: https://www.nuget.org/packages/Remote.Linq.protobuf-net
[31]: https://img.shields.io/myget/aqua/vpre/Remote.Linq.protobuf-net.svg?label=myget
[32]: https://www.myget.org/feed/aqua/package/nuget/Remote.Linq.protobuf-net
[33]: https://img.shields.io/nuget/v/Remote.Linq.Async.Queryable.svg
[34]: https://www.nuget.org/packages/Remote.Linq.Async.Queryable
[35]: https://img.shields.io/myget/aqua/vpre/Remote.Linq.Async.Queryable.svg?label=myget
[36]: https://www.myget.org/feed/aqua/package/nuget/Remote.Linq.Async.Queryable

[lic-badge]: https://img.shields.io/github/license/6bee/Remote.Linq.svg
[lic-link]: https://github.com/6bee/Remote.Linq/blob/main/license.txt

[pub-badge]: https://github.com/6bee/Remote.Linq/actions/workflows/publish.yml/badge.svg
[pub-link]: https://github.com/6bee/Remote.Linq/actions/workflows/publish.yml

[async-stream-ms-doc]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/generate-consume-asynchronous-stream
[ef6-package]: https://www.nuget.org/packages/EntityFramework/
[efcore-package]: https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/
[infocarrier-repo]: https://github.com/azabluda/InfoCarrier.Core
[efcore-client-repo]: https://github.com/JohnGoldInc/EfCore.Client
[ix-net-repo]: https://github.com/dotnet/reactive
[ix-net-async-queryable-package]: https://www.nuget.org/packages/System.Linq.Async.Queryable/
[json-net-package]: https://www.nuget.org/packages/Newtonsoft.Json/
[json-text-package]: https://www.nuget.org/packages/System.Text.Json/
[re-linq-repo]: https://github.com/re-motion/Relinq
