# URI path versioning

This is probably the versioning method that I am most familiar with as the projects I have worked on have used this. 

In order to do this you have to configure that in the `Startup.cs` again.
And you can remove the Reader configuration from earlier.

```csharp
services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true; 
                options.DefaultApiVersion = ApiVersion.Default;
                options.ReportApiVersions = true; // response headers contain version information
            });
```

Now in your controller, however, you want to tell the route to the controller. You could do this at an endpoint level or at the controller level based on your needs.

```csharp
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)] // tells .net core that this controller support api version 1.0
    [ApiVersion("2.0")] // tells .net core that this controller also supports api version 2.0
    public class WeatherForecastController : ControllerBase
    {
```

In the example, if you look at the `Route` attribute, I have used some special syntax to tell ASP .NET Core what the URL path should be to access different version. 

`{version:apiVersion}` is recognised as a number that represents the version of the api supported in the `ApiVersion` attributes of the controller. 

That means, the URL would be something like `v1/weatherforecast` or `v2/weatherforecast`

Wait, now you must be wondering what about minor versions? We did provide `1.0` and `2.0` in  the `ApiVersion` attribute. What about that?

Apparently, if there were minor versions, that would be respected too. 

Run the application and use the following curl command to test it out. 

```shell
curl --location --request GET 'https://localhost:44314/v2/weatherforecast' -i
HTTP/1.1 200 OK
Transfer-Encoding: chunked
Content-Type: application/json; charset=utf-8; version=2
Server: Microsoft-IIS/10.0
api-supported-versions: 2.0
api-deprecated-versions: 1.0
X-Powered-By: ASP.NET
Date: Fri, 12 Mar 2021 21:30:51 GMT

[{"date":"2021-03-13T21:30:51.4485395+00:00","celsius":15,"farenheit":58,"summary":"Freezing"},{"date":"2021-03-14T21:30:51.4485623+00:00","celsius":53,"farenheit":127,"summary":"Hot"},{"date":"2021-03-15T21:30:51.4485767+00:00","celsius":-18,"farenheit":0,"summary":"Cool"},{"date":"2021-03-16T21:30:51.4485867+00:00","celsius":15,"farenheit":58,"summary":"Mild"},{"date":"2021-03-17T21:30:51.4485997+00:00","celsius":31,"farenheit":87,"summary":"Warm"}]
```

Used the `-i` option with curl to display the response headers again. Everything about the headers still say the same as we haven't changed that configuration. 
We have only changed where we specify which version of the API we want to use. You can repeat the same using `v2.0` in the URI and you would still get the same result. 

### How do I get this version of the solution?

In order to get the version of the code with this setup, run this on your git bash:

```shell
git checkout -b versioning/customheader

git reset --hard 3d222ee65308509d90eb63ca3e38682996984a9e
```

You should now have this branch on the commit, where you can run your solution for query string versioning.
[Explore this repository at that commit on Github](https://github.com/lonelydev/CoolProductApi/tree/3d222ee65308509d90eb63ca3e38682996984a9e)

[Back](./AspNetCoreApiVersioning.md)