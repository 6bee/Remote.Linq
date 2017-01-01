# Remote.Linq

| branch | package | AppVeyor | Travis CI |
| --- | --- | --- | --- |
| `master` | [![NuGet Badge](https://buildstats.info/nuget/Remote.Linq?includePreReleases=true)](http://www.nuget.org/packages/Remote.Linq) [![MyGet Pre Release](http://img.shields.io/myget/aqua/vpre/Remote.Linq.svg?style=flat-square&label=myget)](https://www.myget.org/feed/aqua/package/nuget/Remote.Linq) | [![Build status](https://ci.appveyor.com/api/projects/status/64kw6dsuvfwyrdtl?svg=true)](https://ci.appveyor.com/project/6bee/remote-linq) | [![Travis build Status](https://travis-ci.org/6bee/Remote.Linq.svg?branch=master)](https://travis-ci.org/6bee/Remote.Linq?branch=master) |


Remote Linq is a small and easy to use - yet very powerful - library to translate linq expression trees to strongly typed, serializable expression trees and vice versa. It provides functionality to send arbitrary linq queries to a remote service to be applied and executed against any enumerable or queryable data collection.

Building a LINQ interface for custom services is made a breeze by using Remote Linq.


Features:
* Translate linq expressions into serializable expression trees (remote linq expression) and vice versa. 
* Build remote single-type query services (paging, sorting, filtering)
* Build remote complex linq query services (arbitrary linq query including joins, groupings, aggregations, projections, etc.)
