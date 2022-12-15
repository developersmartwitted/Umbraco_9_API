using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models 
{
    public class FormNewsletterSubscribe {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; } = null!;

        public int Status { get; set; } = 0;
        /*
         * 0: Unassigned
         */
    }
}