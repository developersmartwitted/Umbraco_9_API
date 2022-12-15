using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models {
    public class FundingRound : Managed {
        [Key]
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public int SecuritiesId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string Name { get; set; } = null;

        [Required]
        public int SecurityType { get; set; } = 0;
        /*
         * 0: Ordinary share
         * 1: Preferred Share
         * 2: Convertible Notes
         * 3: Warrants
         */

        [Required]
        public float RaisedAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime ClosingDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int SharesIssued { get; set; } = 0;

        [Required]
        public float PricePerShare { get; set; } = 0;

        [Column(TypeName = "ntext")]
        public string Note { get; set; }
    }
}
