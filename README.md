# Remote.Linq

| branch | package | AppVeyor | Travis CI |
| --- | --- | --- | --- |
| `master` | [![NuGet Badge](https://buildstats.info/nuget/Remote.Linq?includePreReleases=true)](http://www.nuget.org/packages/Remote.Linq) [![MyGet Pre Release](http://img.shields.io/myget/aqua/vpre/Remote.Linq.svg?style=flat-square&label=myget)](https://www.myget.org/feed/aqua/package/nuget/Remote.Linq) | [![Build status](https://ci.appveyor.com/api/projects/status/64kw6dsuvfwyrdtl?svg=true)](https://ci.appveyor.com/project/6bee/remote-linq) | [![Travis build Status](https://travis-ci.org/6bee/Remote.Linq.svg?branch=master)](https://travis-ci.org/6bee/Remote.Linq?branch=master) |

### Description:
Remote Linq is a small and easy to use - yet very powerful - library to translate LINQ expression trees to strongly typed, serializable expression trees and vice versa. It provides functionality to send arbitrary LINQ queries to a remote service to be applied and executed against any enumerable or queryable data collection.

Building a LINQ interface for custom services is made a breeze by using Remote Linq.


### Features:
* Translate LINQ expressions into serializable expression trees (remote LINQ expression) and vice versa. 
* Build remote single-type query services (paging, sorting, filtering)
* Build remote complex LINQ query services (arbitrary LINQ query including joins, groupings, aggregations, projections, etc.)

### Scope:
In contrast to [re-linq](https://github.com/re-motion/Relinq), this project enables serialization and deserialization of expression trees and applying LINQ expressions to other LINQ providers e.g. linq-to-object, linq-to-entity, etc. 

This is typically used to store and reload or simply transfer expressions to a service where it’s applied against a data source for querying. 

The API makes it super easy to implement a custom service allowing LINQ queries defined on a client to be executed on a server. 

## Sample

### Client

Implement repository class setting-up server connection and providing the queryable data sets (`IQueryable<>`)
```C#
public class RemoteRepository
{
    private readonly Func<Expression, IEnumerable<DynamicObject>> _dataProvider;

    public RemoteRepository(string uri)
    {
        _dataProvider = expression =>
            {
                // setup service connectivity
                IQueryService service = CreateServerConection(uri);
                // send expression to service to get back results
                IEnumerable<DynamicObject> result = service.ExecuteQuery(expression);
                return result;
            };
    }

    public IQueryable<Blog> Blogs => RemoteQueryable.Create<Blog>(_dataProvider);
   
    public IQueryable<Post> Posts => RemoteQueryable.Create<Post>(_dataProvider);
   
    public IQueryable<User> Users => RemoteQueryable.Create<User>(_dataProvider);
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

### Server

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
