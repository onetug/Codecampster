using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.Models
{
    public enum LevelsOfSkill
    {
        AllSkillLevels = 1,
        SomePriorKnowledgeRequired = 2,
        DeepDive = 3
    }

    public class Session
    {
        public int SessionId { get; set; }

        public string Name { get; set; }

        public string Decripion { get; set; }

        [Display(Name = "Skill Level")]
        public int SkillLevel { get; set; }

        // Keywords associated with the session.
        public string Keywords { get; set; }

        public bool IsApproved { get; set; }

        // Session speakers
        public List<Speaker> Speakers { get; set; }

        [ForeignKey("Event")]
        public int? EventId { get; set; }

        public Event Event { get; set; }
    }
}
