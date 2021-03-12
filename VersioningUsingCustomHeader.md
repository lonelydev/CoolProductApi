# Version using Custom Header

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

[Back](./AspNetCoreApiVersioning.md)