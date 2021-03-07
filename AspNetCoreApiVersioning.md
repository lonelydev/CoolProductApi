# ASP NET Core API versioning


## The API

First let us create a new dotnet core API project. By default the template creates a Weather Forecast API. 


## Install the package

First install [`Microsoft.AspNetCore.Mvc.Versioning`](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Versioning/5.0.0?_src=template) in your project. 

Then in order to start using it, go to your `Startup.cs`

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddApiVersioning();
}
```
