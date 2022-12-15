using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models
{
    public class Address : Managed
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ShareholderId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Address1 { get; set; } = null!;

        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Address2 { get; set; }

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string City { get; set; } = null!;

        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string District { get; set; } // state, province, county

        [Required]
        public int? CountryId { get; set; }

        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string PostalCode { get; set; } // not all areas have postal code
    }
}
