# MS SQL Server Storage
Starting from v6 DbLocalizationProvider supports configurable storage.
It has no dependency anymore on EntityFramework or EFCore, but instead developer can choose what storage suits, add it, configfure and use it.

To add MSSQL storage you need to install package:

```
> dotnet add package DbLocalizationProvider.Storage.SqlServer
```

Once this is done, you need to configure connectionString for the SQL Server package (usually in your `Startup.cs` file):

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbLocalizationProvider(ctx =>
    {
        ...
        ctx.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
    });
}
```

For now only MSSQL Server as storage is implemented, but overtime I guess new storage implementations will be added to the project.
