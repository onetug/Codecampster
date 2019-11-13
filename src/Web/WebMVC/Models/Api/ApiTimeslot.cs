using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Diagnostics;

namespace Codecamp.Models.Api
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonObject(Title = "timeslot")]
    public class ApiTimeslot
    {
        // For Json.Net Serialization/Deserialization
        public ApiTimeslot()
        {

        }

        public ApiTimeslot(Timeslot webTimeslot)
        {
            if (webTimeslot == null)
                return;

            Id = webTimeslot.TimeslotId;

            EventId = webTimeslot.EventId;

            StartTime = webTimeslot.StartTime;
            EndTime = webTimeslot.EndTime;

            ContainsNoSessions = webTimeslot.ContainsNoSessions;
            Name = webTimeslot.Name;
        }

        public int Id { get; set; }

        #region Summary

        public int? EventId { get; set; }

        [JsonConverter(typeof(TimeFormatConverter), "h:mm tt")]
        public DateTime StartTime { get; set; }

        [JsonConverter(typeof(TimeFormatConverter), "h:mm tt")]
        public DateTime EndTime { get; set; }

        public bool ContainsNoSessions { get; set; }

        #endregion

        #region Details

        public string Name { get; set; }

        #endregion

        private string DebuggerDisplay =>
            $"{Id} - Event {EventId} - {StartTime:t} to {EndTime:t} - {Name}";

        private class TimeFormatConverter : IsoDateTimeConverter
        {
            public TimeFormatConverter(string format)
            {
                DateTimeFormat = format;
            }
        }
    }
}
