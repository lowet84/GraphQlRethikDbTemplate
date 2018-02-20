using System.IO;
using GraphQlRethinkDbHttp;
using GraphQlRethinkDbHttp.Handlers;
using SampleAppHttp.Model;

namespace SampleAppHttp.Handlers
{
    public class ImageFileHandler : DefaultImageHandler
    {
        public override IDefaultImage GetImage(string key)
        {
            var imageFile = new ImageFile(key);
            var path = System.IO.Path.Combine(".", "static", imageFile.FileName);
            var bytes = File.ReadAllBytes(path);
            //SimpleHttpServer.MimeTypeMappings.TryGetValue(System.IO.Path.GetExtension(path), out var mime);
            var ret = new ImageFile.ImageFileData("", bytes);
            return ret;
        }
    }
}