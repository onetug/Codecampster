using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.Models
{
    public class Timeslot
    {
        public int TimeslotId { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        [DisplayFormat(DataFormatString = "{HH:mm}")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        [DisplayFormat(DataFormatString = "{HH:mm}")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Special timeslot with no sessions
        /// i.e., keynote, lunch, etc.
        /// </summary>
        [Display(Name = "Timeslot contains no sessions")]
        public bool ContainsNoSessions { get; set; }

        public string Name { get; set; }

        public virtual List<Session> Sessions { get; set; }
    }
}
