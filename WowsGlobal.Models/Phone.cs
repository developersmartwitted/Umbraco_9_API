using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models
{
    public class Phone : Managed
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CountryId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string PhoneNumber { get; set; } = null!;

        [MaxLength(5)]
        [Column(TypeName = "nvarchar(5)")]
        public string AltPhonePrefix { get; set; } = null;
        [Required]
        public int ShareholderId { get; set; }
        [Required]
        public int employeeId { get; set; }
    }
}
