using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Codecamp.BusinessLogic;
using Codecamp.Data;
using Codecamp.Models;
using Codecamp.Services;
using Codecamp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Codecamp.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<CodecampUser> _userManager;
        private readonly SignInManager<CodecampUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly CodecampDbContext _context;
        private readonly IEventBusinessLogic _eventBL;
        private readonly ISpeakerBusinessLogic _speakerBL;

        public IndexModel(
            UserManager<CodecampUser> userManager,
            SignInManager<CodecampUser> signInManager,
            IEmailSender emailSender,
            CodecampDbContext context,
            IEventBusinessLogic eventBL,
            ISpeakerBusinessLogic speakerBL)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _context = context;
            _eventBL = eventBL;
            _speakerBL = speakerBL;
        }

        public string Username { get; set; }

        [Display(Name = "Is email confirmed")]
        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool IsSpeaker { get; set; }

        public string LoginWithRegistration { get; set; }

        public class InputModel
        {
            // Lets set the max file size to 20 MB, that is way big enough
            public const int MaxImageSize = 20000000;

            public int SpeakerId { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Company")]
            public string CompanyName { get; set; }

            [ImageSizeValidation(MaxImageSize)]
            [Display(Name = "Image")]
            public IFormFile ImageFile { get; set; }

            [DataType(DataType.MultilineText)]
            [Display(Name = "Bio")]
            public string Bio { get; set; }

            [DataType(DataType.Url)]
            [Display(Name = "Website URL")]
            public string WebsiteUrl { get; set; }

            [DataType(DataType.Url)]
            [Display(Name = "Blog URL")]
            public string BlogUrl { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Location")]
            public string GeographicLocation { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Twitter Handle")]
            public string TwitterHandle { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "LinkedIn")]
            public string LinkedIn { get; set; }

            [Required]
            [Display(Name = "I would like to volunteer")]
            public bool IsVolunteer { get; set; }

            [Display(Name = "I am an MVP")]
            public bool IsMvp { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Note to Organizers")]
            public string NoteToOrganizers { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string loginWithRegistration = null)
        {
            ViewData["MaxImageSize"] = SpeakerViewModel.MaxImageSize / 1000;

            LoginWithRegistration = loginWithRegistration;

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Finish registering the user as a speaker
            if (loginWithRegistration == "Speaker")
            {
                // Set role as Speaker
                user.IsAttending = true;
                user.IsSpeaker = true;
                await _userManager.AddToRoleAsync(user, "Speaker");

                // Remove Attendee role
                await _userManager.RemoveFromRoleAsync(user, "Attendee");

                // Set the users event to the current event
                var currentEvent
                    = await _eventBL.GetActiveEvent();

                if (user.EventId.HasValue == false
                    || (user.EventId.HasValue == true
                    && user.EventId.Value != currentEvent.EventId))
                {
                    user.EventId = currentEvent.EventId;
                }

                // Save the changes
                await _context.SaveChangesAsync();
            }
            else if (LoginWithRegistration == "Attendee")
            {
                // Set role as Speaker
                user.IsAttending = true;

                // Set the users event to the current event
                var currentEvent
                    = await _eventBL.GetActiveEvent();

                if (user.EventId.HasValue == false
                    || (user.EventId.HasValue == true
                    && user.EventId.Value != currentEvent.EventId))
                {
                    user.EventId = currentEvent.EventId;
                }

                // Save the changes
                await _context.SaveChangesAsync();
            }

            IsSpeaker = User.IsInRole("Speaker");

            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            if (IsSpeaker && user.SpeakerId.HasValue)
            {
                // Retrieve the associated speaker
                var speaker = await _speakerBL.GetSpeakerViewModel(user.SpeakerId.Value);

                Input = new InputModel()
                {
                    SpeakerId = speaker.SpeakerId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    GeographicLocation = user.GeographicLocation,
                    TwitterHandle = user.TwitterHandle,
                    IsVolunteer = user.IsVolunteer,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    CompanyName = speaker != null ? speaker.CompanyName : string.Empty,
                    Bio = speaker != null ? speaker.Bio : string.Empty,
                    WebsiteUrl = speaker != null ? speaker.WebsiteUrl : string.Empty,
                    BlogUrl = speaker != null ? speaker.BlogUrl : string.Empty,
                    LinkedIn = speaker != null ? speaker.LinkedIn : string.Empty,
                    IsMvp = speaker != null ? speaker.IsMvp : false,
                    NoteToOrganizers = speaker != null ? speaker.NoteToOrganizers : string.Empty
                };
            }
            else // An Attendee
            {
                Input = new InputModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    GeographicLocation = user.GeographicLocation,
                    TwitterHandle = user.TwitterHandle,
                    IsVolunteer = user.IsVolunteer,
                    Email = email,
                    PhoneNumber = phoneNumber
                };
            }

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            IsSpeaker = User.IsInRole("Speaker");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var rolesToAdd = new List<string>();
            var rolesToRemove = new List<string>();

            if (IsSpeaker)
            {
                // Retrieve the speaker object
                var speaker = _context.Speakers
                    .Where(s => s.SpeakerId == user.SpeakerId)
                    .FirstOrDefault();

                if (Input.IsVolunteer != user.IsVolunteer)
                {
                    user.IsVolunteer = Input.IsVolunteer;

                    // Add/remove roles accordingly
                    if (User.IsInRole("Volunteer") && user.IsVolunteer == false)
                        rolesToRemove.Add("Volunteer");

                    if (!User.IsInRole("Volunteer") && user.IsVolunteer == true)
                        rolesToAdd.Add("Volunteer");
                }

                if (Input.FirstName != user.FirstName)
                    user.FirstName = Input.FirstName;

                if (Input.LastName != user.LastName)
                    user.LastName = Input.LastName;

                if (Input.CompanyName != speaker.CompanyName)
                    speaker.CompanyName = Input.CompanyName;

                // Convert the image to a byte array and store it in the
                // database
                if (Input.ImageFile != null &&
                    Input.ImageFile.ContentType.ToLower().StartsWith("image/")
                    && Input.ImageFile.Length <= SpeakerViewModel.MaxImageSize)
                {
                    MemoryStream ms = new MemoryStream();
                    Input.ImageFile.OpenReadStream().CopyTo(ms);

                    speaker.Image 
                        = _speakerBL.ResizeImage(ms.ToArray());
                }

                if (Input.Bio != speaker.Bio)
                    speaker.Bio = Input.Bio;

                if (Input.WebsiteUrl != speaker.WebsiteUrl)
                    speaker.WebsiteUrl = Input.WebsiteUrl;

                if (Input.BlogUrl != speaker.BlogUrl)
                    speaker.BlogUrl = Input.BlogUrl;

                if (Input.GeographicLocation != user.GeographicLocation)
                    user.GeographicLocation = Input.GeographicLocation;

                if (Input.TwitterHandle != user.TwitterHandle)
                    user.TwitterHandle = Input.TwitterHandle;

                if (Input.LinkedIn != speaker.LinkedIn)
                    speaker.LinkedIn = Input.LinkedIn;

                if (Input.IsMvp != speaker.IsMvp)
                    speaker.IsMvp = Input.IsMvp;

                if (Input.NoteToOrganizers != speaker.NoteToOrganizers)
                    speaker.NoteToOrganizers = Input.NoteToOrganizers;

                var email = await _userManager.GetEmailAsync(user);
                if (Input.Email != email)
                {
                    var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                    }
                }

                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (Input.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                    }
                }
            }
            else // An Attendee
            {
                if (Input.IsVolunteer != user.IsVolunteer)
                {
                    user.IsVolunteer = Input.IsVolunteer;

                    // Add/remove roles accordingly
                    if (User.IsInRole("Volunteer") && user.IsVolunteer == false)
                        rolesToRemove.Add("Volunteer");

                    if (!User.IsInRole("Volunteer") && user.IsVolunteer == true)
                        rolesToAdd.Add("Volunteer");
                }

                if (Input.FirstName != user.FirstName)
                    user.FirstName = Input.FirstName;

                if (Input.LastName != user.LastName)
                    user.LastName = Input.LastName;

                if (Input.GeographicLocation != user.GeographicLocation)
                    user.GeographicLocation = Input.GeographicLocation;

                if (Input.TwitterHandle != user.TwitterHandle)
                    user.TwitterHandle = Input.TwitterHandle;

                var email = await _userManager.GetEmailAsync(user);
                if (Input.Email != email)
                {
                    var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                    }
                }

                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (Input.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                    }
                }
            }

            // Remove selected roles
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            // Add selected roles
            await _userManager.AddToRolesAsync(user, rolesToAdd);

            // Update the user
            await _userManager.UpdateAsync(user);

            // Update EF changes to DB
            await _context.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Your profile has been updated";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}
