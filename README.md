# Api Versioning

## What is it?

A lot has been said about versioning your product. Semantic version is familiar if you develop apps and consume libraries/packages from package repositories like npm or NuGet.
Sometimes, your product is an API. How do you go about versioning it? I have versioned APIs in the past without doi9ng a full literature review of the different routes that I could have gone for versioning my API. 

But just a hint: Unlike Semantic versioning, one does not Version API with a `x.y.z` format. 

Some APIs do go the `x.y` route, but not any further. Generally you only have to provide a new version for your API endpoint, if and only if the change you have made to the endpoint, results in a change in behaviour that would impact the client directly. 

E.g if your API is sending a new field, as part of the repsonse, that is all good. Whereas if your API is removing a field, you ought to let your clients know as they could be using that field in their app. 


So how do I do it in ASP NET. Core. 

## First things first

If you happened to have found this repository, you probably are on github too. Did you know Microsoft has a repository specifically about api-versioning?
If not, then here it is: [Microsoft's ASP .NET API Versioning](https://github.com/microsoft/aspnet-api-versioning)

## Version in the URL path

A common practice and one that I have used in the past is to version the API such that a newer version is behind a new URL path. 
You might have already seen this in plenty of places, as this is a really common way to separate versioned functionality and easier and obvious way to determine, which API is no longer used by just looking at the codebase folder structure. 
A lot of API Developers prefer this approach because of that very reason. Easy, clear and obvious separation of different versions. Letting you cleanup older versions as they reach their End of Support. 

Usually they go by the following template:
`https://www.awesomeproduct/api/v1/features`

This also means, the consumer of the API only needs to about the URL to start consuming it. So you could configure some API documentation per URL as it ends up thereby being docs for the different versions too!

A route prefix for `v{n}` where `{n}` is usually defaulted to `1`

## Version in URL query string

I haven't personally used an API that versions this way. However, when you turn on versioning in .NET Core by default, the scheme used is this. 

Thus from the earlier example, this url would look like:
`https://www.awesomeproduct/api/features?api-version=1.0`

Now this is still a perfectly valid way of versioning your API, however, not the most obvious for developers maintaining the API. However, it still a valid way if you want to version your APIs without actually changing the original URL.

## Version in a custom header

There are some APIs that actually prefer keeping the URL the same, following the principles stated by [Roy Fielding the creator of REST](https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm) who highlights that changing URLs and giving versions in the URL changes the meaning of the URI. After all in REST, all things you call using the URI is a resource. 

Some people even classify this type of versioning as Continuous Versioning. I didn't delve into the origins of that concept though, so I cannot say much about this apart from the fact that it doesn't involve changing the URL. 

Well, if you can't specify your API version in the URI, the next best place to provide it is by requesting using a value in the request HEADER. 

Here, I mentioned a custom header, hence you could probably use something like `x-api-version: 2.0` in the header and then it would be your API's responsibility to read that value from the header and then route the request to the corresponding endpoint behaviour.

## Version in Content-Type

If you have been making web requests for several years already, you probably already know by now that there are some HEADER elements commonly used by a client to interact with a server. 

`Accept` Header is such an example. 

Some APIs actually let you mention a version in here. 

`Accept: application/super.amazingproduct.v2+json`

Wow! did you know that already? I came across this while reading [Troy Hunt's blog post on URL versioning](https://www.troyhunt.com/your-api-versioning-is-wrong-which-is/).

## So many ways, so what do I do?

Think about it from your perspective, which one would you prefer as an API developer. 

Really, any option is alright. This is a highly opinionated discussion that has gone one  for ages. Use anyone and make your life easy. 

If you are working in a team, have a chat about this with the team, discuss pros and cons amongst yourselves and then decide. What is most important is that those in the team maintaining and developing the API is happy with the choice. 

## Which one do I prefer? 

I lean towards the URL path preference, purely because I have seen the benefits of being able to delete a large amount of code relatively easily by just deleting folders of controllers. In my case, the API was consumed by engineers within the company. Hence, setting a deadline for migrating and removing support for older versions was a relatively easy task. However, once you open up the API to the outside world, things will not be under your control and hence, it will be increasingly harder for you to get rid of older versions even with a lot of follow up. 


## What about versioning in ASP .NET Core?

Lets get to that then. 

Did you know Microsoft has a Nuget package just for this? It is called [Microsoft.AspNetCore.Mvc.Versioning](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Versioning/5.0.0?_src=template).

So let us take a deep dive in [AspNetCoreApiVersioning](./AspNetCoreApiVersioning.md)