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

The API would respond with the `WeatherForecastV1` response. 

If you wanted to hit the v2, you'd use the earlier method of passing in a query param - `https://localhost:44314/weatherforecast?api-version=2.0`

That should give you a response with the DTO that matches `WeatherForecastV2`.

## But it still uses the query params for versioning

We can solve that soon. 

## References:

* [Scott Hanselman on Api Versioning] (https://www.hanselman.com/blog/aspnet-core-restful-web-api-versioning-made-easy)
* [Nick Chapas on Api Versioning] (https://www.youtube.com/watch?v=iVHtKG0eU_s)
* [Troy Hunt on Api Versioning] (https://www.troyhunt.com/your-api-versioning-is-wrong-which-is/)