using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace CodeCampster.Web.Auth
{
    internal class AzureADB2COpenIDConnectEventHandlers
    {
        private IDictionary<string, string> _policyToIssuerAddress =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public AzureADB2COpenIDConnectEventHandlers(string schemeName, AzureADB2COptions options)
        {
            SchemeName = schemeName;
            Options = options;
        }

        public string SchemeName { get; }

        public AzureADB2COptions Options { get; }

        public Task OnRedirectToIdentityProvider(RedirectContext context)
        {
            var defaultPolicy = Options.DefaultPolicy;
            if (context.Properties.Items.TryGetValue(AzureADB2COptions.PolicyAuthenticationProperty, out var policy) &&
                !policy.Equals(defaultPolicy))
            {
                context.ProtocolMessage.Scope = OpenIdConnectScope.OpenIdProfile;
                context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.IdToken;
                context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress.ToLower().Replace(defaultPolicy.ToLower(), policy.ToLower());
                context.Properties.Items.Remove(AzureADB2COptions.PolicyAuthenticationProperty);
            }
            else if (!string.IsNullOrEmpty(Options.ApiUrl))
            {
                context.ProtocolMessage.Scope += $" offline_access {Options.ApiScopes}";
                context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.CodeIdToken;
            }
            context.ProtocolMessage.RedirectUri = Options.RedirectUri;
            return Task.FromResult(0);
        }

        public Task OnRemoteFailure(RemoteFailureContext context)
        {
            context.HandleResponse();
            // Handle the error code that Azure Active Directory B2C throws when trying to reset a password from the login page 
            // because password reset is not supported by a "sign-up or sign-in policy".
            // Below is a sample error message:
            // 'access_denied', error_description: 'AADB2C90118: The user has forgotten their password.
            // Correlation ID: f99deff4-f43b-43cc-b4e7-36141dbaf0a0
            // Timestamp: 2018-03-05 02:49:35Z
            //', error_uri: 'error_uri is null'.
            if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("AADB2C90118"))
            {
                // If the user clicked the reset password link, redirect to the reset password route
                context.Response.Redirect($"{context.Request.PathBase}/AzureADB2C/Account/ResetPassword/{SchemeName}");
            }
            // Access denied errors happen when a user cancels an action on the Azure Active Directory B2C UI. We just redirect back to
            // the main page in that case.
            // Message contains error: 'access_denied', error_description: 'AADB2C90091: The user has cancelled entering self-asserted information.
            // Correlation ID: d01c8878-0732-4eb2-beb8-da82a57432e0
            // Timestamp: 2018-03-05 02:56:49Z
            // ', error_uri: 'error_uri is null'.
            else if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("access_denied"))
            {
                context.Response.Redirect($"{context.Request.PathBase}/");
            }
            else
            {
                context.Response.Redirect($"{context.Request.PathBase}/AzureADB2C/Account/Error");
            }

            return Task.CompletedTask;
        }

        public async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            // Use MSAL to swap the code for an access token
            // Extract the code from the response notification
            var code = context.ProtocolMessage.Code;
            string signedInUserID = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            IConfidentialClientApplication cca = ConfidentialClientApplicationBuilder.Create(Options.ClientId)
                .WithB2CAuthority(Options.Authority)
                .WithRedirectUri(Options.RedirectUri)
                .WithClientSecret(Options.ClientSecret)
                .Build();
            new MSALSessionCache(signedInUserID, context.HttpContext).EnablePersistence(cca.UserTokenCache);

            try
            {
                AuthenticationResult result = await cca.AcquireTokenByAuthorizationCode(Options.ApiScopes.Split(' '), code)
                    .ExecuteAsync();


                context.HandleCodeRedemption(result.AccessToken, result.IdToken);
            }
            catch (Exception ex)
            {
                //TODO: Handle
                throw;
            }
        }
    }
}
