using Codecamp.BusinessLogic;
using Codecamp.BusinessLogic.Api;
using Codecamp.Data;
using Codecamp.Models;
using Codecamp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codecamp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<CodecampDbContext>(options =>
                // options.UseSqlServer(Configuration.GetConnectionString("ProductionDbContextConnection")));
                options.UseSqlServer(Configuration.GetConnectionString("CodecampDbContextConnection")));

            services.AddIdentity<CodecampUser, IdentityRole>()
                .AddEntityFrameworkStores<CodecampDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(options =>
                {
                    options.CacheProfiles.Add("Default300",
                        new CacheProfile()
                        {
                            Duration = 300
                        });
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddRazorPagesOptions(options =>
                {
                    options.AllowAreas = true;
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            services.AddSingleton<IEmailSender, AuthMessageEmailSender>();

            services.AddTransient<ISpeakerBusinessLogic, SpeakerBusinessLogic>();
            services.AddTransient<IUserBusinessLogic, UserBusinessLogic>();
            services.AddTransient<IEventBusinessLogic, EventBusinessLogic>();
            services.AddTransient<ISessionBusinessLogic, SessionBusinessLogic>();
            services.AddTransient<ISponsorBusinessLogic, SponsorBusinessLogic>();
            services.AddTransient<IAnnouncementBusinessLogic, AnnouncementBusinessLogic>();
            services.AddTransient<ITimeslotBusinessLogic, TimeslotBusinessLogic>();
            services.AddTransient<ITrackBusinessLogic, TrackBusinessLogic>();
            services.AddTransient<IScheduleBusinessLogic, ScheduleBusinessLogic>();

            services.AddTransient<IAnnouncementsApiBusinessLogic, AnnouncementsApiBusinessLogic>();
            services.AddTransient<IEventsApiBusinessLogic, EventsApiBusinessLogic>();
            services.AddTransient<ISessionsApiBusinessLogic, SessionsApiBusinessLogic>();
            services.AddTransient<ISpeakersApiBusinessLogic, SpeakersApiBusinessLogic>();
            services.AddTransient<ISponsorsApiBusinessLogic, SponsorsApiBusinessLogic>();
            services.AddTransient<ITimeslotsApiBusinessLogic,
                TimeslotsApiBusinessLogic>();
            services.AddTransient<ITracksApiBusinessLogic, TracksApiBusinessLogic>();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            // Register the event business logic service
            services.AddTransient<EventBusinessLogic>();

            services.Configure<AppOptions>(Configuration.GetSection("AppSettings"));
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireSpeakerRole", policy => policy.RequireRole("Speaker"));
                options.AddPolicy("RequireVolunteerRole", policy => policy.RequireRole("Volunteer"));
            });

            services.AddSession();

            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1",
                    new Info { Title = "Orlando Code Camp API", Version = "v1"});

                // Use controller API group name instead of controller name
                options.DocInclusionPredicate((_, api) =>
                    !string.IsNullOrWhiteSpace(api.GroupName));
                options.TagActionsBy(api => new List<string> { api.GroupName });

                options.CustomSchemaIds(SchemaIdStrategy);

                string SchemaIdStrategy(Type type)
                {
                    var typeName = type.Name;

                    if (typeName.StartsWith("Api"))
                        typeName = typeName.Split("Api",
                            StringSplitOptions.RemoveEmptyEntries)[0];

                    return typeName;
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseDatabaseErrorPage();
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // Enable and use authentication
            app.UseAuthentication();

            app.UseSession();

            app.UseSwagger();
            app.UseSwaggerUI(setup =>
            {
                setup.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "Orlando Code Camp API V1");

                // Don't show the models section at the end of the Swagger page
                setup.DefaultModelsExpandDepth(-1);

                setup.DisplayRequestDuration();

                // Expand to Action level (Group name + Action name)
                setup.DocExpansion(DocExpansion.List);
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Create user roles and admin user
            CreateRoles(serviceProvider).Wait();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<CodecampUser>>();

            string[] roleNames = { "Admin", "Speaker", "Volunteer", "Attendee" };

            IdentityResult roleResult;

            // Create the application roles, if they do not exist
            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Create the site admin user
            var adminUsername = Configuration.GetSection("AppSettings")["AdminUser"];
            var adminEmail = Configuration.GetSection("AppSettings")["AdminUser"];
            var adminPassword = Configuration.GetSection("AppSettings")["AdminPass"];
            if (adminUsername.Length != 0 && adminPassword.Length != 0
                && adminEmail.Length != 0)
            {
                var sysAdmin = new CodecampUser
                {
                    UserName = adminUsername,
                    Email = adminEmail,
                    EmailConfirmed = true,
                };

                var _user = await userManager.FindByEmailAsync(adminEmail);
                if (_user == null)
                {
                    var createAdminUserResult
                        = await userManager.CreateAsync(sysAdmin, adminPassword);

                    if (createAdminUserResult.Succeeded)
                        // Assign the sys admin user to the admin role
                        await userManager.AddToRoleAsync(sysAdmin, "Admin");
                }
            }
        }
    }
}
