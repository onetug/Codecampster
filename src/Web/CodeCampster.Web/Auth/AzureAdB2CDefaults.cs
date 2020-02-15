using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCampster.Web.Auth
{
    /// <summary>
    /// Constants for different Azure Active Directory B2C authentication components.
    /// </summary>
    public static class AzureADB2CDefaults
    {
        /// <summary>
        /// The key for the policy used in <see cref="AuthenticationProperties"/>.
        /// </summary>
        public static readonly string PolicyKey = "Policy";
        
        /// <summary>
        /// The scheme name for Open ID Connect when using
        /// </summary>
        public const string OpenIdScheme = "AzureADB2COpenID";

        /// <summary>
        /// The scheme name for cookies when using
        /// </summary>
        public const string CookieScheme = "AzureADB2CCookie";

        /// <summary>
        /// The default scheme for Azure Active Directory B2C Bearer.
        /// </summary>
        public const string BearerAuthenticationScheme = "AzureADB2CBearer";

        /// <summary>
        /// The scheme name for JWT Bearer when using
        /// </summary>
        public const string JwtBearerAuthenticationScheme = "AzureADB2CJwtBearer";

        /// <summary>
        /// The default scheme for Azure Active Directory B2C.
        /// </summary>
        public const string AuthenticationScheme = "AzureADB2C";

        /// <summary>
        /// The display name for Azure Active Directory B2C.
        /// </summary>
        public static readonly string DisplayName = "Azure Active Directory B2C";
    }
}
