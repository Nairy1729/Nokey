using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nokey.Models;

namespace Nokey.Authentication
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        { }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Profile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Person-Profile relationship
            modelBuilder.Entity<Profile>()
                .HasOne<Person>() // A Profile is associated with one Person
                .WithMany()       // A Person can have many related entities (if needed, can be empty)
                .HasForeignKey(p => p.PersonId) // Use PersonId as the foreign key
                .OnDelete(DeleteBehavior.Cascade); // Delete Profile when Person is deleted

            // Ensure unique email for Person
            modelBuilder.Entity<Person>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure Company-Job relationship
            modelBuilder.Entity<Job>()
                .HasOne<Company>() // Job belongs to a Company
                .WithMany()        // A Company can have many Jobs
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for Company-Job relationship

            // Configure Job's CreatedById relationship
            modelBuilder.Entity<Job>()
                .HasOne<Person>() // Job is created by a Person
                .WithMany()       // A Person can create many Jobs
                .HasForeignKey(j => j.CreatedById)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete on Job's CreatedById

            // Configure Application-Job relationship
            modelBuilder.Entity<Application>()
                .HasOne<Job>() // Application belongs to a Job
                .WithMany()    // A Job can have many Applications
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete on Application's JobId

            // Configure Application-Applicant relationship
            modelBuilder.Entity<Application>()
                .HasOne<Person>() // Application belongs to a Person (Applicant)
                .WithMany()       // A Person can apply to many Jobs
                .HasForeignKey(a => a.ApplicantId)
                .OnDelete(DeleteBehavior.NoAction); // No cascading delete on Application's ApplicantId

            // Configure Skills property in Profile
            modelBuilder.Entity<Profile>()
                .Property(p => p.Skills)
                .HasConversion(
                    v => string.Join(',', v), // Convert list to a comma-separated string
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() // Convert string back to list
                );
        }
    }
}
