using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models {
    public class SecuritiesAllotment : Managed {
        [Key]
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public int SecuritiesId { get; set; }

        public int FundingRoundId { get; set; }

        public int ShareholderId { get; set; }

        [Required]
        public int SecurityType { get; set; } = 0;
        /*
         * 0: Ordinary share
         * 1: Preferred Share
         * 2: Convertible Notes
         * 3: Warrants
         */

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime AllotmentDate { get; set; } = DateTime.UtcNow;

        [Required]
        public float AmountInvested { get; set; } = 0;

        [Required]
        public float SharePrice { get; set; } = 0;

        [Required]
        public bool SendEmail { get; set; } = false;

        [Required]
        public bool IsVoting { get; set; } = false;

        [Required]
        public int CreationStatus { get; set; } = 0;
        /*
         * 0: Manually Created
         * 1: Created After Conversion
         */

        [Required]
        public int ConversionStatus { get; set; } = 0;
        /*
         * 0: Not converted
         * 1: Converted
         */
    }
}
