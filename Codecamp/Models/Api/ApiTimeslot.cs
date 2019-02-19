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

        public int? EventId { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "h:mm tt")]
        public DateTime StartTime { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "h:mm tt")]
        public DateTime EndTime { get; set; }

        public bool ContainsNoSessions { get; set; }

        public string Name { get; set; }

        private string DebuggerDisplay =>
            $"{Id} - Event {EventId} - {StartTime:t} to {EndTime:t} - {Name}";

        // TODO public?
        private class DateFormatConverter : IsoDateTimeConverter
        {
            public DateFormatConverter(string format)
            {
                DateTimeFormat = format;
            }
        }
    }
}
