# Respond to every API request with information about versions

Wouldn't it be nice if you could tell your API users about what versions you support and related information? Well, that is possible too. 

Back in your `Startup.cs` file, 

```csharp
services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true; 
        options.DefaultApiVersion = ApiVersion.Default;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new HeaderApiVersionReader("e-version"),
            new MediaTypeApiVersionReader("version")
        );
        options.ReportApiVersions = true;
    });
```

Now make your regular request. 

```shell
curl --location --request GET 'https://localhost:44314/weatherforecast' -i
HTTP/1.1 200 OK
Transfer-Encoding: chunked
Content-Type: application/json; charset=utf-8; version=1.0
Server: Microsoft-IIS/10.0
api-supported-versions: 2.0
api-deprecated-versions: 1.0
X-Powered-By: ASP.NET
Date: Fri, 12 Mar 2021 18:34:06 GMT

[{"date":"2021-03-13T18:34:06.9403056+00:00","temperatureC":18,"temperatureF":64,"summary":"Cool"},{"date":"2021-03-14T18:34:06.9403268+00:00","temperatureC":36,"temperatureF":96,"summary":"Chilly"},{"date":"2021-03-15T18:34:06.940341+00:00","temperatureC":-6,"temperatureF":22,"summary":"Bracing"},{"date":"2021-03-16T18:34:06.940351+00:00","temperatureC":-6,"temperatureF":22,"summary":"Chilly"},{"date":"2021-03-17T18:34:06.9403667+00:00","temperatureC":14,"temperatureF":57,"summary":"Balmy"}]
```

I used an `-i` flag with the Curl request to print the response headers too. And you can now see two new Header values `api-supported-version` and `api-deprecated-version`. 

To tell ASP .NET Core that I have deprecated a version, i.e. no long going to release new changes to it, I can use 

```csharp
    [ApiVersion("1.0", Deprecated = true)] // tells .net core that this controller support api version 1.0 but is deprecated
    [ApiVersion("2.0")] // tells .net core that this controller also supports api version 2.0
```

### How do I get this version of the solution?

In order to get the version of the code with this setup, run this on your git bash:

```shell
git checkout -b versioning/customheader

git reset --hard 7093aad73c59ff5d34d37f321e25143aaf4ef3bc
```

You should now have this branch on the commit, where you can run your solution for query string versioning.
[Explore this repository at that commit on Github](https://github.com/lonelydev/CoolProductApi/tree/7093aad73c59ff5d34d37f321e25143aaf4ef3bc)

[Back](./AspNetCoreApiVersioning.md)