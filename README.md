![alt text](https://raw.githubusercontent.com/jcamp-code/MongoEntityFramework.AspNetCore.Identity/main/assets/mongoidentity_logo_64x64.png "Mongo Identity")

# MongoEntityFramework.AspNetCore.Identity

Asp.Net Core Identity providers for [MongoDB.EntityFrameworkCore](https://github.com/mongodb/mongo-efcore-provider).

## Features

MongoFramework Implementations

- IdentityUser
- IdentityRole
- RoleStore
- UserStore
- UserOnlyStore

ServiceCollection Extensions for

- Identity Stores (adds to IdentityBuilder)

```cs
builder.Services.AddDefaultIdentity<MongoIdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddMongoEntityFrameworkStores<ApplicationDbContext>();
```

Sample .NET 9.0 Project shows a fully working example

Unit Tests, including passing Asp.Net Core's IdentitySpecificationBase

## IdentitySpec Tests

[This issue](https://github.com/dotnet/aspnetcore/issues/27873) shows the spec tests weren't
publicly released for .NET 5.0. They are supposed to be, but do not show up on NuGet yet.
I have added the code manually to the test project until this gets published.
