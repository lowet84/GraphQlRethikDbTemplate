using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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

        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /// <param name="port">Port of the server.</param>
        public SimpleHttpServer(int port, string host, params SpecialHandler[] handlers)
        {
            Handlers = handlers;
            Port = port;
            Host = host;
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
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private void Process(HttpListenerContext context)
        {
            var specialHandler = Handlers
                .FirstOrDefault(d => context.Request.RawUrl.StartsWith(d.Path));

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
            var filename = Path.Combine(".", "static", path);
            if (File.Exists(filename))
            {
                try
                {
                    Stream input = new FileStream(filename, FileMode.Open);

                    //Adding permanent http response headers
                    context.Response.ContentType = MimeUtil.MimeTypeMappings.TryGetValue(Path.GetExtension(filename), out var mime) ? mime : "application/octet-stream";
                    context.Response.ContentLength64 = input.Length;
                    context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                    context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filename).ToString("r"));

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
