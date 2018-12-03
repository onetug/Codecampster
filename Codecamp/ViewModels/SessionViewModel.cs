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

        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Skill Level")]
        public string SkillLevel { get; set; }

        // Keywords associated with the session.
        public string Keywords { get; set; }

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        // Session speakers
        public string Speakers { get; set; }

        public virtual List<SpeakerSession> SpeakerSessions { get; set; }

        [Display(Name = "Event Name")]
        public string EventName { get; set; }
    }
}
