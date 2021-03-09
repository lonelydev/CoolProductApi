# ASP NET Core API versioning


## The API

First let us create a new dotnet core API project. By default the template creates a Weather Forecast API. 
You should be able to get a bunch of random weather reports in JSON format from `https://localhost:44314/weatherforecast`


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

The momnent you add that and try running your project, you realise that your previously working API endpoint is no longer working.!

In fact you get an error that goes something like this:

```json
{"error":{"code":"ApiVersionUnspecified","message":"An API version is required, but was not specified.","innerError":null}}
```

But you only just called `AddApiVersioning()`. 


That is exactly the problem. 

You said you wanted to add versioning to your API, but didn't tell .NET Core about the method of versioning you want it to do, so it chose the default. 

## What is the default then?

I never would have guessed until I read about it. 

It uses a query param versioning scheme by default. So in order to access your weather forecast api now, yuou'll have to use the following URL. 

`https://localhost:44314/weatherforecast?api-version=1.0`

Your localhost url might be different because it is porobably using another port. But the idea is the query string now says which versoin of the API is required to serve the request. 

But what you have done now is basically broken your API for everyone who was already using the old endpoint!


## What are my other options?

Modify your `Startup.cs` to update the versioning configuration.

```csharp
services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true; 
        options.DefaultApiVersion = ApiVersion.Default;
    });
```

Go to your Controller - WeatherForecastController. 

Create a copy your existing endpoint - the Get endpoint. 

Create a copy of the response DTO. 

So in my case. I did:

```csharp
public class WeatherForecastV2
{
    public DateTime Date { get; set; }
    public int Celsius { get; set; }
    public int Farenheit => 32 + (int)(Celsius / 0.5556);
    public string Summary { get; set; }
}
```

This is just a copy of the other one, but with some properties renamed. 

I also renamed the first DTO to WeatherForecastV1 and retained all the properties as they were earlier. 

My new endpoint looks like:

```csharp
[HttpGet]
[MapToApiVersion("2.0")]
public IEnumerable<WeatherForecastV2> GetV2()
{
    var rng = new Random();
    return Enumerable.Range(1, 5).Select(index => new WeatherForecastV2
    {
        Date = DateTime.Now.AddDays(index),
        Celsius= rng.Next(-20, 55),
        Summary = Summaries[rng.Next(Summaries.Length)]
    })
    .ToArray();
}
```

Notice the `[MapToApiVersion("2.0")]` attribute. 

I also decorated the controller with some attributes:

```csharp

    [ApiController]
    [Route("[controller]")]
    [ApiVersion("1.0")] // tells .net core that this controller support api version 1.0
    [ApiVersion("2.0")] // tells .net core that this controller also supports api version 2.0
    public class WeatherForecastController: ControllerBase
```

This means, you can use the same controller for requests for both version 1.0 and 2.0. 

This may not always be your usecase. You might have scenarios like the ones I have faced in the past where we had to create a completely new version of the entire API because every interface was changing. 

However, in this example, it is just a single endpoint that has had a new version. 

Now try running your app and hit the url: `https://localhost:44314/weatherforecast`

or if you use Curl, then use the following code:

```shell
curl --location --request GET 'https://localhost:44314/weatherforecast?api-version=2.0'
```

The API would respond with the `WeatherForecastV1` response. 

If you wanted to hit the v2, you'd use the earlier method of passing in a query param - `https://localhost:44314/weatherforecast?api-version=2.0`

That should give you a response with the DTO that matches `WeatherForecastV2`.

In this example, we have only decorated the version 2.0 api to an endpoint. The other endpoint is not marked as version 1.0. But as you decorated the controller with both versions, anything that isn't already marked as version 2.0 will be considered version 1.0.

But it still uses the query params for versioning.

### How do get this solution?

In order to get the version of the code with this setup, run this on your git bash:

```shell
git checkout -b versioning/querystring

git reset --hard 2778af32ae705341765bec57b879d5bd6e1f9b73
```

You should now have this branch on the commit, where you can run your solution for query string versioning.
[Explore this repository at that commit on Github](https://github.com/lonelydev/CoolProductApi/tree/2778af32ae705341765bec57b879d5bd6e1f9b73)


## Versioning using Accept Header

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


## References:

* [Scott Hanselman on Api Versioning] (https://www.hanselman.com/blog/aspnet-core-restful-web-api-versioning-made-easy)
* [Nick Chapas on Api Versioning] (https://www.youtube.com/watch?v=iVHtKG0eU_s)
* [Troy Hunt on Api Versioning] (https://www.troyhunt.com/your-api-versioning-is-wrong-which-is/)