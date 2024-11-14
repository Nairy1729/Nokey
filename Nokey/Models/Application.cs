using Nokey.models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nokey.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key to Job
        [Required]
        public int JobId { get; set; }

        [ForeignKey("JobId")]
        public Job Job { get; set; }

        // Foreign Key to User (Applicant) - now as a string to match Person Id
        [Required]
        public string ApplicantId { get; set; }

        [ForeignKey("ApplicantId")]
        public Person Applicant { get; set; }

        [Required]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Method to update timestamp
        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public enum ApplicationStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}
