Remora.Rest
===========
Remora.Rest is a compact helper library for building REST API wrappers, giving
you the tools you need to rapidly create robust, highly configurable wrappers of
your own.

The library is somewhat opinionated, and it does target a single subset of REST
API types - those whose payloads (error or otherwise) are JSON-serialized. For 
these use cases, however, the library is quite adept at what it does.

## Quickstart
At its core, the library expects you to do two things - one, to access the REST
endpoints through a configured `RestHttpClient<TError>`, and two, to define the
entities in the API model through an interface/implementation combination.

```c#
var services = new ServiceCollection();

var clientBuilder = services
    .AddRestHttpClient<RestHttpClient<ErrorPayloadType>>();

// Perform HttpClient configuration (adding Polly policies, setting default
// request headers, etc)
clientBuilder...

// Configure API model types
services.Configure<JsonSerializerOptions>(clientBuilder.Name, options => 
{
    options.AddDataObjectConverter<IModelType, ModelType>();
});
```

After you've set up your client and your types, you can then inject an instance
of `RestHttpClient<TError>` into your actual API wrapper type (the instance is
transient, so you can treat it the same as any HttpClient you'd normally 
inject).

```c#
public class MyWrapper
{
    private readonly RestHttpClient<ErrorPayloadType> _restClient;
    
    public MyWrapper(RestHttpClient<ErrorPayloadType> restClient)
    {
        _restClient = restClient;
    }
}
```

More detailed documentation will hopefully be available in the future; in the 
meantime, check out [Remora.Discord][1], for which this project was originally
developed - it pushes all available functionality to its limits, and serves as a
relatively comprehensive demonstration of what you can do with the library.


[1]: https://github.com/Remora/Remora.Discord
