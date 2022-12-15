using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models {
    public class VestingPeriod : Managed {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public int GrantId { get; set; }
        public int VestingSheduleId { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime VestingPeriodDate { get; set; } = DateTime.UtcNow;

        [Required]
        public float Units { get; set; } = 0;

        [Required]
        public int Status { get; set; } = 0;
        /*
         * 0: Unvested
         * 1: Vested
         */
    }
}
