using Nokey.models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nokey.Models
{
    public class Job
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Salary { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public int CreatedById { get; set; }
        public Person CreatedBy { get; set; }  // Navigation property to Person who created the job

        public ICollection<JobRequirement> Requirements { get; set; } = new List<JobRequirement>(); // Navigation to JobRequirements
        public ICollection<Application> Applications { get; set; } = new List<Application>(); // Navigation to Applications
    }
}
