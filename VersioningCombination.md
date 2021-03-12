# Combining multiple ways of requesting versions

So far we have been looking at ways to explicitly support a certain type of Header in the request to direct it to the right version of the API endpoint. 
ASP .NET also allows you to support a combination of them. Let us say some clients would love to use the Accept header while others prefer the custom header route. You could please both groups of your clients by letting them specify versions in either way.

To do this we go back to our `Startup.cs` and change the configuration to:

```csharp
services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true; 
        options.DefaultApiVersion = ApiVersion.Default;
        options.ApiVersionReader = ApiVersionReader.Combine(
        new HeaderApiVersionReader("e-version"),
        new MediaTypeApiVersionReader("version")
        );
    });
```

Now if you were to run your application, you could use either the accept header or the custom header to specify which version of the endpoint you want to use!

#### Custom Header version request

```shell
curl --location --request GET 'https://localhost:44314/weatherforecast' \
--header 'Accept: application/json' \
--header 'e-version: 2.0'
```

#### Accept Header version request

```shell
curl --location --request GET 'https://localhost:44314/weatherforecast' \
--header 'Accept: application/json; version=2.0'
```

#### What happens when you combine both?

```shell
curl --location --request GET 'https://localhost:44314/weatherforecast' \
--header 'Accept: application/json; version=1.0' \
--header 'e-version: 2.0'
```

You get an error.

```json
{
    "error": {
        "code": "AmbiguousApiVersion",
        "message": "The following API versions were requested: 2.0, 1.0. At most, only a single API version may be specified. Please update the intended API version and retry the request.",
        "innerError": null
    }
}
```

At least your API is clear on what it will accept. 

### How do I get this version of the solution?

In order to get the version of the code with this setup, run this on your git bash:

```shell
git checkout -b versioning/customheader

git reset --hard 6c7615ec993e7aa09c1ed4ab6f441df5f9a81bb9
```

You should now have this branch on the commit, where you can run your solution for query string versioning.
[Explore this repository at that commit on Github](https://github.com/lonelydev/CoolProductApi/tree/6c7615ec993e7aa09c1ed4ab6f441df5f9a81bb9)

[Back](./AspNetCoreApiVersioning.md)