using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models {
    public class Shareholder : Managed {
        [Key]
        public int Id { get; set; }

        public int CompanyId { get; set; }

        [Required]
        public int InvestorType { get; set; } = 0;
        /*
         * 0: Individual
         * 1: Institution
         */

        [Required]
        public int InstituitionalType { get; set; } = 0;
        /*
         * 0: Venture Capital
         * 1: Private Equity
         * 2: Family Office
         * 3: Other
         */

        [Required]
        public int ShareholderType { get; set; } = 0;
        /*
         * 0: Investor
         * 1: Founder
         * 2: Employee
         */

        [Required]
        public int CitizenshipCountryId { get; set; }

        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string BrandName { get; set; } = null;

        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string LegalName { get; set; } = null;

        [MaxLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string Salutation { get; set; } = null;

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string FirstName { get; set; } = null!;

        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string MiddleName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; } = null!;

        [Required]
        public int TaxCountryId { get; set; }

        public virtual ICollection<Phone> Phones { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }

        [Required]
        public int Status { get; set; } = 0;
        /*
         * 0: Inactive
         * 1: Active
         */
    }
}
