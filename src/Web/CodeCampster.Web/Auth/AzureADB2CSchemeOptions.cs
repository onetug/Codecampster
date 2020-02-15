using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCampster.Web.Auth
{
    internal class AzureADB2CSchemeOptions
    {
        public IDictionary<string, AzureADB2COpenIDSchemeMapping> OpenIDMappings { get; set; } = new Dictionary<string, AzureADB2COpenIDSchemeMapping>();

        public IDictionary<string, JwtBearerSchemeMapping> JwtBearerMappings { get; set; } = new Dictionary<string, JwtBearerSchemeMapping>();

        public class AzureADB2COpenIDSchemeMapping
        {
            public string OpenIdConnectScheme { get; set; }
            public string CookieScheme { get; set; }
        }

        public class JwtBearerSchemeMapping
        {
            public string JwtBearerScheme { get; set; }
        }
    }
}
