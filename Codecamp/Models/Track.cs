using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.Models
{
    public class Track
    {
        public int TrackId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }

        public virtual List<Session> Sessions { get; set; }
    }
}
