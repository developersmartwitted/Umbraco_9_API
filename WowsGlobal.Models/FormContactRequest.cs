using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models {
    public class FormContactRequest {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; } = null!;

        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Topic { get; set; } = null!;

        [MaxLength(500)]
        [Column(TypeName = "ntext")]
        public string Note { get; set; }

        [MaxLength(1000)]
        [Column(TypeName = "ntext")]
        public string Recaptcha { get; set; }
        
        public int Status { get; set; } = 0;
        /*
         * 0: Unassigned
         * 1: Responded
         * 2: Engaged
         */
    }
}