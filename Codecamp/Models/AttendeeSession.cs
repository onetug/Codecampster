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
        [ForeignKey("CodecampUser")]
        public string CodecampUserId { get; set; }

        public virtual CodecampUser CodecampUser { get; set; }

        [ForeignKey("Session")]
        public int SessionId { get; set; }

        public virtual Session Session { get; set; }
    }
}
