using System;
using System.Diagnostics.CodeAnalysis;
using Codecamp.Data;

namespace Codecamp.BusinessLogic.Api
{
    public class ApiBusinessLogic
    {
        protected ApiBusinessLogic(CodecampDbContext context, string imageFolder =  "")
        {
            Context = context;
            ImageFolder = imageFolder;
        }

        protected CodecampDbContext Context { get; }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        protected string ImageFolder { get; }

        protected Uri GetImageUrl(int id)
        {
            var imageUrl =
                new Uri($"/api/{ImageFolder}/{id}/image", UriKind.Relative);

            return imageUrl;
        }
    }
}
