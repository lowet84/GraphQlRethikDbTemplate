using GraphQlRethinkDbLibrary.Handlers;
using GraphQlRethinkDbLibrary.Schema.Types;

namespace GraphQlRethinkDbTemplate.Model
{
    public class ImageFile : NodeBase<ImageFile>
    {
        public string FileName { get; }

        public ImageFile(string fileName)
        {
            FileName = fileName;
        }

        public class ImageFileData : IDefaultImage
        {
            public ImageFileData(string contentType, byte[] imageData)
            {
                ContentType = contentType;
                ImageData = imageData;
            }

            public string ContentType { get; }
            public byte[] ImageData { get; }
        }
    }
}
