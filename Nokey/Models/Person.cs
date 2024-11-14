using Nokey.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Nokey.Models
{
    public class Person
    {
        [Key, ForeignKey("ApplicationUser")]
        public string Id { get; set; } // Primary key matching ApplicationUser ID

        [Required]
        public string Fullname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public long PhoneNumber { get; set; } // Use string if international numbers are possible

        [Required]
        public string Role { get; set; } // Role of the user

        // Owned attribute ensures UserProfile fields are stored in the Person table
        public UserProfile Profile { get; set; } = new UserProfile();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        //public ApplicationUser ApplicationUser { get; set; } // Navigation property to ApplicationUser
    }

    // Mark UserProfile as an owned type so that its properties are stored in the Person table
    [Owned]
    public class UserProfile
    {
        [MaxLength(500)]
        public string Bio { get; set; } = ""; // Short bio of the user

        // Skills stored as a List<string>, converted in OnModelCreating for compatibility
        [NotMapped] // Configure in OnModelCreating instead
        public List<string> Skills { get; set; } = new List<string>();

        [Column(TypeName = "varbinary(max)")]
        public byte[]? Resume { get; set; } = null; // Allow nullable byte array
                                                    // Initialize with empty byte array


        public string? ResumeFileName { get; set; } // Original file name of resume

        [MaxLength(255)]
        public string? ProfilePhoto { get; set; } = ""; // URL or path to profile photo
    }
}
