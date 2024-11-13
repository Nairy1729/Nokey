using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nokey.models;
using Nokey.Models;

namespace Nokey.Authentication
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        { }

        //Scaffold-DbContext "Server=NAIRY;Database=API_CF_Demo;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models/SecondDatabase -Context SecondDbContext -ContextDir Data/Contexts -Force

        public DbSet<Person> Persons { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<JobRequirement> JobRequirements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User (Person) entity configuration
            modelBuilder.Entity<Person>()
                .OwnsOne(u => u.Profile, profile =>
                {
                    profile.Property(p => p.Skills)
                        .HasConversion(
                            v => string.Join(',', v),
                            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .ToList()
                        );
                });

            modelBuilder.Entity<Person>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Company and Job relationship configuration
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Company)
                .WithMany(c => c.Jobs) // Use 'Jobs' as the collection property in Company
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Job and JobRequirement relationship configuration
            modelBuilder.Entity<JobRequirement>()
                .HasOne(jr => jr.Job)
                .WithMany(j => j.Requirements)
                .HasForeignKey(jr => jr.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // Job and Person (User) relationship for CreatedBy field
            modelBuilder.Entity<Job>()
                .HasOne(j => j.CreatedBy)
                .WithMany()
                .HasForeignKey(j => j.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Application relationships with Job and Person (User)
            modelBuilder.Entity<Application>()
                .HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // Change this to NoAction to prevent cascading delete for Applicant
            modelBuilder.Entity<Application>()
                .HasOne(a => a.Applicant)
                .WithMany()
                .HasForeignKey(a => a.ApplicantId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent cascading deletes for ApplicantId
        }


    }
}
