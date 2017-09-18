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
```Startup.cs

### Query and Mutation
