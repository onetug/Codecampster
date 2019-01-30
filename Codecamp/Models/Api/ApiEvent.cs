using Newtonsoft.Json;

namespace Codecamp.Models.Api
{
    [JsonObject(Title = "event")]
    public class ApiEvent : Event
    {
        public ApiEvent(Event webEvent)
        {
            EventId = webEvent.EventId;

            // Summary
            Name = webEvent.Name;
            StartDateTime = webEvent.StartDateTime;
            EndDateTime = webEvent.EndDateTime;

            // State
            IsActive = webEvent.IsActive;
            IsSpeakerRegistrationOpen = webEvent.IsSpeakerRegistrationOpen;
            IsAttendeeRegistrationOpen = webEvent.IsAttendeeRegistrationOpen;

            // Details
            LocationAddress = webEvent.LocationAddress;
            SocialMediaHashtag = webEvent.SocialMediaHashtag;
        }
    }
}