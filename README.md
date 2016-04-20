# Remote.Linq

Remote Linq is a small and easy to use - yet very powerful - library to translate linq expression trees to strongly typed, serializable expression trees and vice versa. It provides functionality to send arbitrary linq queries to a remote service to be applied and executed against any enumerable or queryable data collection.

Building a LINQ interface for custom services is made a breeze by using Remote Linq.


Features:
* Translate linq expressions into serializable expression trees (remote linq expression) and vice versa. 
* Build remote single-type query services (paging, sorting, filtering)
* Build remote complex linq query services (arbitrary linq query including joins, groupings, aggregations, projections, etc.)


```
* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
Get Latest Version from NuGet
PM> Install-Package Remote.Linq

https://www.nuget.org/packages/Remote.Linq/ 
* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
```