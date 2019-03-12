using Codecamp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.ViewModels
{
    public class SessionViewModel
    {
        public int SessionId { get; set; }

        [Display(Name = "Title")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Skill Level")]
        public string SkillLevel { get; set; }

        // Keywords associated with the session.
        public string Keywords { get; set; }

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        // Session speakers
        [Display(Name = "Speakers")]
        public string SpeakerNames { get; set; }

        [Display(Name = "Event Name")]
        public string EventName { get; set; }

        [Display(Name = "Favorite")]
        public bool IsUserFavorite { get; set; }

        public int? TrackId { get; set; }

        [Display(Name = "Track Name")]
        public string TrackName { get; set; }

        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }

        public int? TimeslotId { get; set; }

        [Display(Name = "Timeslot Name")]
        public string TimeslotName { get; set; }

        [Display(Name = "Start Time")]
        public DateTime? StartTime { get; set; }

        [Display(Name = "End Time")]
        public DateTime? EndTime { get; set; }
    }
}
