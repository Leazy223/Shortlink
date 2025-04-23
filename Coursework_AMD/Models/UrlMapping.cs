using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Coursework_AMD.Models
{
    public class UrlMapping
    {
        public int Id { get; set; }

        [Required]
        public string OriginalUrl { get; set; }

        public string ShortCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; }
    }
}