using Newtonsoft.Json;
using System.Diagnostics;

namespace Codecamp.Models.Api
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonObject(Title = "track")]
    public class ApiTrack
    {
        public ApiTrack(Track webTrack)
        {
            if (webTrack == null)
                return;

            Id = webTrack.TrackId;

            EventId = webTrack.EventId;

            Name = webTrack.Name;
            RoomNumber = webTrack.RoomNumber;
        }

        public int Id { get; set; }

        public int? EventId { get; set; }

        public string Name { get; set; }

        public string RoomNumber { get; set; }

        private string DebuggerDisplay =>
            $"{Id} - Event {EventId} - {Name} - {RoomNumber}";
    }
}
