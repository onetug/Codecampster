using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Codecamp.Data;
using Codecamp.Models;
using Codecamp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace Codecamp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterAttendeeModel : PageModel
    {
        private readonly SignInManager<CodecampUser> _signInManager;
        private readonly UserManager<CodecampUser> _userManager;
        private readonly ILogger<RegisterAttendeeModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly CodecampDbContext _context;

        public RegisterAttendeeModel(
            UserManager<CodecampUser> userManager,
            SignInManager<CodecampUser> signInManager,
            ILogger<RegisterAttendeeModel> logger,
            IEmailSender emailSender,
            CodecampDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
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
        }

        public void OnGet(string returnUrl = null) { }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            var manageUserPage = Url.Content("./Manage/Index");

            if (ModelState.IsValid)
            {
                var user
                    = new CodecampUser
                    {
                        IsAttending = true,
                        IsSpeaker = false,
                        UserName = Input.Email,
                        Email = Input.Email
                    };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _userManager.AddToRoleAsync(user, "Attendee");

                    // Save the DB changes
                    await _context.SaveChangesAsync();

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    // await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to the manage user page so the speaker can add
                    // additional information
                    return RedirectToPage(manageUserPage, new { LoginWithRegistration = "Attendee" });
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
                                ReturnUrl = manageUserPage,
                                LoginWithRegistration = "Attendee",
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
