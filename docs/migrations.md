# Migrations

**Migrations are automatically run before the API starts up**  
(See `Program.cs`, and the corrosponding `SeedDatabaseAsync` extension method for the implementation)

**The site will only startup once the migrations are complete.**

1. [Adding new migrations using the package manager console](Addingnewmigrationsusingthepackagemanagerconsole)
    * 1.1. [SIOTranslatorDbContext](#SIOTranslatorDbContext)
2. [Adding new migrations using the command line](#Addingnewmigrationsusingthecommandline)
    * 2.1. [SIOTranslatorDbContext](#SIOTranslatorDbContext-1)

##  1. <a name='Addingnewmigrationsusingthepackagemanagerconsole'></a>Adding new migrations using the package manager console

####  1.1. <a name='SIOTranslatorDbContext'></a>SIOTranslatorDbContext

```
Add-Migration <MigrationName> -c SIOTranslatorDbContext -p SIO.Migrations -o Migrations/SIO
```

##  2. <a name='Addingnewmigrationsusingthecommandline'></a>Adding new migrations using the command line

####  2.1. <a name='SIOTranslatorDbContext-1'></a>SIOTranslatorDbContext

```
dotnet ef migrations add <MigrationName> -c SIOTranslatorDbContext -p SIO.Migrations -o Migrations/SIO
```