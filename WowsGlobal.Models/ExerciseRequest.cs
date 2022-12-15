using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models
{
    public class ExerciseRequest : Managed
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


        [Column(TypeName = "datetime")]
        public DateTime? TransactionDate { get; set; } = null;

        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string TransactionProof { get; set; } = null;

        [Required]
        public int Status { get; set; } = 0;
        /*
         * 0: Unassigned
         * 1: Unexercised
         * 2: Pending payment
         * 3: Paid
         * 4: Exercised
         */
    }
}
