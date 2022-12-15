using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models {
    public class VestingSchedule : Managed {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public int GrantId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; } = null!;

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime VestingStartDate { get; set; } = DateTime.UtcNow;

        public int CliffDays { get; set; } = 0;

        [Required]
        public int SegmentInterval { get; set; } = 0;

        [Required]
        public int IntervalType { get; set; } = 0;
        /*
         * 0: Days
         * 1: Months
         * 2: Years
         */

        [Required]
        public int VestingSegments { get; set; } = 1;

        [Column(TypeName = "datetime")]
        public DateTime? FixedDurationEnd { get; set; } = null;

        [Required]
        public int EmployedExercisePeriod { get; set; } = 0;
        /*
         * 0: At liquidity event
         * 1: Anytime
         * 2: Custom period
         */

        [Required]
        public int ResignedExercisePeriod { get; set; } = 0;
        /*
         * 0: At liquidity event
         * 1: Specific duration
         */

        [Required]
        public int EmploymentEndedType { get; set; } = 0;
        /*
         * 0: Employed
         * 1: Resigned
         * 2: Terminated with cause
         * 3: Terminated without cause
         */

        [Required]
        public int TotalVestedUnits { get; set; } = 0;

        [Required]
        public int TotalExercisedUnits { get; set; } = 0;

        [Required]
        public int TotalUnvestedUnits { get; set; } = 0;

        [Column(TypeName = "ntext")]
        public string Note { get; set; }

        [Required]
        public int Status { get; set; } = 0;
        /*
         * 0: Active
         * 1: Not Active
         */

    }
}
