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

        // =========================
        // SCHOOL CLASS
        // =========================

        builder.Entity<SchoolClass>()
            .HasIndex(c => new { c.Grade, c.Section })
            .IsUnique();

        builder.Entity<SchoolClass>()
            .HasMany(c => c.Students)
            .WithOne(u => u.SchoolClass)
            .HasForeignKey(u => u.SchoolClassId)
            .OnDelete(DeleteBehavior.SetNull);

        // =========================
        // ACTIVITY
        // =========================

        builder.Entity<Activity>()
            .HasOne(a => a.DocenteReferente)
            .WithMany()
            .HasForeignKey(a => a.DocenteReferenteId)
            .OnDelete(DeleteBehavior.Restrict);


        // =========================
        // ENROLLMENT
        // =========================

        // Uno studente non può essere iscritto
        // due volte alla stessa attività.
        builder.Entity<Enrollment>()
            .HasIndex(e => new { e.StudentId, e.ActivityId })
            .IsUnique();

        // Studente -> Enrollment
        builder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Utente che ha effettuato l'iscrizione -> Enrollment
        builder.Entity<Enrollment>()
            .HasOne(e => e.EnrolledBy)
            .WithMany()
            .HasForeignKey(e => e.EnrolledById)
            .OnDelete(DeleteBehavior.Restrict);

        // Activity -> Enrollment
        builder.Entity<Enrollment>()
            .HasOne(e => e.Activity)
            .WithMany()
            .HasForeignKey(e => e.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}