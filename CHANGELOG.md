# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased vNext][vnext-unreleased]

### Added

### Changed

- Improved and extended API for fluent configuration of custom strategies for expression execution
  (namespace `Remote.Linq.ExpressionExecution`).
- Bump _Microsoft.EntityFramework_ from 6.4.4 to 6.5.1 (concerns _Remote.Linq.EntityFramework_)
- Bump _Microsoft.EntityFrameworkCore_ from 8.0.6 to 8.0.7 (net8.0) (concerns _Remote.Linq.EntityFrameworkCore_)

### Deprecated

### Removed

### Fixed

### Security

## [7.2.1][7.2.1] - 2024-07-10

### Security

- Bump _aqua-core_ from 5.4.0 to 5.4.1 ([CVE-2024-30105][CVE-2024-30105])

## [7.2.0][7.2.0] - 2024-06-04

### Added

- Add .NET 8.0 framework target
- Add `SystemExpression.Factory`

### Changed

- Bump _aqua-core_ from 5.2.0 to 5.4.0
- Bump _Microsoft.Bcl.AsyncInterfaces_ from 6.0.0 to 8.0.0
- Bump _Microsoft.EntityFrameworkCore_ from 6.0.10 to 7.0.20 (net6.0) and 8.0.6 (net8.0) (concerns _Remote.Linq.EntityFrameworkCore_)

### Removed

- Binary serialization removed for .NET 8.0 and later ([SYSLIB0050: Formatter-based serialization is obsolete][syslib0050])
- Removed package reference for _System.Linq.Expressions_

### Fixed

- Pass `ITypeResolver` to remote expression visitors to avoid potential duplication of emitting dynamic type

## [7.1.0][7.1.0] - 2022-11-11

### Changed

- Added optional result mapper argument to all _RemoteQueryable.Factory_ methods.
- Re-ordered method arguments to be consistent for all _RemoteQueryable.Factory_ methods.

### Removed

- Removed various types and methods previously marked as obsolete.

### Fixed

- Fixed issue with subqueries with EF Core [#112][issue#112]

## [7.0.0][7.0.0] - 2021-09-29

### Added

- Added support for [async queryable (Ix.NET)][async-queryable].
- Added support for [async streams][async-streams] ([IAsyncDisposable][iasyncdisposable]).
- Added support for [filtered include][ef-filtered-include] queryable extensions.
- Added support for [protobuf-net v2][protobuf-net-v2] serialization.
- Added support for _System.Text.Json_ serialization.
- Introduced `IExpressionTranslatorContext` interface to bundle parameterization options.
- Introduced `QueryArgumentAttribute` to annotate types to prevent local evaluation (i.e. substitution of constant expression value) when translating expressions.
- Introduced `QueryMarkerFunctionAttribute` to annotate methods to prevent local evaluation (i.e. execution of the method) when translating expressions.

### Changed

- Migrated to [nullable reference types][nullable-references].
- Moved _async_ queryable extension methods to namespace _Remote.Linq.Async_.
- Moved expression _execute_ extension methods to namespace _Remote.Linq.ExpressionExecution_.
- Moved `Include` and `ThenInclude` queryable extensions to namespace _Remote.Linq.Include_.
- Moved types `Query` and `Query<T>` to namespace _Remote.Linq.SimpleQuery_.
- Revised `RemoteQueryable.Factory` methods:
  - Renamed methods to `CreateQueryable`, `CreateAsyncQueryable`, `CreateAsyncStreamQueryable`, etc.
  - Introduced `IExpressionToRemoteLinqContext` argument for parameterization.
- Revised expression execution methods and types:
  - Introduced `IExpressionFromRemoteLinqContext` argument for parameterization.

### Fixed

- Various minor API improvements and bug fixes.

## [6.3.1][6.3.1] - 2021-08-29

### Fixed

- Fixed issue with async `IQueryable` extensions methods
  - `SequenceEqualAsync` (without comparer)
  - `SumAsync` (for Int64)

## [6.3.0][6.3.0] - 2021-01-16

### Added

- Added target framework `netstandard2.1` for _Remote.Linq.EntityFramework_.
- Added support for `ThenInclude` queryable extensions (_EF6_ and _EFCore_).

### Removed

- Dropped unused dependency on _System.Runtime.Serialization.Formatters_.

[vnext-unreleased]: https://github.com/6bee/Remote.Linq/compare/v7.2.1...main
[7.2.1]: https://github.com/6bee/Remote.Linq/compare/v7.2.0...v7.2.1
[7.2.0]: https://github.com/6bee/Remote.Linq/compare/7.1.0...v7.2.0
[7.1.0]: https://github.com/6bee/Remote.Linq/compare/7.0.0...7.1.0
[7.0.0]: https://github.com/6bee/Remote.Linq/compare/6.3.1...7.0.0
[6.3.1]: https://github.com/6bee/Remote.Linq/compare/6.3.0...6.3.1
[6.3.0]: https://github.com/6bee/Remote.Linq/compare/6.2.3...6.3.0

[issue#112]: https://github.com/6bee/Remote.Linq/issues/112

[async-queryable]: https://www.nuget.org/packages/System.Linq.Async.Queryable/
[async-streams]: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/generate-consume-asynchronous-stream
[ef-filtered-include]: https://docs.microsoft.com/en-us/ef/core/querying/related-data/eager#filtered-include
[iasyncdisposable]: https://docs.microsoft.com/en-us/dotnet/api/system.iasyncdisposable
[nullable-references]: https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references
[protobuf-net-v2]: https://www.nuget.org/packages/protobuf-net/2.4.6
[syslib0050]: https://learn.microsoft.com/en-us/dotnet/fundamentals/syslib-diagnostics/syslib0050
[CVE-2024-30105]: https://github.com/advisories/GHSA-hh2w-p6rv-4g7w
