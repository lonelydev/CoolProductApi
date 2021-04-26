# How to add Swagger to your Api

A couple of years ago, I created a sample repository to demonstrate Swagger - [WebApiWithSagger](https://github.com/lonelydev/WebApiWithSwagger).
But it is pretty old now, so I thought I might add Swagger to this one and add some markdown docs to accompany it. 

For an Introduction to what Swagger is, I'd suggest reading my write up on the [WebApiWithSagger](https://github.com/lonelydev/WebApiWithSwagger)

## Get Swagger in your project

You can find step by step instructions to do this on [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

## Got Swagger, now what?

There are three main components to Swashbuckle:

- [Swashbuckle.AspNetCore.Swagger](https://www.nuget.org/packages/Swashbuckle.AspNetCore.Swagger/): a Swagger object model and middleware to expose SwaggerDocument objects as JSON endpoints.
- [Swashbuckle.AspNetCore.SwaggerGen](https://www.nuget.org/packages/Swashbuckle.AspNetCore.SwaggerGen/): a Swagger generator that builds SwaggerDocument objects directly from your routes, controllers, and models. It's typically combined with the Swagger endpoint middleware to automatically expose Swagger JSON.
- [Swashbuckle.AspNetCore.SwaggerUI](https://www.nuget.org/packages/Swashbuckle.AspNetCore.SwaggerUI/): an embedded version of the Swagger UI tool. It interprets Swagger JSON to build a rich, customizable experience for describing the web API functionality. It includes built-in test harnesses for the public methods.

## Versions?

When starting development of an API you obviously will have jsut one verison. However, as you maintain it, you'll find yourself in situations where you have to release newer versions due to changing needs of the clients of your API. 
We have already taken a look at how to do versioning of WebApi in ASP .NET Core. 

Let us take a look at the changes involved

### Startup

We had configured Api Versioning. Now is the time to `AddVersionedApiExplorer` with appropriate options in the `ConfigureServices` method. 

Then you ensure you configure the Options required by SwaggerGen adding a transient dependency to `ConfigureSwaggerOptions` class.

Then `AddSwaggerGen()` with the `OperationFilter<SwaggerDefaultValues>` to ensure that default values are set and include XmlComments which is generated during the build in the application directory..


In `Configure` method which gets called by runtime, add an additional parameter to get the `IApiVersionDescriptionProvider`. This is essential to configure Swagger UI.

Use the extension methods from Swagger to do the configuration:

- `UseSwagger()`
- `UseSwaggerUI()`

Looking at the parameters passed to the `UseSwaggerUI`, you can see how the Api version descriptions are accessed an iterated through using the provider that we passed in to the method. 

### Swagger Directory and Files

The files in here are the ones that I used from [Microsoft's official Versioning Repository](https://github.com/microsoft/aspnet-api-versioning) that has example code for configuring API versioning with Swagger. 
And this has been extremely useful to get going. 

In fact the solution is just an extension using what has been documented here [ASP Net Core Versioning - Swagger] (https://github.com/microsoft/aspnet-api-versioning/wiki/API-Documentation#aspnet-core)

### Controllers

Do not forget to use the `[ApiVersion("X")]` and `[MapToApiVersion("X")]` attributes to let ASP NET Core route your requests to the right endpoint for the right version. 

## Done

That's all I have for now. I hope you found it useful.