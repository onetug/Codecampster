using Newtonsoft.Json;

namespace Codecamp.Models.Api
{
    [JsonObject(Title = "user")]
    public class ApiUser
    {
        public ApiUser(CodecampUser webUser)
        {
            Id = webUser.Id;

            FirstName = webUser.FirstName;
            LastName = webUser.LastName;
            EmailAddress = webUser.Email;

            SpeakerId = webUser.SpeakerId;

            // TODO Future
            //EventId = webSpeaker.EventId;
        }

        public string Id { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string EmailAddress { get; }

        public int? SpeakerId { get; }

        // TODO Consolidate with Codecamp.Models.CodeCampUser.FullName
        public string FullNameOrEmailAddress
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
                    return EmailAddress;

                return FirstName + (FirstName.Length > 0 ? " " : "") + LastName;
            }
        }
    }
}
