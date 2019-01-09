using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.Services
{
    public class AppOptions
    {
        public string EmailServer { get; set; }
        public int PortNumber { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string CaptchaKey { get; set; }
    }
}
