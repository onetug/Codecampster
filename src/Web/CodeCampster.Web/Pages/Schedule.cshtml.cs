﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeCampster.Web
{
    public class ScheduleModel : PageModel
    {
        public void OnGet()
        {

        }

        public string SessionizeUrl => Sessionize.ScheduleSmartGridUrl;
    }
}