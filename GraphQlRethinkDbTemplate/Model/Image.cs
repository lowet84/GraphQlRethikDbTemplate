using GraphQlRethinkDbLibrary.Handlers;
using GraphQlRethinkDbLibrary.Schema.Types;

namespace GraphQlRethinkDbTemplate.Model
{
    public class Image : NodeBase<Image>, IDeafultImage
    {
        public Image(string imageData, string source, string contentType)
        {
            ImageData = imageData;
            Source = source;
            ContentType = contentType;
        }

        public string ContentType { get; }
        public string ImageData { get; }
        public string Source { get; }
    }
}
