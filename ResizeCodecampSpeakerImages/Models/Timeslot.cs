using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Codecamp.Models
{
    public class Timeslot
    {
        public int TimeslotId { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Special timeslot with no sessions
        /// i.e., keynote, lunch, etc.
        /// </summary>
        [Display(Name = "Timeslot contains no sessions")]
        public bool ContainsNoSessions { get; set; }

        public string Name { get; set; }

        [ForeignKey("Event")]
        public int? EventId { get; set; }

        public Event Event { get; set; }

        public virtual List<Session> Sessions { get; set; }
    }
}
