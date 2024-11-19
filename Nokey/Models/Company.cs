﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nokey.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [StringLength(100)]
        public string Name { get; set; } 

        public string Description { get; set; } 

        [Url]
        public string Website { get; set; } 

        public string Location { get; set; } 

        public string Logo { get; set; } 

        [Required]
        public string PersonId { get; set; }

        [ForeignKey("PersonId")]


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
