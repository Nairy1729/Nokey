using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nokey.models;
using Nokey.Models;
using System.Collections.Generic;

namespace Nokey.Authentication
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        { }

        // DbSets for your entities
        public DbSet<Person> Persons { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<JobRequirement> JobRequirements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User (Person) entity with UserProfile
            modelBuilder.Entity<Person>()
                .OwnsOne(u => u.Profile, profile =>
                {
                    // Configure Skills as a comma-separated list if Skills is a collection in UserProfile
                    profile.Property(p => p.Skills)
                        .HasConversion(
                            v => string.Join(',', v), // Convert list to a comma-separated string
                            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries) // Convert string back to list
                                  .ToList()
                        );

                    profile.Property(p => p.Bio).HasMaxLength(500); // Limit Bio length
                    profile.Property(p => p.Resume).HasColumnType("varbinary(max)"); // Store Resume as binary data if needed
                    profile.Property(p => p.ProfilePhoto).HasMaxLength(255); // Store Profile Photo path or URL
                });

            // Ensure email uniqueness for Person
            modelBuilder.Entity<Person>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Job-Company relationship (using foreign key without navigation property)
            modelBuilder.Entity<Job>()
                .HasOne<Company>()
                .WithMany()
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Job-JobRequirement relationship (using foreign key without navigation property)
            modelBuilder.Entity<JobRequirement>()
                .HasOne<Job>()
                .WithMany()
                .HasForeignKey(jr => jr.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // Job-CreatedBy relationship (using foreign key without navigation property)
            modelBuilder.Entity<Job>()
                .HasOne<Person>()
                .WithMany()
                .HasForeignKey(j => j.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Application-Job relationship (using foreign key without navigation property)
            modelBuilder.Entity<Application>()
                .HasOne<Job>()
                .WithMany()
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // Application-Applicant relationship (using foreign key without navigation property)
            modelBuilder.Entity<Application>()
                .HasOne<Person>()
                .WithMany()
                .HasForeignKey(a => a.ApplicantId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
