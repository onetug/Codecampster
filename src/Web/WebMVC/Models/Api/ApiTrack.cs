using Newtonsoft.Json;
using System.Diagnostics;

namespace Codecamp.Models.Api
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonObject(Title = "track")]
    public class ApiTrack
    {
        // For Json.Net Serialization/Deserialization
        public ApiTrack()
        {

        }

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

        #region Summary

        public int? EventId { get; set; }

        public string Name { get; set; }

        #endregion

        #region Details

        public string RoomNumber { get; set; }

        #endregion

        private string DebuggerDisplay =>
            $"{Id} - Event {EventId} - {Name} - {RoomNumber}";
    }
}
