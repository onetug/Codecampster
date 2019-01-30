using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Codecamp.Models.Api
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonObject("speaker")]
    public class ApiSpeaker : Speaker
    {
        public ApiSpeaker(Speaker webSpeaker) :
            this(webSpeaker, null, false)
        {

        }

        public ApiSpeaker(Speaker webSpeaker, Uri imageUrl,  bool includeDetails = false)
        {
            if (webSpeaker == null)
                return;

            SpeakerId = webSpeaker.SpeakerId;

            ImageUrl = imageUrl;
            ApiUser = (webSpeaker.CodecampUser == null)
                ? null
                : new ApiUser(webSpeaker.CodecampUser);

            IsMvp = webSpeaker.IsMvp;
            IsApproved = webSpeaker.IsApproved;

            // TODO Future
            //EventId = webSpeaker.EventId;

            if (!includeDetails)
                return;

            CompanyName = webSpeaker.CompanyName;
            Bio = webSpeaker.Bio;
            WebsiteUrl = webSpeaker.WebsiteUrl;
            BlogUrl = webSpeaker.BlogUrl;
            LinkedIn = webSpeaker.LinkedIn;
        }

        [JsonProperty("user")]
        public ApiUser ApiUser { get; }

        public Uri ImageUrl { get; }

        private string DebuggerDisplay =>
            $"{SpeakerId} - {ApiUser.FullNameOrEmailAddress}";
    }
}
