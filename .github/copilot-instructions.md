# Copilot Instructions for MongoEntityFramework.AspNetCore.Identity

## Project Overview

This repository provides ASP.NET Core Identity providers for MongoDB.EntityFrameworkCore. It implements the ASP.NET Core Identity system using MongoDB as the backing store via the Entity Framework Core provider.

## Technology Stack

- **Language:** C# (latest LangVersion)
- **Framework:** .NET 8.0 and .NET 9.0 (multi-targeting)
- **Database:** MongoDB (v8.0+)
- **Identity:** ASP.NET Core Identity
- **ORM:** MongoDB.EntityFrameworkCore
- **Testing:** xUnit with AwesomeAssertions

## Repository Structure

```
/src
  /MongoFramework.AspNetCore.Identity - Main library source code
    - MongoIdentityUser.cs - User entity
    - MongoIdentityRole.cs - Role entity
    - MongoUserStore.cs - User store implementation
    - MongoRoleStore.cs - Role store implementation
    - MongoUserOnlyStore.cs - User-only store (no roles)
    - MongoIdentityDbContext.cs - DbContext for Identity
    - MongoIdentityBuilderExtensions.cs - Service collection extensions
/tests
  /MongoFramework.AspNetCore.Identity.Tests - xUnit test project
/examples
  /Net6Example - Sample .NET 6.0 web application
  /Net9Sample - Sample .NET 9.0 web application
/Net9MultiTenant - Multi-tenant example application (at root, but grouped with examples in .sln)
```

## Build and Test Commands

### Prerequisites
- .NET 8.0 and 9.0 SDKs installed
- MongoDB 8.0+ running locally (required for tests)

### Build
```bash
dotnet restore
dotnet build --no-restore -c Release
```

### Test
```bash
dotnet test --no-restore --no-build --verbosity normal
```

Note: Tests require a running MongoDB instance on the default port (27017).

## Code Standards and Conventions

- **Warnings as Errors:** `TreatWarningsAsErrors` is enabled - all warnings must be resolved
- **CLS Compliance:** Code must be CLS compliant (`CLSCompliant=true`)
- **Language Version:** Uses latest C# language features
- **Multi-targeting:** All library code must support both .NET 8.0 and 9.0
- **Testing:** Uses xUnit and AwesomeAssertions for test assertions

## Key Dependencies

### .NET 9.0
- Microsoft.Extensions.Identity.Core v9.0.10
- Microsoft.Extensions.Identity.Stores v9.0.10
- MongoDB.EntityFrameworkCore v9.0.3

### .NET 8.0
- Microsoft.Extensions.Identity.Core v8.0.21
- Microsoft.Extensions.Identity.Stores v8.0.21
- MongoDB.EntityFrameworkCore v8.3.3

## Architecture Patterns

### Identity Store Implementations

The library implements ASP.NET Core Identity interfaces:
- `IUserStore<TUser>` - Core user storage
- `IRoleStore<TRole>` - Core role storage
- `IUserRoleStore<TUser>` - User-role relationships
- `IUserClaimStore<TUser>` - User claims
- `IUserLoginStore<TUser>` - External logins
- `IUserPasswordStore<TUser>` - Password hashing
- `IUserSecurityStampStore<TUser>` - Security stamps
- And many more Identity-related stores

### Entity Framework Core Integration

- Uses MongoDB.EntityFrameworkCore provider
- DbContext classes: `MongoIdentityDbContext` (with roles), `MongoIdentityUserContext` (users only)
- Entities inherit from Identity base classes and add MongoDB-specific ID handling

### Service Registration

Uses extension methods on `IdentityBuilder`:
```csharp
builder.Services.AddDefaultIdentity<MongoIdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddMongoEntityFrameworkStores<ApplicationDbContext>();
```

## Important Implementation Details

1. **Identity Specification Tests**: The project includes ASP.NET Core's Identity specification tests (IdentitySpecificationBase) to ensure compliance with the Identity contract.

2. **Check Class**: Uses a `Check` utility class for argument validation - prefer this over manual null checks.

3. **MongoDB ID Handling**: Entities use `ObjectId` from MongoDB.Bson for primary keys.

4. **Async Patterns**: All store methods follow async/await patterns with proper cancellation token support.

## Common Tasks

### Adding a New Store Method
1. Implement the interface method in the appropriate store class (`MongoUserStore`, `MongoRoleStore`, etc.)
2. Use Entity Framework Core patterns for data access
3. Add corresponding unit tests in the tests project
4. Ensure cancellation token is properly threaded through
5. Include appropriate null checks using the `Check` class

### Updating Dependencies
- Dependencies are version-specific per target framework (net8.0 vs net9.0)
- Update both framework conditions in .csproj files
- Ensure compatibility with both MongoDB.EntityFrameworkCore versions

### Adding Tests
- Place tests in appropriate subdirectories under `tests/MongoFramework.AspNetCore.Identity.Tests/`
- Group related tests in subdirectories (e.g., `MongoUserStoreTests/`)
- Use AwesomeAssertions for more readable assertions
- Follow existing test naming patterns

## CI/CD

- **Testing:** GitHub Actions runs tests on Ubuntu with MongoDB 8.0
- **Publishing:** Automated NuGet package publishing via release-please
- **Multi-targeting:** Tests run against both .NET 8.0 and 9.0

## Package Information

- **Package ID:** jcamp.MongoEntityFramework.AspNetCore.Identity
- **License:** MIT
- **Author:** John Campion
- **Tags:** mongodb, mongoframework, mongo, identity

## When Making Changes

1. Ensure changes work for both .NET 8.0 and 9.0 target frameworks
2. Run all tests before committing (`dotnet test`)
3. Maintain backward compatibility where possible
4. Update XML documentation comments for public APIs
5. Follow existing patterns in the codebase
6. Consider if Identity specification tests need updates
