using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nokey.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; } // Primary key for Company

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Company name

        public string Description { get; set; } // Company description

        [Url]
        public string Website { get; set; } // Company website URL

        public string Location { get; set; } // Company location

        public string Logo { get; set; } // URL for the company logo

        [Required]
        public string PersonId { get; set; }

        [ForeignKey("PersonId")]


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
