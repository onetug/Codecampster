using Newtonsoft.Json;

namespace Codecamp.Models.Api
{
    [JsonObject(Title = "user")]
    public class ApiUser
    {
        // For Json.Net Serialization/Deserialization
        public ApiUser()
        {

        }

        public ApiUser(CodecampUser webUser, bool includeDetails = false)
        {
            Id = webUser.Id;

            FirstName = webUser.FirstName;
            LastName = webUser.LastName;

            // TODO Future
            //EventId = webSpeaker.EventId;

            if (!includeDetails)
                return;

            EmailAddress = webUser.Email;
        }

        #region Summary

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        // TODO Future
        //public int? EventId { get; set; }

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

        #endregion

        #region Details

        public string EmailAddress { get; set; }

        #endregion

        // Redundant unless attendees are directly accessible
        private int? SpeakerId { get; set; }

        private string DebuggerDisplay =>
            $"{Id}" +  DebugSpeakerId + $" - {FullNameOrEmailAddress}";

        private string DebugSpeakerId =>
            SpeakerId != null ? $" - Speaker {SpeakerId}" : "";
    }
}
