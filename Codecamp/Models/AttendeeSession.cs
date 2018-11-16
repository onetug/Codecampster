using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.Models
{
    /// <summary>
    /// Attendee session, this class represents a session favorited by an attendee
    /// </summary>
    public class AttendeeSession
    {
        public int AttendeeSessionId { get; set; }

        [ForeignKey("CodecampUser")]
        public string CodecampUserId { get; set; }

        public CodecampUser CodecampUser { get; set; }

        [ForeignKey("Session")]
        public int SessionId { get; set; }

        public Session Session { get; set; }
    }
}
