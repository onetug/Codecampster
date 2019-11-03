using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.ViewModels
{
    public class TimeslotViewModel
    {
        public int TimeslotId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool ContainsNoSessions { get; set; }
        public List<SessionViewModel> Sessions { get; set; }
    }
}
