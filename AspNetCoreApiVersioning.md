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

### How do I get this version of the solution?

In order to get the version of the code with this setup, run this on your git bash, while on `main` branch:

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

### How do I get this version of the solution?

In order to get the version of the code with this setup, run this on your git bash, while on `main` branch:

```shell
git checkout -b versioning/acceptheader

git reset --hard 5f074a21
```

You should now have this branch on the commit, where you can run your solution for query string versioning.
[Explore this repository at that commit on Github](https://github.com/lonelydev/CoolProductApi/tree/5f074a21fc8f07526e445d35234eb2dd41f66be3)


## Version using Custom Header

Now that you have done versioning using Accept Header, versioning using a custom header is very similar. 

In your `Startup.cs`, you could modify the versioning configuration to 

```csharp
services.AddApiVersioning(options =>
   {
       options.AssumeDefaultVersionWhenUnspecified = true; 
       options.DefaultApiVersion = ApiVersion.Default;
       options.ApiVersionReader = new HeaderApiVersionReader("e-version");
   });
```

The third option value is the key here. You have just told ASP .NET Core to read Version information from Header and also from a specific header called `e-version`. 
I just chose that random header name. You could pick yours. I think by convention, I have seen many header versioned API's using custom Headers that start with `x-`. However, that is now just a deprecated recommendation according [this detailed answer on stackoverflow with links](https://stackoverflow.com/a/3561399/2262959).

Time to run your app to test it. 

Go hit the url using the following Curl command

```shell
curl --location --request GET 'https://localhost:44314/weatherforecast' \
--header 'Accept: application/json' \
--header 'e-version: 2.0'
```

That's how you do versioning using a custom header.

### How do I get this version of the solution?

In order to get the version of the code with this setup, run this on your git bash:

```shell
git checkout -b versioning/customheader

git reset --hard 10c164e99304d9db6db82ff32f504a627c20d3c3
```

You should now have this branch on the commit, where you can run your solution for query string versioning.
[Explore this repository at that commit on Github](https://github.com/lonelydev/CoolProductApi/tree/10c164e99304d9db6db82ff32f504a627c20d3c3)


## Supporting multiple ways of requesting versions

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

## Provide clients with version information

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

## What about URI path versioning

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

We will take a look at another way to configure this without specifying all those attributes.

## References:

* [Scott Hanselman on Api Versioning] (https://www.hanselman.com/blog/aspnet-core-restful-web-api-versioning-made-easy)
* [Nick Chapas on Api Versioning] (https://www.youtube.com/watch?v=iVHtKG0eU_s)
* [Troy Hunt on Api Versioning] (https://www.troyhunt.com/your-api-versioning-is-wrong-which-is/)
* [Microsoft Github - API Versioning Example - different framework versions](https://github.com/microsoft/aspnet-api-versioning)