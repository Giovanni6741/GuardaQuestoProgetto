using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PariniFSL.Models;

namespace PariniFSL.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Activity> Activities { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<SchoolClass> SchoolClasses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<SchoolClass>()
            .HasIndex(c => new { c.Grade, c.Section })
            .IsUnique();

        builder.Entity<SchoolClass>()
            .HasMany(c => c.Students)
            .WithOne(u => u.SchoolClass)
            .HasForeignKey(u => u.SchoolClassId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}