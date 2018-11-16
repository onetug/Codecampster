using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codecamp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Codecamp.Data
{
    public class CodecampDbContext : IdentityDbContext<CodecampUser>
    {
        // Collection of Codecamp users
        public DbSet<CodecampUser> CodecampUsers { get; set; }

        // Collection of speakers
        public DbSet<Speaker> Speakers { get; set; }

        // Collection of sessions
        public DbSet<Session> Sessions { get; set; }

        // Colletion of events
        public DbSet<Event> Events { get; set; }

        public DbSet<AttendeeSession> AttendeesSessions { get; set; }

        public CodecampDbContext(DbContextOptions<CodecampDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<Session>().HasMany(s => s.Speakers);

            builder.Entity<Speaker>().HasMany(s => s.Sessions);

        }
    }
}
