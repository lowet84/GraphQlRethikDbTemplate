using System;
using System.IO;
using GraphQlRethinkDbLibrary.Handlers;
using GraphQL.Conventions;
using SampleApp.Model;

namespace SampleApp.Handlers
{
    public class ImageFileHandler : DefaultImageHandler
    {
        public override IDefaultImage GetImage(string key)
        {
            var id = new Id(key);
            if (id.IsIdentifierForType<Image>())
            {
                return HandlerUtil.Get<Image>(id);
            }
            if (id.IsIdentifierForType<ImageFile>())
            {
                return GetImageFromFile(id);
            }
            throw new NotImplementedException();
        }

        private IDefaultImage GetImageFromFile(Id id)
        {
            var imageFile = HandlerUtil.Get<ImageFile>(id);
            var bytes = File.ReadAllBytes(imageFile.FileName);
            var ret = new ImageFile.ImageFileData("image/jpeg", bytes);
            return ret;
        }
    }
}