using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codecamp.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Codecamp.Areas.Identity.Pages.Account
{
    public class RegistrationSuccessfulModel : PageModel
    {
        private readonly IEventBusinessLogic _eventBL;

        public RegistrationSuccessfulModel(
            IEventBusinessLogic eventBL)
        {
            _eventBL = eventBL;
        }

        public EventData Event { get; set; }

        public string ReturnUrl { get; set; }

        public class EventData
        {
            public string EventTitle { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            var theEvent = await _eventBL.GetActiveEvent();

            Event = new EventData
            {
                EventTitle = theEvent != null ? theEvent.Name : "the current event was not found."
            };

            return Page();
        }

        public IActionResult OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            return LocalRedirect(returnUrl);
        }
    }
}