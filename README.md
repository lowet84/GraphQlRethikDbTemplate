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
    [Description("Get author by id")]
    public Task<Author> Author(UserContext context, Id id)
    {
        var data = context.Get<Author>(id);
        return Task.FromResult(data);
    }
}
```

```Mutation.cs
[ImplementViewer(OperationType.Mutation)]
public class Mutation
{
    public DefaultResult<Author> AddAuthor(
    UserContext context,
    NonNull<string> firstName,
    NonNull<string> lastName)
    {
        var author = new Author(firstName, lastName);
        var ret = context.AddDefault(author);
        return new DefaultResult<Author>(ret);
    }
}
```
