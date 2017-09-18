# GraphQlRethinkDbTemplate
========

## Getting started
Start with an ASP.NET Core web application. Upgrade the project to dotnet core 2.0.

### Startup.cs
```Startup.cs
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    BaseHandler.Setup(
        app,
        env,
        new DatabaseName(Program.DatabaseName),
        new DatabaseUrl("localhost"),
        new GraphQlDefaultHandler<Query,Mutation>(),
        new ImageFileHandler(),  // Optional, if you want to serve images from the api
        new AudioFileHandler()); // Optional, if you want to stream audio from the api
}
```

### Query and Mutation
```Query.cs
[ImplementViewer(OperationType.Query)]
public class Query
{
    [Description("Get data by id")]
    public Task<SomeDataClass> Data(UserContext context, Id id)
    {
        var data = context.Get<SomeDataClass>(id);
        return Task.FromResult(data);
    }
}
```

```Mutation.cs
[ImplementViewer(OperationType.Mutation)]
public class Mutation
{
    public DefaultResult<SomeDataClass> AddData(
    UserContext context,
    NonNull<string> text)
    {
        var data = new SomeDataClass(text);
        var ret = context.AddDefault(data);
        return new DefaultResult<SomeDataClass>(ret);
    }
}
```

### Data
``` SomeDataClass.cs
public class SomeDataClass : NodeBase<SomeDataClass>
{
    public SomeDataClass(string text)
    {
        Text = text;
    }

    public string Text { get; }
}
```
