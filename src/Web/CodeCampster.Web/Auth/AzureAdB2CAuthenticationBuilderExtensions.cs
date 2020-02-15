using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace CodeCampster.Web.Auth
{
    public static class AzureAdB2CAuthenticationBuilderExtensions
    {
        /// <summary>
        /// Adds JWT Bearer authentication to your app for Azure AD B2C Applications.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="configureOptions">The <see cref="Action{AzureADB2COptions}"/> to configure the
        /// <see cref="AzureADB2COptions"/>.
        /// </param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddAzureADB2CBearer(this AuthenticationBuilder builder, Action<AzureADB2COptions> configureOptions) =>
            builder.AddAzureADB2CBearer(
                AzureADB2CDefaults.BearerAuthenticationScheme,
                AzureADB2CDefaults.JwtBearerAuthenticationScheme,
                configureOptions);

        /// <summary>
        /// Adds JWT Bearer authentication to your app for Azure AD B2C Applications.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="scheme">The identifier for the virtual scheme.</param>
        /// <param name="jwtBearerScheme">The identifier for the underlying JWT Bearer scheme.</param>
        /// <param name="configureOptions">The <see cref="Action{AzureADB2COptions}"/> to configure the
        /// <see cref="AzureADB2COptions"/>.
        /// </param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddAzureADB2CBearer(
            this AuthenticationBuilder builder,
            string scheme,
            string jwtBearerScheme,
            Action<AzureADB2COptions> configureOptions)
        {
            builder.AddPolicyScheme(scheme, displayName: null, configureOptions: o =>
            {
                o.ForwardDefault = jwtBearerScheme;
            });

            builder.Services.Configure(TryAddJwtBearerSchemeMapping(scheme, jwtBearerScheme));

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<AzureADB2COptions>, AzureADB2COptionsConfiguration>());

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<JwtBearerOptions>, AzureADB2CJwtBearerOptionsConfiguration>());

            builder.Services.Configure(scheme, configureOptions);
            builder.AddJwtBearer(jwtBearerScheme, o => { });

            return builder;
        }

        /// <summary>
        /// Adds Azure Active Directory B2C Authentication to your application.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="configureOptions">The <see cref="Action{AzureADB2COptions}"/> to configure the
        /// <see cref="AzureADB2COptions"/>
        /// </param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddAzureADB2C(this AuthenticationBuilder builder, Action<AzureADB2COptions> configureOptions) =>
            builder.AddAzureADB2C(
                AzureADB2CDefaults.AuthenticationScheme,
                AzureADB2CDefaults.OpenIdScheme,
                AzureADB2CDefaults.CookieScheme,
                AzureADB2CDefaults.DisplayName,
                configureOptions);

        /// <summary>
        /// Adds Azure Active Directory B2C Authentication to your application.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="scheme">The identifier for the virtual scheme.</param>
        /// <param name="openIdConnectScheme">The identifier for the underlying Open ID Connect scheme.</param>
        /// <param name="cookieScheme">The identifier for the underlying cookie scheme.</param>
        /// <param name="displayName">The display name for the scheme.</param>
        /// <param name="configureOptions">The <see cref="Action{AzureADB2COptions}"/> to configure the
        /// <see cref="AzureADB2COptions"/>
        /// </param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddAzureADB2C(
            this AuthenticationBuilder builder,
            string scheme,
            string openIdConnectScheme,
            string cookieScheme,
            string displayName,
            Action<AzureADB2COptions> configureOptions)
        {
            AddAdditionalMvcApplicationParts(builder.Services);
            builder.AddPolicyScheme(scheme, displayName, o =>
            {
                o.ForwardDefault = cookieScheme;
                o.ForwardChallenge = openIdConnectScheme;
            });

            builder.Services.Configure(TryAddOpenIDCookieSchemeMappings(scheme, openIdConnectScheme, cookieScheme));

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<AzureADB2COptions>, AzureADB2COptionsConfiguration>());

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<OpenIdConnectOptions>, AzureADB2COpenIdConnectOptionsConfiguration>());

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<CookieAuthenticationOptions>, AzureADB2CCookieOptionsConfiguration>());

            builder.Services.Configure(scheme, configureOptions);

            builder.AddOpenIdConnect(openIdConnectScheme, null, o => { });
            builder.AddCookie(cookieScheme, null, o => { });

            return builder;
        }

        
        public class AzureAdB2CAuthenticationBuilderExtensions : IConfigureNamedOptions<OpenIdConnectOptions>
        {

            public AzureAdB2CAuthenticationBuilderExtensions(IOptions<AzureADB2COptions> b2cOptions)
            {
                AzureAdb2COptions = b2cOptions.Value;
            }

            public AzureADB2COptions AzureAdb2COptions { get; set; }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = AzureAdb2COptions.ClientId;
                options.Authority = AzureAdb2COptions.Authority;
                options.UseTokenLifetime = true;
                options.TokenValidationParameters = new TokenValidationParameters() { NameClaimType = "name" };

                options.Events = new OpenIdConnectEvents()
                {
                    OnRedirectToIdentityProvider = OnRedirectToIdentityProvider,
                    OnRemoteFailure = OnRemoteFailure,
                    OnAuthorizationCodeReceived = OnAuthorizationCodeReceived
                };
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }

            public Task OnRedirectToIdentityProvider(RedirectContext context)
            {
                var defaultPolicy = AzureAdb2COptions.DefaultPolicy;
                if (context.Properties.Items.TryGetValue(AzureADB2COptions.PolicyAuthenticationProperty, out var policy) &&
                    !policy.Equals(defaultPolicy))
                {
                    context.ProtocolMessage.Scope = OpenIdConnectScope.OpenIdProfile;
                    context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.IdToken;
                    context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress.ToLower().Replace(defaultPolicy.ToLower(), policy.ToLower());
                    context.Properties.Items.Remove(AzureADB2COptions.PolicyAuthenticationProperty);
                }
                else if (!string.IsNullOrEmpty(AzureAdb2COptions.ApiUrl))
                {
                    context.ProtocolMessage.Scope += $" offline_access {AzureAdb2COptions.ApiScopes}";
                    context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                }
                return Task.FromResult(0);
            }

            public Task OnRemoteFailure(RemoteFailureContext context)
            {
                context.HandleResponse();
                // Handle the error code that Azure AD B2C throws when trying to reset a password from the login page 
                // because password reset is not supported by a "sign-up or sign-in policy"
                if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("AADB2C90118"))
                {
                    // If the user clicked the reset password link, redirect to the reset password route
                    context.Response.Redirect("/Session/ResetPassword");
                }
                else if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("access_denied"))
                {
                    context.Response.Redirect("/");
                }
                else
                {
                    context.Response.Redirect("/Home/Error?message=" + Uri.EscapeDataString(context.Failure.Message));
                }
                return Task.FromResult(0);
            }

            public async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
            {
                // Use MSAL to swap the code for an access token
                // Extract the code from the response notification
                var code = context.ProtocolMessage.Code;

                string signedInUserID = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                IConfidentialClientApplication cca = ConfidentialClientApplicationBuilder.Create(AzureAdb2COptions.ClientId)
                    .WithB2CAuthority(AzureAdb2COptions.Authority)
                    .WithRedirectUri(AzureAdb2COptions.RedirectUri)
                    .WithClientSecret(AzureAdb2COptions.ClientSecret)
                    .Build();
                new MSALSessionCache(signedInUserID, context.HttpContext).EnablePersistence(cca.UserTokenCache);

                try
                {
                    AuthenticationResult result = await cca.AcquireTokenByAuthorizationCode(AzureAdb2COptions.ApiScopes.Split(' '), code)
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

        private static Action<AzureADB2CSchemeOptions> TryAddJwtBearerSchemeMapping(string scheme, string jwtBearerScheme)
        {
            return TryAddMapping;

            void TryAddMapping(AzureADB2CSchemeOptions o)
            {
                if (o.JwtBearerMappings.ContainsKey(scheme))
                {
                    throw new InvalidOperationException($"A scheme with the name '{scheme}' was already added.");
                }
                foreach (var mapping in o.JwtBearerMappings)
                {
                    if (mapping.Value.JwtBearerScheme == jwtBearerScheme)
                    {
                        throw new InvalidOperationException(
                            $"The JSON Web Token Bearer scheme '{jwtBearerScheme}' can't be associated with the Azure Active Directory B2C scheme '{scheme}'. " +
                            $"The JSON Web Token Bearer scheme '{jwtBearerScheme}' is already mapped to the Azure Active Directory B2C scheme '{mapping.Key}'");
                    }
                }
                o.JwtBearerMappings.Add(scheme, new AzureADB2CSchemeOptions.JwtBearerSchemeMapping
                {
                    JwtBearerScheme = jwtBearerScheme
                });
            };
        }

        private static Action<AzureADB2CSchemeOptions> TryAddOpenIDCookieSchemeMappings(string scheme, string openIdConnectScheme, string cookieScheme)
        {
            return TryAddMapping;

            void TryAddMapping(AzureADB2CSchemeOptions o)
            {
                if (o.OpenIDMappings.ContainsKey(scheme))
                {
                    throw new InvalidOperationException($"A scheme with the name '{scheme}' was already added.");
                }
                foreach (var mapping in o.OpenIDMappings)
                {
                    if (mapping.Value.CookieScheme == cookieScheme)
                    {
                        throw new InvalidOperationException(
                            $"The cookie scheme '{cookieScheme}' can't be associated with the Azure Active Directory B2C scheme '{scheme}'. " +
                            $"The cookie scheme '{cookieScheme}' is already mapped to the Azure Active Directory B2C scheme '{mapping.Key}'");
                    }

                    if (mapping.Value.OpenIdConnectScheme == openIdConnectScheme)
                    {
                        throw new InvalidOperationException(
                            $"The Open ID Connect scheme '{openIdConnectScheme}' can't be associated with the Azure Active Directory B2C scheme '{scheme}'. " +
                            $"The Open ID Connect scheme '{openIdConnectScheme}' is already mapped to the Azure Active Directory B2C scheme '{mapping.Key}'");
                    }
                }
                o.OpenIDMappings.Add(scheme, new AzureADB2CSchemeOptions.AzureADB2COpenIDSchemeMapping
                {
                    OpenIdConnectScheme = openIdConnectScheme,
                    CookieScheme = cookieScheme
                });
            };
        }

        private static void AddAdditionalMvcApplicationParts(IServiceCollection services)
        {
            var additionalParts = GetAdditionalParts();
            var mvcBuilder = services
                .AddMvc()
                .ConfigureApplicationPartManager(apm =>
                {
                    foreach (var part in additionalParts)
                    {
                        if (!apm.ApplicationParts.Any(ap => HasSameName(ap.Name, part.Name)))
                        {
                            apm.ApplicationParts.Add(part);
                        }
                    }

                    apm.FeatureProviders.Add(new AzureADB2CAccountControllerFeatureProvider());
                });

            bool HasSameName(string left, string right) => string.Equals(left, right, StringComparison.Ordinal);
        }
        private static IEnumerable<ApplicationPart> GetAdditionalParts()
        {
            var thisAssembly = typeof(Auth.AzureAdB2CAuthenticationBuilderExtensions).Assembly;
            var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(thisAssembly, throwOnError: true);

            foreach (var reference in relatedAssemblies)
            {
                yield return new CompiledRazorAssemblyPart(reference);
            }
        }
    }
}
