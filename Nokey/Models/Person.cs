
using Nokey.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nokey.models
{
    public class Person
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required]
        public string Fullname { get; set; } // User's full name

        [Required]
        [EmailAddress]
        public string Email { get; set; } // User's email

        [Required]
        public long PhoneNumber { get; set; } // User's phone number

        [Required]
        public string Password { get; set; } // Password for authentication

        //[Required]
        //[EnumDataType(typeof(UserRole))]
        //public UserRole Role { get; set; } // Role as enum for student or recruiter

        // Profile fields as a complex type or navigational properties
        public UserProfile Profile { get; set; } = new UserProfile(); // Profile info

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class UserProfile
    {
        public string Bio { get; set; } // Short bio of the user

        public List<string> Skills { get; set; } = new List<string>(); // List of skills

        public string Resume { get; set; } // URL to resume file

        public string ResumeOriginalName { get; set; } // Original file name of resume

        public string ProfilePhoto { get; set; } = ""; // URL to profile photo
    }

    //public enum UserRole
    //{
    //    Student,
    //    Recruiter
    //}
}
