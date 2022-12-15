using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowsGlobal.Models
{
    public class Employee : Managed
    {
        [Key]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string FullName { get; set; } = null!;


        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string EmployeeId { get; set; } = null;

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime JoiningDate { get; set; } = DateTime.UtcNow;


        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Designation { get; set; } = null;


        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Department { get; set; } = null;


        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Subsidiary { get; set; } = null;


        [Column(TypeName = "ntext")]
        public string Note { get; set; } = null;

        public int? ImportId { get; set; } = null;

        public int Status { get; set; } = 0;
        /*
         * 0: Unassigned
         * 1: Active
         * 2: Inactive
         */

        public virtual ICollection<Phone> Phones { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }

    }
}
