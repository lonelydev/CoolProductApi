# Versioning without Attributes in controller

Routing via attributes came later to ASP .NET but some people like viewing all of it at one place, others like to view on their respective controllers. 
I am honping your API is not a GOD API that has too many controller than you can really maintain. So you could really choose either way to configure the versioning by URI path.

In this case, obviously as you don't need the Route attribute on the controller, nor the version attributes. You are going to set it all up in the `Startup.cs`. 

```csharp
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
    ...
    ... all that amazing code here
    [HttpGet]
    public IEnumerable<WeatherForecastV2> GetV2()
    {
```

So the route has been reverted to what it was originally and you can also see that there are no more versioning attributes.
Thus in this case, by looking at the controller it is hard to say what versions it really supports. 

Now head to `Startup.cs`

```csharp
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
...
...
...
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true; 
                options.DefaultApiVersion = ApiVersion.Default;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("e-version"),
                    new MediaTypeApiVersionReader("version")
                    );

                options.Conventions.Controller<WeatherForecastController>()
                .HasDeprecatedApiVersion(1, 0)
                .HasApiVersion(2,0)
                .Action(typeof(WeatherForecastController).GetMethod(nameof(WeatherForecastController.GetV2)))
                .MapToApiVersion(2,0);

                options.ReportApiVersions = true; // response headers contain version information
            });
```

That is a lot of configuration right there. 

Some people separte that controller configuration into a separate extension method and might even put that in a separate extension method file, in order to modularise the whole controller setup based on domain area or whatever. 
So in the end they could just call `services.AddWeatherForecastApi()`. 

It does look neat. But I still prefer information on the controller directly so that it is visible. But this is purely subjective, probably because that is how I have gotten used to recognizing which controller support which version without having to switch from the controller file. But it is completely upto you and your team's preference.

Remember that we removed the URI path versioning and are back to the Header and Mediatype versioning thus in order to test this you'll have to run the following curl commands:

```shell
❯ curl -i --location --request GET 'https://localhost:44314/weatherforecast' --header 'e-version: 2.0'
HTTP/1.1 200 OK
Transfer-Encoding: chunked
Content-Type: application/json; charset=utf-8; version=2.0
Server: Microsoft-IIS/10.0
api-supported-versions: 2.0
api-deprecated-versions: 1.0
X-Powered-By: ASP.NET
Date: Fri, 12 Mar 2021 22:10:09 GMT

[{"date":"2021-03-13T22:10:09.4576212+00:00","celsius":39,"farenheit":102,"summary":"Scorching"},{"date":"2021-03-14T22:10:09.4576289+00:00","celsius":3,"farenheit":37,"summary":"Mild"},{"date":"2021-03-15T22:10:09.4576321+00:00","celsius":23,"farenheit":73,"summary":"Hot"},{"date":"2021-03-16T22:10:09.4576351+00:00","celsius":19,"farenheit":66,"summary":"Warm"},{"date":"2021-03-17T22:10:09.457638+00:00","celsius":38,"farenheit":100,"summary":"Cool"}]
```

[Back](./AspNetCoreApiVersioning.md)