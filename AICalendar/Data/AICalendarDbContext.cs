using AICalendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.Data
{
    public class AppDbContext : DbContext   // renamed class
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)  //  updated constructor
            : base(options)
        {
        }

        public DbSet<Calendar> Calendars => Set<Calendar>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Attendee> Attendees => Set<Attendee>();
        public DbSet<Reminder> Reminders => Set<Reminder>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Calendar)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CalendarId);

            modelBuilder.Entity<Attendee>()
                .HasOne(a => a.Event)
                .WithMany(e => e.Attendees)
                .HasForeignKey(a => a.EventId);

            modelBuilder.Entity<Reminder>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Reminders)
                .HasForeignKey(r => r.EventId);
        }
    }
}
