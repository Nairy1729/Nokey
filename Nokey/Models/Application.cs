using Nokey.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nokey.models;

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

        // Foreign Key to User (Applicant)
        [Required]
        public int ApplicantId { get; set; }

        [ForeignKey("ApplicantId")]
        public Person Applicant { get; set; }

        [Required]
        [EnumDataType(typeof(ApplicationStatus))]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Update this field only when the entity is modified
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Update UpdatedAt when modifying
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
