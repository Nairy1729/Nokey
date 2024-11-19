

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

        public int CompanyId { get; set; }

        [Required]
        public string CreatedById { get; set; }

        [NotMapped]
        public List<string> Requirements { get; set; } = new List<string>();

        [Column("Requirements")]
        public string RequirementsString
        {
            get => string.Join(",", Requirements);  
            set => Requirements = value?.Split(',')?.ToList();  
        }
    }
}
