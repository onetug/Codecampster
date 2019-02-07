using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.Models
{
    public class SpeakerSession
    {
        [ForeignKey("Speaker")]
        public int SpeakerId { get; set; }
        public virtual Speaker Speaker { get; set; }
        [ForeignKey("Session")]
        public int SessionId { get; set; }
        public virtual Session Session { get; set; }
    }
}
