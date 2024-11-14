//using Nokey.models;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Nokey.Models
//{
//    public class Job
//    {
//        [Key]
//        public int Id { get; set; }

//        public string Title { get; set; }
//        public string Description { get; set; }
//        public decimal Salary { get; set; }

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

//        // Foreign Key to Company
//        public int CompanyId { get; set; }
//        //public Company Company { get; set; }

//        // Foreign Key to Person (CreatedBy) - changed to string to match Person.Id
//        [Required]
//        public string CreatedById { get; set; }  // Changed to string

//        [ForeignKey("CreatedById")]
//        //public Person CreatedBy { get; set; }  // Navigation property to Person who created the job

//        // Navigation to JobRequirements
//        public ICollection<JobRequirement> Requirements { get; set; } = new List<JobRequirement>();

//        // Navigation to Applications
//        //public ICollection<Application> Applications { get; set; } = new List<Application>();
//    }
//}


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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

        // Foreign Key to Company
        public int CompanyId { get; set; }

        // Foreign Key to Person (CreatedBy)
        [Required]
        public string CreatedById { get; set; }

        // Instead of JobRequirement, storing multiple strings
        [NotMapped]
        public List<string> Requirements { get; set; } = new List<string>();

        // Store Requirements as a single comma-separated string in the database
        [Column("Requirements")]
        public string RequirementsString
        {
            get => string.Join(",", Requirements);  // Convert List<string> to a comma-separated string
            set => Requirements = value?.Split(',')?.ToList();  // Convert the comma-separated string back to a List<string>
        }
    }
}
