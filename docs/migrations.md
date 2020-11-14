# Migrations

**Migrations are automatically run before the API starts up**  
(See `Program.cs`, and the corrosponding `SeedDatabaseAsync` extension method for the implementation)

**The site will only startup once the migrations are complete.**

1. [Adding new migrations using the package manager console](Addingnewmigrationsusingthepackagemanagerconsole)
    * 1.1. [OpenEventSourcingProjectionDbContext](#OpenEventSourcingProjectionDbContext)
    * 1.2. [OpenEventSourcingDbContext](#OpenEventSourcingDbContext)
2. [Adding new migrations using the command line](#Addingnewmigrationsusingthecommandline)
    * 2.1. [OpenEventSourcingProjectionDbContext](#OpenEventSourcingProjectionDbContext-1)
    * 2.2. [OpenEventSourcingDbContext](#OpenEventSourcingDbContext-1)

##  1. <a name='Addingnewmigrationsusingthepackagemanagerconsole'></a>Adding new migrations using the package manager console

####  1.1. <a name='OpenEventSourcingProjectionDbContext'></a>OpenEventSourcingProjectionDbContext

```
Add-Migration <MigrationName> -c OpenEventSourcingProjectionDbContext -p SIO.Migrations -o Migrations/OpenEventSourcing/Projection
```
####  1.2. <a name='OpenEventSourcingDbContext'></a>OpenEventSourcingDbContext

```
Add-Migration <MigrationName> -c OpenEventSourcingDbContext -p SIO.Migrations -o Migrations/OpenEventSourcing/Store
```

##  2. <a name='Addingnewmigrationsusingthecommandline'></a>Adding new migrations using the command line

####  2.1. <a name='OpenEventSourcingProjectionDbContext-1'></a>OpenEventSourcingProjectionDbContext

```
dotnet ef migrations add <MigrationName> -c OpenEventSourcingProjectionDbContext -p SIO.Migrations -o Migrations/OpenEventSourcing/Projection
```
####  2.2. <a name='OpenEventSourcingDbContext-1'></a>OpenEventSourcingDbContext

```
dotnet ef migrations add <MigrationName> -c OpenEventSourcingDbContext -p SIO.Migrations -o Migrations/OpenEventSourcing/Store
```