# Introduction to Entity Framework Core

In this module you will learn how to define a data structure using Entity Framework (EF) Core.
https://docs.microsoft.com/en-us/ef/core/

With EF Core, data access is performed using a model. A model is made up of entity classes
and a context object that represents a session with the database.  The context object allows querying and saving data. If you're using a relational database, EF Core can create and update tables for your entities via migrations.

## Topics
 - [Creating and configuring a model](https://docs.microsoft.com/en-us/ef/core/modeling/)
 - [DbContext configuration](https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/)
 - Fluent API vs Data Annotations
 - [Primary & alternate keys](https://docs.microsoft.com/en-us/ef/core/modeling/keys?tabs=fluent-api)
 - [Indexes](https://docs.microsoft.com/en-us/ef/core/modeling/indexes?tabs=fluent-api)
 - [Spatial Data](https://docs.microsoft.com/en-us/ef/core/modeling/spatial)
 - [Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)

## Prerequisites
You'll need to set up your machine to run .NET, including the C# 10.0 compiler. [Download](https://dotnet.microsoft.com/download/dotnet/6.0) & install the .NET 6 SDK.

This tutorial assumes you're familiar with C#, .NET and the concept of object-relational mapping (ORM). 

It is recommended you read through the [Entity Framework Core documentation](https://docs.microsoft.com/en-us/ef/core/) before starting the assignments.

## Requirements

We are going to create an application that keeps track of the modes of transport that a company's employees use to commute to work daily. The full solution will include:
- Data project: defines the data structure
- Domain project: business logic implementations
- WebApplication project: includes the web API's & backoffice web application to manage the employee data
- Mobile app project: will be used by the employees to input the mode of travel for their daily commute

Let's start with implementing the data layer.

## Assignments

### 1. Create a class library project
 - Create a new class library project, targeting .NET6
 - [Opt in the nullable reference types feature](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/nullable-reference-types#create-the-application-and-enable-nullable-reference-types)
### 2. Install the necessary NuGet packages
https://docs.microsoft.com/en-us/ef/core/get-started/overview/install

To add EF Core to an application, install the NuGet package for the database provider you want to use.

To install or update NuGet packages, you can use the .NET Core command-line interface (CLI), the Visual Studio Package Manager Dialog, or the Visual Studio Package Manager Console.

NuGet packages to install:
 - [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/)
 - [Microsoft.EntityFrameworkCore.Design](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Design)
 - [Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer)
 - [Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite)

### 3. Create a DataContext class
https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli#create-the-model

### 4. Set up an SQL Server database
To set up SQL Server on your development environment, go to https://www.microsoft.com/en-us/sql-server/developer-get-started/ and select the programming language (C#) & the operating system of your development environment. 

Then, simply follow the instructions to set up a SQL Server locally.

### 5. Configure the DataContext to use the SqlServer database
After you have the SQL Server set up, you should have a database up & running on your local machine. 
Now configure the DataContext with the connection string, and enable mapping to spatial types via NetTopologySuite (NTS)
https://docs.microsoft.com/en-us/ef/core/modeling/spatial#nettopologysuite

### 6. Create a BaseEntity class
The BaseEntity entity class will serve as a base type for other entity types.
The BaseEntity entity has 3 properties:
- Id ([Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid))
- CreatedAt ([DateTime](https://docs.microsoft.com/en-us/dotnet/api/system.datetime))
- UpdatedAt ([DateTime](https://docs.microsoft.com/en-us/dotnet/api/system.datetime))

### 7. Create an Employee entity class
The Employee entity class inherits from BaseEntity and has properties:
- Id (inherited from BaseEntity)
- CreatedAt (inherited from BaseEntity)
- UpdatedAt (inherited from BaseEntity)
- Name ([String](https://docs.microsoft.com/en-us/dotnet/api/system.string), max. 128 characters, index)
- Email ([String](https://docs.microsoft.com/en-us/dotnet/api/system.string), max. lenght 128 characters, unique index)
- HomeLocation ([Point](http://nettopologysuite.github.io/NetTopologySuite/api/NetTopologySuite.Geometries.Point.html?q=Point))
- DefaultWorkLocation ([Point](http://nettopologysuite.github.io/NetTopologySuite/api/NetTopologySuite.Geometries.Point.html?q=Point))
- DefaultCommuteType (enum CommuteType, see below)
- Commutes (one-to-many relation with Commute entity)

```c#
public enum CommuteType
{
    Undefined,
    Bike,
    Car,
    CarpoolDriver,
    CarpoolPassenger,
    PublicTransport,
    Train,
    Walking,
}
```

### 8. Create a Commute class
The Commute entity will be used to keep track of the daily commutes reported by employees. Each Employee can have many Commutes.

The Commute entity class inherits from BaseEntity and has properties:
- Id (inherited from BaseEntity)
- CreatedAt (inherited from BaseEntity)
- UpdatedAt (inherited from BaseEntity)
- Employee (relation with Employee entity)
- CommuteType (enum CommuteType, see previous step)

### 9. Create a migration to initialize the database structure
https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli

### 10. Override SaveChangesAsync to automatically set CreatedAt and/or UpdatedAt fields on saving changes
To automatically set the CreatedAt & UpdatedAt fields to the current timestamp, you can override the SaveChangesAsync method in the DataContext class.

Add the following method to the DataContext class:
``` c#
/// <summary>
/// Automatically set CreatedAt and/or UpdatedAt fields on saving changes
/// </summary>
/// <param name="cancellationToken"></param>
/// <returns></returns>
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
{
    var entries = ChangeTracker
        .Entries()
        .Where(e => e.Entity is BaseEntity && e.State is EntityState.Added or EntityState.Modified);

    foreach (var entityEntry in entries)
    {
        ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

        if (entityEntry.State == EntityState.Added)
        {
            ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
        }
    }
    return await base.SaveChangesAsync(true, cancellationToken);
}
```
