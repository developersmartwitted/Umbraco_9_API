using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models
{
    public class User : Managed
    {
        [Key]
        public int Id { get; set; }

        // don't require email for login purposes
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string UserName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Password { get; set; } = null!;

        public int RoleId { get; set; } = 0;
        /*
         * 0: Unassigned
         * 1: System Admin
         * 2: Company Admin
         * 3: Company Employee
         * 4L Shareholder
         */
    }
}
