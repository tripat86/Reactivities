using System;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    public required DbSet<Activity> Activities { get; set; }

    public required DbSet<ActivityAttendee> ActivityAttendees { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // configure many-to-many relationship
        // Here we are specifying a composite primary key for the join entity ActivityAttendee
        builder.Entity<ActivityAttendee>(x => x.HasKey(a => new { a.ActivityId, a.UserId }));

        // This means that one user can have many activities
        builder.Entity<ActivityAttendee>()
            .HasOne(u => u.User)
            .WithMany(u => u.Activities)
            .HasForeignKey(u => u.UserId);

        // This means that one activity can have many attendees. An activity can have many users  
        builder.Entity<ActivityAttendee>()
            .HasOne(a => a.Activity)
            .WithMany(a => a.Attendees)
            .HasForeignKey(a => a.ActivityId);
    }
}
