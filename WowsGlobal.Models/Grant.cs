using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models
{
    public class Grant : Managed
    {
        [Key]
        public int Id { get; set; }

        public int CompanyId { get; set; }
        public int PoolId { get; set; }
        public int? VestingScheduleId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int SecurityType { get; set; } = 0;
        /*
         * 0: Option
         * 1: RSU
         */

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime GrantDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int VotingType { get; set; } = 0;
        /*
         * 0: Non Voting
         * 1: Voting
        */

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime VestingStartDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime VestingCommencementDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int Units { get; set; } = 0;

        // not required as grant can be RSU
        public float ExercisePricePerOption { get; set; }
        
        [Column(TypeName = "ntext")]
        public string Note { get; set; } = null;

        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string BoardResolutionDocument { get; set; } = null;

        [Required]
        public int Status { get; set; } = 0;
        /*
         * 0: Added
         * 1: Pending Cancellation
         * 2: ESOPs Canceled
         */
    }
}
