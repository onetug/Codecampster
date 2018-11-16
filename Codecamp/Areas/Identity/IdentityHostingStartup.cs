using System;
using Codecamp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

//[assembly: HostingStartup(typeof(Codecamp.Areas.Identity.IdentityHostingStartup))]
namespace Codecamp.Areas.Identity
{
    //public class IdentityHostingStartup : IHostingStartup
    //{
    //    public void Configure(IWebHostBuilder builder)
    //    {
    //        builder.ConfigureServices((context, services) => {
    //            services.AddDbContext<CodecampDbContext>(options =>
    //                options.UseSqlServer(
    //                    context.Configuration.GetConnectionString("CodecampDbContextConnection")));

    //            services.AddIdentity<CodecampUser, IdentityRole>()
    //                .AddEntityFrameworkStores<CodecampDbContext>();
    //        });
    //    }
    //}
}