using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models
{
    public abstract class Managed
    {
        [Required]
        [Column(TypeName = "datetime")]
        // creation date
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "datetime")]
        // creation date
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
