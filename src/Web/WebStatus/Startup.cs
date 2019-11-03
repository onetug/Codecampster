using HealthChecks.UI.Client;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace WebStatus
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
            RegisterAppInsights(services);

            services.AddOptions();
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddApplicationInsightsPublisher(Configuration["ApplicationInsights:InstrumentationKey"]);
            services.AddHealthChecksUI();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
                // app.UseForwardedHeaders(new ForwardedHeadersOptions
                // {
                //     ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                // });

            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting()
                .UseEndpoints(config =>
                {
                    config.MapHealthChecks("/liveness", new HealthCheckOptions
                    {
                        Predicate = r => r.Name.Contains("self")
                    });
                    config.MapHealthChecksUI(hcUIOptions => { hcUIOptions.UIPath = "/hc-ui"; });
                    config.MapDefaultControllerRoute();
                });
        }

        private void RegisterAppInsights(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);
            var orchestratorType = Configuration.GetValue<string>("OrchestratorType");

            if (orchestratorType?.ToUpper() == "K8S")
            {
                // Enable K8s telemetry initializer
                services.AddApplicationInsightsKubernetesEnricher();
            }
        }
    }
}