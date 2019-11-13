using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Codecamp.BusinessLogic;
using Codecamp.Data;
using Codecamp.Models;
using Codecamp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codecamp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterSpeakerModel : PageModel
    {
        private readonly SignInManager<CodecampUser> _signInManager;
        private readonly UserManager<CodecampUser> _userManager;
        private readonly ILogger<RegisterSpeakerModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly CodecampDbContext _context;
        private readonly IEventBusinessLogic _eventBL;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IOptions<AppOptions> _options;
        private ISession _session => _httpAccessor.HttpContext.Session;

        public RegisterSpeakerModel(
            UserManager<CodecampUser> userManager,
            SignInManager<CodecampUser> signInManager,
            ILogger<RegisterSpeakerModel> logger,
            IEmailSender emailSender,
            CodecampDbContext context,
            IEventBusinessLogic eventBL,
            IHttpContextAccessor httpAccessor,
            IOptions<AppOptions> options)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _eventBL = eventBL;
            _httpAccessor = httpAccessor;
            _options = options;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string CaptchaKey { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            await   Task.Run(() =>
            {
                Input = new InputModel
                {
                    CaptchaKey = _options.Value.CaptchaKey
                };
            });
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Get the current event
                var theEvent = await _eventBL.GetActiveEvent();

                // This is a speaker registration
                var user
                    = new CodecampUser
                    {
                        IsAttending = true,
                        IsSpeaker = true,
                        UserName = Input.Email,
                        Email = Input.Email,
                        EventId = theEvent != null ? theEvent.EventId : (int?)null
                    };

                // Create a user
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Create a speaker and tie it to the CodecampUser
                    user.Speaker = new Speaker()
                    {
                        CodecampUserId = user.Id,
                        EventId = user.EventId,
                    };

                    // Add the user to the Speaker role
                    await _userManager.AddToRoleAsync(user, "Speaker");

                    // Save the DB changes
                    await _context.SaveChangesAsync();

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    // Generate and send a confirmation email to the user
                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    // Redirect to the registration almost complete page
                    return RedirectToPage("./RegistrationAlmostComplete");
                }

                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName")
                    {
                        // The account currently exists, redirect to login page, note
                        // this is to complete speaker registration.
                        return RedirectToPage("./Login",
                            new
                            {
                                ReturnUrl = "./Manage/Index",
                                LoginWithRegistration = "Speaker",
                                Email = Input.Email // pass the email/username for convenience
                            });
                    }

                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
