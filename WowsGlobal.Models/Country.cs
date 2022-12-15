using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(5)]
        [Column(TypeName = "varchar(5)")]
        public string PhonePrefix { get; set; } = "+1";

    }
}
