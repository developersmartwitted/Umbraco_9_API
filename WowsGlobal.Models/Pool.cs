using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models
{
    public class Pool : Managed
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; } = 0;

        public int UserId { get; set; } = 0;

        [Required]
        public int PoolSize { get; set; } = 0;

        [Required]
        public int TotalAllocated { get; set; } = 0;

        [Required]
        public int Options { get; set; } = 0;

        [Required]
        public int RSU { get; set; } = 0;

        [Required]
        public int TotalTransferable { get; set; } = 0;

        [Required]
        [Column(TypeName = "datetime")]
        // creation date
        public DateTime PoolSetupDate { get; set; } = DateTime.UtcNow;

    }
}
