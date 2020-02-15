using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace CodeCampster.Web.Auth
{
    public class AzureADB2COptions
    {
        public const string PolicyAuthenticationProperty = "Policy";

        public AzureADB2COptions()
        {
            AzureAdB2CInstance = "https://tenant.b2clogin.com/tfp";
        }

        /// <summary>
        /// Gets or sets the OpenID Connect authentication scheme to use for authentication with this instance
        /// of Azure Active Directory B2C authentication.
        /// </summary>
        public string OpenIdConnectSchemeName { get; set; } = OpenIdConnectDefaults.AuthenticationScheme;

        /// <summary>
        /// Gets or sets the Cookie authentication scheme to use for sign in with this instance of
        /// Azure Active Directory B2C authentication.
        /// </summary>
        public string CookieSchemeName { get; set; } = CookieAuthenticationDefaults.AuthenticationScheme;

        /// <summary>
        /// Gets or sets the Jwt bearer authentication scheme to use for validating access tokens for this
        /// instance of Azure Active Directory B2C Bearer authentication.
        /// </summary>
        public string JwtBearerSchemeName { get; internal set; }
        /// <summary>
        /// Gets or sets the client Id.
        /// </summary>
        public string ClientId { get; set; }
        public string AzureAdB2CInstance { get; set; }
        public string Tenant { get; set; }
        public string SignUpSignInPolicyId { get; set; }
        public string SignInPolicyId { get; set; }
        public string SignUpPolicyId { get; set; }
        public string ResetPasswordPolicyId { get; set; }
        public string EditProfilePolicyId { get; set; }
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the default policy.
        /// </summary>
        public string DefaultPolicy => SignUpSignInPolicyId;
        public string Authority => $"{AzureAdB2CInstance}/{Tenant}/{DefaultPolicy}/v2.0";

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        public string ClientSecret { get; set; }

        public string ApiUrl { get; set; }
        public string ApiScopes { get; set; }

        /// <summary>
        /// Gets all the underlying authentication schemes.
        /// </summary>
        public string[] AllSchemes => new[] { CookieSchemeName, OpenIdConnectSchemeName };
    }
}
