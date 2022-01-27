# ef-core-unit-of-work

[![build-and-tests](https://github.com/Alelho/ef-core-unit-of-work/actions/workflows/build-and-tests.yml/badge.svg)](https://github.com/Alelho/ef-core-unit-of-work/actions/workflows/build-and-tests.yml)
[![build-and-publish](https://github.com/Alelho/ef-core-unit-of-work/actions/workflows/build-and-publish.yml/badge.svg)](https://github.com/Alelho/ef-core-unit-of-work/actions/workflows/build-and-publish.yml)
[![Coverage Status](https://coveralls.io/repos/github/Alelho/ef-core-unit-of-work/badge.svg)](https://coveralls.io/github/Alelho/ef-core-unit-of-work?branch=ef-core-unit-of-work-5)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

This is an implementation of the following patterns, unit of work and generic repository for .NET Core. Unit of work provides a way to execute a bunch of operations (insert, update, delete and so on kinds) in a single transaction. The generic repository provides a set of basic operations like insert, find, update, etc for each database entity.

---

| Package | .NET Core | NuGet |
|---|---|---|
| EfCoreUnitOfWork | 5.x.x | ![Nuget](https://img.shields.io/nuget/v/EFCoreUnitOfWork) |

---

## Give a star! :star:

If you liked it or if this project helped you in any way, please give a star. Thanks!

## How to install
The package is available on the NuGet gallery. Run the command below to install in your project:

```
Install-Package EFCoreUnitOfWork -Version 5.0.0
```

## How to use
