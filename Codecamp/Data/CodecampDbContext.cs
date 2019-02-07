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

        public DbSet<SpeakerSession> SpeakerSessions { get; set; }

        public DbSet<Timeslot> Timeslots { get; set; }

        public DbSet<Track> Tracks { get; set; }

        public DbSet<Sponsor> Sponsors { get; set; }

        public DbSet<Announcement> Announcements { get; set; }

        public DbSet<Schedule> CodecampSchedule { get; set; }

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

            // Many to many relationship between speakers and sessions
            builder.Entity<SpeakerSession>()
                .HasKey(ss => new { ss.SpeakerId, ss.SessionId });

            builder.Entity<SpeakerSession>()
                .HasOne(ss => ss.Speaker)
                .WithMany(ss => ss.SpeakerSessions)
                .HasForeignKey(ss => ss.SpeakerId);

            builder.Entity<SpeakerSession>()
                .HasOne(ss => ss.Session)
                .WithMany(ss => ss.SpeakerSessions)
                .HasForeignKey(ss => ss.SessionId);

            // Many to many relationship between attendees and sessions
            builder.Entity<AttendeeSession>()
                .HasKey(_as => new { _as.CodecampUserId, _as.SessionId });

            builder.Entity<AttendeeSession>()
                .HasOne(_as => _as.CodecampUser)
                .WithMany(_as => _as.AttendeeSessions)
                .HasForeignKey(_as => _as.CodecampUserId);

            builder.Entity<AttendeeSession>()
                .HasOne(_as => _as.Session)
                .WithMany(_as => _as.AttendeeSessions)
                .HasForeignKey(_as => _as.SessionId);

            builder.Entity<Schedule>()
                .HasKey(s => new { s.SessionId,
                    s.TrackId, s.TimeslotId });
        }
    }
}
