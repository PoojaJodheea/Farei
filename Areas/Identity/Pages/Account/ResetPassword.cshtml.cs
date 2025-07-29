using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using FormRequest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace FormRequest.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ResetPasswordModel> _logger;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager,
                                  ILogger<ResetPasswordModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [StringLength(100, ErrorMessage = "The password must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Reset code is required.")]
            public string Code { get; set; }
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A password reset code must be supplied.");
            }

            Input = new InputModel
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
               
                _logger.LogWarning("Password reset attempt for non-existing email: {Email}", Input.Email);
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Password successfully reset for user {Email}", Input.Email);
                return RedirectToPage("./Login");
                // Optionally redirect to login: return RedirectToPage("./Login");
            }

            // Handle specific token expiration or invalid cases
            foreach (var error in result.Errors)
            {
                if (error.Code == "InvalidToken")
                {
                    ModelState.AddModelError(string.Empty, "The password reset link has expired or is invalid. Please request a new one.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            _logger.LogWarning("Password reset failed for user {Email}. Errors: {Errors}",
                Input.Email, string.Join(", ", result.Errors));

            return Page();
        }
    }
}


//user clicks reset link- reads and decodes reset token from URL
//User enters new pw and email,if email=valid ,uses token+ new password to reset passwrd