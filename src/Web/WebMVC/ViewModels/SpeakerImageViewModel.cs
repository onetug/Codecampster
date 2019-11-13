using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.ViewModels
{
    public class SpeakerImageViewModel
    {
        public bool ImageAvailable { get; set; }
        public string DefaultImagePath { get; set; }
        public string ImageString { get; set; }
    }
}
