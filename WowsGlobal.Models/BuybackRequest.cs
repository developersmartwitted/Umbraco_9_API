using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models
{
    public class BuybackRequest : Managed
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public int GrantId { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int Units { get; set; } = 0;

        public int? ResponseUnits { get; set; } = null;

        [Column(TypeName = "datetime")]
        public DateTime? TransactionDate { get; set; } = null;

        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string TransactionProof { get; set; } = null;

        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }

        [Required]
        public int Status { get; set; } = 0;
        /*
         * 0: Unassigned
         * 1: Active
         * 2: Expired
         * 3: Accepted
         * 4: Rejected
         */
    }
}
