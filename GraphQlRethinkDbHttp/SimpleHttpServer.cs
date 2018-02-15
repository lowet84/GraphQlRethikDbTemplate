using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GraphQlRethinkDbCore;
using GraphQlRethinkDbCore.Database;
using GraphQlRethinkDbHttp.Handlers;

namespace GraphQlRethinkDbHttp
{
    public class SimpleHttpServer
    {
        public SpecialHandler[] Handlers { get; }
        private readonly Thread _serverThread;
        private HttpListener _listener;

        public int Port { get; }
        public string Host { get; }

        public SimpleHttpServer(int port, string host, DatabaseName databaseName, DatabaseUrl databaseUrl, params SpecialHandler[] handlers)
        {
            new UserContext(null, databaseUrl, databaseName);
            Handlers = handlers;
            Port = port;
            Host = host;
            Console.WriteLine($"Running server on port: {port}");
            foreach (var handler in handlers)
            {
                Console.WriteLine($"Using handler: {handler.GetType().Name} on /{handler.Path}");
            }
            _serverThread = new Thread(Listen);
            _serverThread.Start();
        }

        public void Stop()
        {
            _serverThread.Abort();
            _listener.Stop();
        }

        private void Listen()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://{Host}:{Port}/");
            _listener.Start();
            while (true)
            {
                try
                {
                    var context = _listener.GetContext();
                    Process(context);
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
        }

        private void Process(HttpListenerContext context)
        {
            var specialHandler = Handlers
                .FirstOrDefault(d => context.Request.RawUrl.Split('/').FirstOrDefault(e => !string.IsNullOrEmpty(e)) == d.Path);

            if (specialHandler != null)
                specialHandler.Process(context);
            else
            {
                ProcessStaticFiles(context);
            }
        }

        public static void ProcessStaticFiles(HttpListenerContext context)
        {
            const string indexDefault = "/index.html";
            var path = context.Request.RawUrl;
            if (path == "/") path = indexDefault;
            var pathParts = new[] { ".", "static" }.Concat(path.Split('/')).Where(d => !string.IsNullOrEmpty(d)).ToArray();
            var filename = Path.Combine(pathParts);
            if (File.Exists(filename))
            {
                try
                {
                    Stream input = new FileStream(filename, FileMode.Open);

                    //Adding permanent http response headers
                    context.Response.ContentType = MimeUtil.MimeTypeMappings.TryGetValue(Path.GetExtension(filename), out var mime) ? mime : "application/octet-stream";
                    context.Response.ContentLength64 = input.Length;
                    context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                    context.Response.AddHeader("Last-Modified", File.GetLastWriteTime(filename).ToString("r"));

                    var buffer = new byte[1024 * 16];
                    int nbytes;
                    while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                        context.Response.OutputStream.Write(buffer, 0, nbytes);
                    input.Close();

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.OutputStream.Flush();
                }
                catch (Exception)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            context.Response.OutputStream.Close();
        }
    }
}
