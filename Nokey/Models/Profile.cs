using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nokey.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Person")]
        public string PersonId { get; set; }


        [MaxLength(500)]
        public string Bio { get; set; } = "";

        [NotMapped]
        public List<string> Skills { get; set; } = new List<string>();

        [Column(TypeName = "varbinary(max)")]
        public byte[]? Resume { get; set; } = null;

        public string? ResumeFileName { get; set; }

        [MaxLength(255)]
        public string? ProfilePhoto { get; set; } = "";
    }
}