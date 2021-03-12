# API Versioning using information from Accept Header

For this you need tell ASP NET Core to actually read the version information from the header. So go back to your `Startup.cs` and then edit the Versioning configuration. 

```csharp
services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true; 
        options.DefaultApiVersion = ApiVersion.Default;
        options.ApiVersionReader = new MediaTypeApiVersionReader("version");
    });
```

The last configuration that reads `options.ApiVersionReader = new MediaTypeApiVersionReader("version");` is what tells ASP NET Core where to read the version information from. MediaType is read from the Accept header. 

Now run your API locally and then go to your favourite REST Client and make a request. 

You will now notice that the earlier query string param url still gets mapped to version 1.0!

So how do you make a request for version 2.0 now that you have configured ASP NET Core to read from MediaType?

```shell
curl --location --request GET 'https://localhost:44314/weatherforecast' \
--header 'Accept: application/json; version=2.0'
```

So you mention the version in the Accept Header and ASP Net Core maps to the right version for you!

### How do I get this version of the solution?

In order to get the version of the code with this setup, run this on your git bash, while on `main` branch:

```shell
git checkout -b versioning/acceptheader

git reset --hard 5f074a21
```

You should now have this branch on the commit, where you can run your solution for query string versioning.
[Explore this repository at that commit on Github](https://github.com/lonelydev/CoolProductApi/tree/5f074a21fc8f07526e445d35234eb2dd41f66be3)

[Back](./AspNetCoreApiVersioning.md)