using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models {
    public class Securities : Managed {
        [Key]
        public int Id { get; set; }

        public int CompanyId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string Name { get; set; } = null;

        [Required]
        [MaxLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string EqPrefix { get; set; } = null;

        public float FaceValue { get; set; }

        [Required]
        public float PricePerShare { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime BoardFirstIssueDate { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string LegalDocument { get; set; } = null;

        public float Multiplier { get; set; }

        public bool IsParticipating { get; set; } = false;

        public int ParticipationCap { get; set; } = 0;

        public float ConversionPrice { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? MaturityDate { get; set; } = null;

        public float PreMoneyValuation { get; set; }

        public float PreMoneyValuationCap { get; set; }

        public float ConversionDiscount { get; set; }

        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string ConvertToFundingRound { get; set; } = null;

        [MaxLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string ConvertToPrefix { get; set; } = null;

        public float ExercisePrice { get; set; }

        [Required]
        public bool IsVoting { get; set; } = false;
    }
}
