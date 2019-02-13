using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.Models
{
    public class Schedule
    {
        [ForeignKey("Session")]
        public int SessionId { get; set; }

        public virtual Session Session { get; set; }

        [ForeignKey("Track")]
        public int TrackId { get; set; }

        public virtual Track Track { get; set; }

        [ForeignKey("Timeslot")]
        public int TimeslotId { get; set; }

        public virtual Timeslot Timeslot { get; set; }
    }
}
