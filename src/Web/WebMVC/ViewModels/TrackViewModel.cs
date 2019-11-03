using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.ViewModels
{
    public class TrackViewModel
    {
        public int TrackId { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }
        public List<SessionViewModel> Sessions { get; set; }
    }
}
