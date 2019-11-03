using Codecamp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.ViewModels
{
    public class ScheduleViewModel
    {
        public int SessionId { get; set; }

        [Display(Name = "Title")]
        public string Name { get; set; }

        public string Description { get; set; }

        public int SkillLevelId { get; set; }

        [Display(Name = "Skill Level")]
        public string SkillLevel { get; set; }

        public string Keywords { get; set; }

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        [Display(Name = "Speakers")]
        public string Speakers { get; set; }

        [Display(Name ="Track")]
        public int? TrackId { get; set; }

        [Display(Name = "Track")]
        public string TrackName { get; set; }

        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }

        [Display(Name = "Timeslot")]
        public int? TimeslotId { get; set; }

        [Display(Name = "Start Time")]
        public DateTime? StartTime { get; set; }

        [Display(Name = "End Time")]
        public DateTime? EndTime { get; set; }
    }
}
