using System;
using GraphQlRethinkDbLibrary.Handlers;
using GraphQlRethinkDbLibrary.Schema.Types;
using GraphQL.Conventions;

namespace GraphQlRethinkDbTemplate.Model
{
    public class Image : NodeBase<Image>, IDefaultImage
    {
        public Image(string imageData, string source, string contentType)
        {
            ImageData = imageData;
            Source = source;
            ContentType = contentType;
        }

        public string ContentType { get; }
        public string ImageData { get; }
        byte[] IDefaultImage.ImageData => Convert.FromBase64String(ImageData);
        public string Source { get; }
    }

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
