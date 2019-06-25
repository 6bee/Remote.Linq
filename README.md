# Remote.Linq

| branch | AppVeyor | Travis CI |
| --- | --- | --- |
| `master` | [![Build status](https://ci.appveyor.com/api/projects/status/64kw6dsuvfwyrdtl/branch/master?svg=true)](https://ci.appveyor.com/project/6bee/remote-linq/branch/master) | [![Travis build Status](https://travis-ci.org/6bee/Remote.Linq.svg?branch=master)](https://travis-ci.org/6bee/Remote.Linq?branch=master) |


| package | nuget | myget |
| --- | --- | --- |
| `Remote.Linq` | [![NuGet Badge](https://buildstats.info/nuget/Remote.Linq?includePreReleases=true)](http://www.nuget.org/packages/Remote.Linq) | [![MyGet Pre Release](http://img.shields.io/myget/aqua/vpre/Remote.Linq.svg?style=flat-square&label=myget)](https://www.myget.org/feed/aqua/package/nuget/Remote.Linq) |
| `Remote.Linq.EntityFramework` | [![NuGet Badge](https://buildstats.info/nuget/Remote.Linq.EntityFramework?includePreReleases=true)](http://www.nuget.org/packages/Remote.Linq.EntityFramework) | [![MyGet Pre Release](http://img.shields.io/myget/aqua/vpre/Remote.Linq.EntityFramework.svg?style=flat-square&label=myget)](https://www.myget.org/feed/aqua/package/nuget/Remote.Linq.EntityFramework) |
| `Remote.Linq.EntityFrameworkCore` | [![NuGet Badge](https://buildstats.info/nuget/Remote.Linq.EntityFrameworkCore?includePreReleases=true)](http://www.nuget.org/packages/Remote.Linq.EntityFrameworkCore) | [![MyGet Pre Release](http://img.shields.io/myget/aqua/vpre/Remote.Linq.EntityFrameworkCore.svg?style=flat-square&label=myget)](https://www.myget.org/feed/aqua/package/nuget/Remote.Linq.EntityFrameworkCore) |
| `Remote.Linq.Newtonsoft.Json` | [![NuGet Badge](https://buildstats.info/nuget/Remote.Linq.Newtonsoft.Json?includePreReleases=true)](http://www.nuget.org/packages/Remote.Linq.Newtonsoft.Json) | [![MyGet Pre Release](http://img.shields.io/myget/aqua/vpre/Remote.Linq.Newtonsoft.Json.svg?style=flat-square&label=myget)](https://www.myget.org/feed/aqua/package/nuget/Remote.Linq.Newtonsoft.Json) |


### Description
Remote Linq is a small and easy to use - yet very powerful - library to translate LINQ expression trees to strongly typed, serializable expression trees and vice versa. It provides functionality to send arbitrary LINQ queries to a remote service to be applied and executed against any enumerable or queryable data collection.

Building a LINQ interface for custom services is made a breeze by using Remote Linq.


### Features
* Translate LINQ expressions into serializable expression trees (remote LINQ expression) and vice versa. 
* Build remote single-type query services (paging, sorting, filtering)
* Build remote complex LINQ query services (arbitrary LINQ query including joins, groupings, aggregations, projections, etc.)

### Scope
In contrast to [re-linq](https://github.com/re-motion/Relinq), this project enables serialization and deserialization of expression trees and applying LINQ expressions to other LINQ providers e.g. linq-to-object, linq-to-entity, etc. 

This is typically used to store and reload or simply transfer expressions to a service where it’s applied against a data source for querying. 

The API makes it super easy to implement a custom service allowing LINQ queries defined on a client to be executed on a server. 

Write operations (insert/update/delete) have to be implemented by other means if needed. [InfoCarrier.Core](https://github.com/azabluda/InfoCarrier.Core) might be interesting for such scenario.

### Sample

#### Client

Implement repository class, setting-up server connection and providing the queryable data sets (`IQueryable<>`)
```C#
public class RemoteRepository
{
    private readonly Func<Expression, IEnumerable<DynamicObject>> _dataProvider;

    public RemoteRepository(string uri)
    {
        _dataProvider = expression =>
            {
                // setup service connectivity
                IQueryService service = CreateServerConnection(uri);
                // send expression to service and get back results
                IEnumerable<DynamicObject> result = service.ExecuteQuery(expression);
                return result;
            };
    }

    public IQueryable<Blog> Blogs => RemoteQueryable.Factory.CreateQueryable<Blog>(_dataProvider);
   
    public IQueryable<Post> Posts => RemoteQueryable.Factory.CreateQueryable<Post>(_dataProvider);
   
    public IQueryable<User> Users => RemoteQueryable.Factory.CreateQueryable<User>(_dataProvider);
}
```

Use your repository to compose LINQ query and let the data be retrieved from the backend service
```C#
var repository = new RemoteRepository();

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

#### Server

Implement the backend service, handling the client's query expression by applying it to a data source e.g. an ORM

```C#
public interface IQueryService
{
    IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression);
}

public class QueryService : IQueryService
{
    // any linq provider e.g. entity framework, nhibernate, ...
    private IDataProvider _datastore = new ObjectRelationalMapper();

    // you need to be able to retrieve an IQueryable by type
    private Func<Type, IQueryable> _queryableProvider = type => _datastore.GetQueryableByType(type);


    public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
    {
        // Execute is an extension method provided by Remote.Linq
        return queryExpression.Execute(queryableProvider: _queryableProvider);
    }

    public void Dispose()
    {
        _datastore.Dispose();
    }
}
```

## Remote.Linq.EntityFramework / Remote.Linq.EntityFrameworkCore

Remote linq extensions for entity framework and entity framework core. 

Use this package when using features specific to EF6 and EF Core:
- Apply eager-loading (`Include`-expressions)
- Make use of DB functions e.g. `queryable.Where(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Name, "%fruit%"))`

### Sample

#### Client

Query blogs including posts and owner

```C#
using (var repository = new RemoteRepository())
{
  var blogs = repository.Blogs
    .Include("Posts")
    .Include("Owner")
    .ToList();
}
```

#### Server

Execute query on database via EF Core

```C#
public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
{
  using (var dbContext = new DbContext())
  {
    return queryExpression.ExecuteWithEntityFrameworkCore(dbContext);
  }
}
```

## Remote.Linq.Newtonsoft.Json

Provides [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) serialization settings for Remote.Linq types.

### Sample

```C#
public T Demo<T>(T expression) where T : Remote.Linq.Expressions.Expression
{
  JsonSerializerSettings serializerSettings = new JsonSerializerSettings().ConfigureRemoteLinq();
 
  string json = JsonConvert.SerializeObject(expression, serializerSettings);
 
  T result = JsonConvert.DeserializeObject<T>(json, serializerSettings);
 
  return result;
}
```
